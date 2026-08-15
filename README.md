# Library Management System

A RESTful API for managing books, members, system users, and borrowing transactions, built with ASP.NET Core, EF Core, and SQL Server following Clean Architecture.

## ERD

![Library Management System ERD](images/erd.png)
## API Testing Collection

A ready-to-import Postman/Apidog collection is included at [`postman/LibraryManagementSystem.postman_collection.json`](postman/LibraryManagementSystem.postman_collection.json).
## Design Decisions

### Schema

- **Three separate identity tables**: `SystemUser` (authenticates, has RBAC), `Member` (a data record — never logs in, staff act on their behalf), and `Author` (pure book metadata). Each has a distinct responsibility and lifecycle.
- **Publisher is one-per-book**, via a plain `PublisherId` foreign key on `Book`, not a many-to-many junction. A library copy corresponds to one printed edition; different publishers of the same title are separate `Book` rows with their own ISBN/edition.
- **Book availability is derived from `Quantity`**, adjusted directly during borrow/return. There is no separate `status`/`in-out` flag, so there's nothing that can drift out of sync with the actual loan data.
- **`AuditLog` is scoped to `SystemUser` actions only**, satisfying the "user activity logging" requirement. Members never perform authenticated actions, so nothing about them is logged.
- **Soft delete** (`IsDeleted`) on `Book`, `Publisher`, `Author`, `Member`, and `SystemUser`, enforced via EF Core global query filters. `Category` uses hard delete, since it carries no history that needs preserving.

### Concurrency

- **Optimistic concurrency** via EF Core's `RowVersion` (`[Timestamp]`) on `Book`. Borrowing/returning decrements/increments `Quantity` directly; a `DbUpdateConcurrencyException` on conflict triggers an automatic retry (up to 3 attempts) rather than surfacing a raw database error to the client.
- **Book, Loan, and AuditLog writes commit as a single unit of work** (`IUnitOfWork.SaveChangesAsync()`), so a conflict never leaves a decremented `Quantity` with no corresponding `Loan` record.

### RBAC

Three roles, each with a distinct purpose:

| Role | Responsibilities |
|---|---|
| **Administrator** | Manages `SystemUser` accounts (create/update/delete) — the only role with this access |
| **Librarian** | Catalogs books/authors/categories/publishers; processes borrow/return transactions; manages members |
| **Staff** | Read-only catalog/member access, plus a dedicated statistics/reporting dashboard |

### Architecture

Clean Architecture, 4 layers, dependencies point inward:

```
Domain          → entities, enums. No dependencies.
Application     → interfaces, DTOs, business rules. Depends on Domain.
Infrastructure  → EF Core, repository & service implementations. Depends on Application.
Presentation    → controllers, middleware, DI composition root.
```

`Presentation` is the only layer that references `Infrastructure` directly, and only for DI wiring in `Program.cs` — controllers depend exclusively on `Application` interfaces.

- **Generic repository + Unit of Work**, with dedicated interfaces where an entity needs more than plain CRUD (`IBookRepository` for the concurrency-safe borrow query and pagination, `ILoanRepository` for active/overdue queries, `IRefreshTokenRepository` for token lookup/revocation).
- **Generic service base class** applied to genuinely plain-CRUD entities (`Publisher`, `Author`, `Category`, `Member`). `Book`, `SystemUser`, and `Loan` are fully custom services, since each carries business logic that doesn't fit a generic shape.

### Auth

- **JWT access tokens + refresh token rotation.** Refresh tokens are hashed before storage, never stored raw. Each refresh operation revokes the old token and issues a new one, limiting the blast radius of a stolen refresh token to a single use.

### Background processing

- A scheduled `BackgroundService` marks loans `Overdue` once `DueDate` has passed. Actions it takes are attributed in `AuditLog` to the first `Administrator` account, with `Action = "SYSTEM_MARK_OVERDUE"` so automated entries are distinguishable from real staff actions in the audit trail.

### Caching

- `IMemoryCache` is used for read-heavy, low-churn reference data (categories, publishers, authors, statistics dashboard) — never for `Book.Quantity`/availability, since that value is protected by the concurrency mechanism above and must always be read fresh.

---

## SQL Seed Script

```sql
------------------------------------------------------------
-- SYSTEM USERS
-- NOTE: PasswordHash values below are real ASP.NET Core Identity hashes,
-- generated once via IPasswordHasher<SystemUser> in-app.
------------------------------------------------------------

INSERT INTO SystemUsers (Id, Name, Email, PasswordHash, RoleType, CreatedAt, UpdatedAt, IsDeleted) VALUES
('017F6205-00A6-4EAF-B02C-53F5207F8555', 'Tamer',   'tamer@gmail.com',   'AQAAAAIAAYagAAAAEHWE+bSViPkj3Ip3FI1KiAUBEx5qhS2coz+5mdbkHyRMAPpl86st4gKF83dY488z0w==', 'Staff',         GETUTCDATE(), GETUTCDATE(), 0),
('81128A5B-BD0A-4F2C-8E5D-0D23B07B1FBB', 'mahmoud', 'mahmoud@gmail.com', 'AQAAAAIAAYagAAAAEINYxrQvQTjZnSrnJ5eCQddprzf14Ju57IPVyGQwlgMZ8tINvZyDiAaLgw+gxYtD7g==', 'Administrator', GETUTCDATE(), GETUTCDATE(), 0),
('6DC9E3DE-E0BC-4D82-B579-CEE6BA1403B6', 'Ahmed',   'Ahmed@gmail.com',   'AQAAAAIAAYagAAAAEEb5HjRiHGi/LoW46zrXcMYRJvdBaUJrxmCL9ZR9wq1Qohy+GfIvQmJpkLoiPL4yAQ==', 'Librarian',     GETUTCDATE(), GETUTCDATE(), 0);

------------------------------------------------------------
-- PUBLISHERS
------------------------------------------------------------

INSERT INTO Publishers (Id, Name, CreatedAt, UpdatedAt, IsDeleted) VALUES
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Penguin Random House', GETUTCDATE(), GETUTCDATE(), 0),
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'HarperCollins', GETUTCDATE(), GETUTCDATE(), 0),
('cccccccc-cccc-cccc-cccc-cccccccccccc', 'Oxford University Press', GETUTCDATE(), GETUTCDATE(), 0),
('dddddddd-dddd-dddd-dddd-dddddddddddd', 'O''Reilly Media', GETUTCDATE(), GETUTCDATE(), 0);

------------------------------------------------------------
-- AUTHORS
------------------------------------------------------------

INSERT INTO Authors (Id, Name, Bio, CreatedAt, UpdatedAt, IsDeleted) VALUES
('11111111-aaaa-aaaa-aaaa-111111111111', 'Robert C. Martin', 'Software engineer and author known for books about software development.', GETUTCDATE(), GETUTCDATE(), 0),
('22222222-aaaa-aaaa-aaaa-222222222222', 'Martin Fowler', 'Author and software developer known for his work on software architecture.', GETUTCDATE(), GETUTCDATE(), 0),
('33333333-aaaa-aaaa-aaaa-333333333333', 'Andrew Hunt', 'Software developer and co-author of The Pragmatic Programmer.', GETUTCDATE(), GETUTCDATE(), 0),
('44444444-aaaa-aaaa-aaaa-444444444444', 'Eric Freeman', 'Author and software engineer specializing in software development.', GETUTCDATE(), GETUTCDATE(), 0);

------------------------------------------------------------
-- CATEGORIES
------------------------------------------------------------

INSERT INTO Categories (Id, Name) VALUES
('11111111-bbbb-bbbb-bbbb-111111111111', 'Programming'),
('22222222-bbbb-bbbb-bbbb-222222222222', 'Software Architecture'),
('33333333-bbbb-bbbb-bbbb-333333333333', 'Database'),
('44444444-bbbb-bbbb-bbbb-444444444444', 'Web Development'),
('55555555-bbbb-bbbb-bbbb-555555555555', 'Computer Science');

------------------------------------------------------------
-- MEMBERS
------------------------------------------------------------

INSERT INTO Members (Id, Name, Email, Phone, MembershipStatus, CreatedAt, UpdatedAt, IsDeleted) VALUES
('11111111-cccc-cccc-cccc-111111111111', 'Ahmed Hassan', 'ahmed.hassan@example.com', '01012345678', 'Active', GETUTCDATE(), GETUTCDATE(), 0),
('22222222-cccc-cccc-cccc-222222222222', 'Omar Mohamed', 'omar.mohamed@example.com', '01112345678', 'Active', GETUTCDATE(), GETUTCDATE(), 0),
('33333333-cccc-cccc-cccc-333333333333', 'Sara Ali', 'sara.ali@example.com', '01212345678', 'Active', GETUTCDATE(), GETUTCDATE(), 0),
('44444444-cccc-cccc-cccc-444444444444', 'Mariam Adel', 'mariam.adel@example.com', '01512345678', 'Suspended', GETUTCDATE(), GETUTCDATE(), 0),
('55555555-cccc-cccc-cccc-555555555555', 'Youssef Ibrahim', 'youssef.ibrahim@example.com', '01098765432', 'Expired', GETUTCDATE(), GETUTCDATE(), 0);

------------------------------------------------------------
-- BOOKS
-- NOTE: RowVersion is auto-generated by SQL Server — never inserted manually.
------------------------------------------------------------

INSERT INTO Books (Id, Title, Isbn, PublishYear, Edition, Language, PageCount, PublisherId, Quantity, Metadata, CreatedAt, UpdatedAt, IsDeleted) VALUES
('11111111-dddd-dddd-dddd-111111111111', 'Clean Code', '9780132350884', 2008, '1st Edition', 'English', 464, 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 5, '{"coverImageUrl":"https://example.com/clean-code.jpg","notes":"Software craftsmanship"}', GETUTCDATE(), GETUTCDATE(), 0),
('22222222-dddd-dddd-dddd-222222222222', 'Refactoring', '9780134757599', 2018, '2nd Edition', 'English', 448, 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 3, '{"coverImageUrl":"https://example.com/refactoring.jpg","notes":"Improving existing code"}', GETUTCDATE(), GETUTCDATE(), 0),
('33333333-dddd-dddd-dddd-333333333333', 'The Pragmatic Programmer', '9780135957059', 2019, '2nd Edition', 'English', 352, 'cccccccc-cccc-cccc-cccc-cccccccccccc', 4, '{"coverImageUrl":"https://example.com/pragmatic.jpg","notes":"Programming best practices"}', GETUTCDATE(), GETUTCDATE(), 0),
('44444444-dddd-dddd-dddd-444444444444', 'Head First Design Patterns', '9780596007126', 2004, '1st Edition', 'English', 694, 'dddddddd-dddd-dddd-dddd-dddddddddddd', 2, '{"coverImageUrl":"https://example.com/design-patterns.jpg","notes":"Design patterns"}', GETUTCDATE(), GETUTCDATE(), 0),
('55555555-dddd-dddd-dddd-555555555555', 'Domain-Driven Design', '9780321125217', 2003, '1st Edition', 'English', 560, 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 3, '{"coverImageUrl":"https://example.com/ddd.jpg","notes":"Domain modeling"}', GETUTCDATE(), GETUTCDATE(), 0);

------------------------------------------------------------
-- BOOK_AUTHOR
------------------------------------------------------------

INSERT INTO Book_Author (AuthorsId, BooksId) VALUES
('11111111-aaaa-aaaa-aaaa-111111111111', '11111111-dddd-dddd-dddd-111111111111'), -- Clean Code -> Robert C. Martin
('22222222-aaaa-aaaa-aaaa-222222222222', '22222222-dddd-dddd-dddd-222222222222'), -- Refactoring -> Martin Fowler
('33333333-aaaa-aaaa-aaaa-333333333333', '33333333-dddd-dddd-dddd-333333333333'), -- Pragmatic Programmer -> Andrew Hunt
('44444444-aaaa-aaaa-aaaa-444444444444', '44444444-dddd-dddd-dddd-444444444444'), -- Design Patterns -> Eric Freeman
('22222222-aaaa-aaaa-aaaa-222222222222', '55555555-dddd-dddd-dddd-555555555555'); -- DDD -> Martin Fowler

------------------------------------------------------------
-- BOOK_CATEGORY
------------------------------------------------------------

INSERT INTO Book_Category (BooksId, CategoriesId) VALUES
('11111111-dddd-dddd-dddd-111111111111', '11111111-bbbb-bbbb-bbbb-111111111111'), -- Clean Code -> Programming
('11111111-dddd-dddd-dddd-111111111111', '22222222-bbbb-bbbb-bbbb-222222222222'), -- Clean Code -> Software Architecture
('22222222-dddd-dddd-dddd-222222222222', '11111111-bbbb-bbbb-bbbb-111111111111'), -- Refactoring -> Programming
('22222222-dddd-dddd-dddd-222222222222', '22222222-bbbb-bbbb-bbbb-222222222222'), -- Refactoring -> Software Architecture
('33333333-dddd-dddd-dddd-333333333333', '11111111-bbbb-bbbb-bbbb-111111111111'), -- Pragmatic Programmer -> Programming
('44444444-dddd-dddd-dddd-444444444444', '11111111-bbbb-bbbb-bbbb-111111111111'), -- Design Patterns -> Programming
('44444444-dddd-dddd-dddd-444444444444', '22222222-bbbb-bbbb-bbbb-222222222222'), -- Design Patterns -> Software Architecture
('55555555-dddd-dddd-dddd-555555555555', '22222222-bbbb-bbbb-bbbb-222222222222'); -- DDD -> Software Architecture

------------------------------------------------------------
-- LOANS
-- One active, one returned, one overdue (set directly here for demo
-- purposes only — in the running app, Overdue is only ever set by
-- OverdueLoanBackgroundService, never written directly by a client).
------------------------------------------------------------

INSERT INTO Loans (Id, BookId, MemberId, ProcessedByUserId, BorrowedAt, DueDate, ReturnedAt, Fine, Status, CreatedAt, UpdatedAt) VALUES
('11111111-eeee-eeee-eeee-111111111111', '11111111-dddd-dddd-dddd-111111111111', '11111111-cccc-cccc-cccc-111111111111', '81128A5B-BD0A-4F2C-8E5D-0D23B07B1FBB', DATEADD(DAY, -5, GETUTCDATE()), DATEADD(DAY, 9, GETUTCDATE()), NULL, NULL, 'Active', GETUTCDATE(), GETUTCDATE()),
('22222222-eeee-eeee-eeee-222222222222', '22222222-dddd-dddd-dddd-222222222222', '22222222-cccc-cccc-cccc-222222222222', '017F6205-00A6-4EAF-B02C-53F5207F8555', DATEADD(DAY, -30, GETUTCDATE()), DATEADD(DAY, -16, GETUTCDATE()), DATEADD(DAY, -18, GETUTCDATE()), 0.00, 'Returned', GETUTCDATE(), GETUTCDATE()),
('33333333-eeee-eeee-eeee-333333333333', '33333333-dddd-dddd-dddd-333333333333', '33333333-cccc-cccc-cccc-333333333333', '6DC9E3DE-E0BC-4D82-B579-CEE6BA1403B6', DATEADD(DAY, -25, GETUTCDATE()), DATEADD(DAY, -11, GETUTCDATE()), NULL, 50.00, 'Overdue', GETUTCDATE(), GETUTCDATE()),
('44444444-eeee-eeee-eeee-444444444444', '44444444-dddd-dddd-dddd-444444444444', '55555555-cccc-cccc-cccc-555555555555', '81128A5B-BD0A-4F2C-8E5D-0D23B07B1FBB', DATEADD(DAY, -2, GETUTCDATE()), DATEADD(DAY, 12, GETUTCDATE()), NULL, NULL, 'Active', GETUTCDATE(), GETUTCDATE());

------------------------------------------------------------
-- AUDIT LOGS
------------------------------------------------------------

INSERT INTO AuditLogs (Id, PerformedByUserId, Action, EntityType, EntityId, OldValue, NewValue, Timestamp) VALUES
('11111111-ffff-ffff-ffff-111111111111', '81128A5B-BD0A-4F2C-8E5D-0D23B07B1FBB', 'CREATE_BOOK', 'Book', '11111111-dddd-dddd-dddd-111111111111', NULL, '{"title":"Clean Code"}', GETUTCDATE()),
('22222222-ffff-ffff-ffff-222222222222', '017F6205-00A6-4EAF-B02C-53F5207F8555', 'CREATE_MEMBER', 'Member', '11111111-cccc-cccc-cccc-111111111111', NULL, '{"name":"Ahmed Hassan"}', GETUTCDATE()),
('33333333-ffff-ffff-ffff-333333333333', '6DC9E3DE-E0BC-4D82-B579-CEE6BA1403B6', 'UPDATE_BOOK', 'Book', '22222222-dddd-dddd-dddd-222222222222', '{"quantity":2}', '{"quantity":3}', GETUTCDATE());

------------------------------------------------------------
-- REFRESH TOKENS
-- NOTE: Token values below are placeholders for seed/demo purposes only —
-- real tokens are generated and hashed at login time via AuthService.
------------------------------------------------------------

INSERT INTO RefreshTokens (Id, UserId, Token, ExpiresAt, IsRevoked, CreatedAt) VALUES
('11111111-9999-9999-9999-111111111111', '81128A5B-BD0A-4F2C-8E5D-0D23B07B1FBB', 'test-refresh-token-mahmoud-001', DATEADD(DAY, 30, GETUTCDATE()), 0, GETUTCDATE()),
('22222222-9999-9999-9999-222222222222', '017F6205-00A6-4EAF-B02C-53F5207F8555', 'test-refresh-token-tamer-001', DATEADD(DAY, 30, GETUTCDATE()), 0, GETUTCDATE());
```

---

## API Requests

### Auth

**`POST /api/auth/login`**
```json
{
  "email": "mahmoud@gmail.com",
  "password": "123123123"
}
```
Response:
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "b64-random-string",
  "expiresAt": "2026-08-15T12:15:00Z"
}
```

**`POST /api/auth/refresh`**
```json
{
  "refreshToken": "b64-random-string"
}
```

**`POST /api/auth/revoke`**
```json
{
  "refreshToken": "b64-random-string"
}
```

### System Users (Administrator only)

**`POST /api/systemusers`**
```json
{
  "name": "Layla Librarian",
  "email": "librarian@library.com",
  "password": "Librarian@12345",
  "roleType": "Librarian"
}
```

**`PUT /api/systemusers/{id}`**
```json
{
  "name": "Layla Librarian",
  "email": "librarian@library.com",
  "roleType": "Librarian"
}
```

### Publishers

**`POST /api/publishers`**
```json
{
  "name": "O'Reilly Media"
}
```

### Authors

**`POST /api/authors`**
```json
{
  "name": "Robert C. Martin",
  "bio": "Software engineer and author known for books about software development."
}
```

### Categories

**`POST /api/categories`**
```json
{
  "name": "Software Engineering"
}
```

### Books

**`POST /api/books`**
```json
{
  "title": "Clean Code",
  "isbn": "9780132350884",
  "publishYear": 2008,
  "edition": "1st",
  "language": "English",
  "pageCount": 464,
  "publisherId": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
  "quantity": 5,
  "metadata": {
    "coverImageUrl": "https://example.com/clean-code.jpg",
    "notes": "Popular pick for junior developer onboarding.",
    "awards": ["Jolt Productivity Award 2009"]
  },
  "authorIds": ["11111111-aaaa-aaaa-aaaa-111111111111"],
  "categoryIds": ["11111111-bbbb-bbbb-bbbb-111111111111"]
}
```

**`GET /api/books?search=clean&categoryId=11111111-bbbb-bbbb-bbbb-111111111111&page=1&pageSize=20`**

Response:
```json
{
  "items": [
    {
      "id": "11111111-dddd-dddd-dddd-111111111111",
      "title": "Clean Code",
      "isbn": "9780132350884",
      "publishYear": 2008,
      "quantity": 5,
      "availableCopies": 4,
      "publisherName": "Penguin Random House",
      "authorNames": ["Robert C. Martin"],
      "categoryNames": ["Programming"],
      "metadata": {
        "coverImageUrl": "https://example.com/clean-code.jpg",
        "notes": "Popular pick for junior developer onboarding."
      }
    }
  ],
  "totalCount": 1,
  "page": 1,
  "pageSize": 20,
  "totalPages": 1
}
```

### Members

**`POST /api/members`**
```json
{
  "name": "Ahmed Hassan",
  "email": "ahmed.hassan@example.com",
  "phone": "01012345678",
  "membershipStatus": "Active"
}
```

### Loans

**`POST /api/loans`** (borrow)
```json
{
  "bookId": "11111111-dddd-dddd-dddd-111111111111",
  "memberId": "11111111-cccc-cccc-cccc-111111111111"
}
```
Response:
```json
{
  "id": "11111111-eeee-eeee-eeee-111111111111",
  "bookId": "11111111-dddd-dddd-dddd-111111111111",
  "bookTitle": "Clean Code",
  "memberId": "11111111-cccc-cccc-cccc-111111111111",
  "processedByUserId": "81128a5b-bd0a-4f2c-8e5d-0d23b07b1fbb",
  "borrowedAt": "2026-08-15T10:00:00Z",
  "dueDate": "2026-08-29T10:00:00Z",
  "returnedAt": null,
  "fine": null,
  "status": "Active"
}
```

**`POST /api/loans/{id}/return`** — no request body.

**`GET /api/loans/active`**

**`GET /api/loans/overdue`**

### Statistics

**`GET /api/statistics`**
```json
{
  "totalBooks": 5,
  "totalCopies": 17,
  "currentlyBorrowed": 2,
  "overdueCount": 1,
  "activeMembersCount": 3,
  "mostBorrowedBooks": [
    { "bookId": "11111111-dddd-dddd-dddd-111111111111", "title": "Clean Code", "borrowCount": 4 }
  ]
}
```
