using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.ErrorMessages
{
    public static class ErrorMessages
    {
        // Publisher
        public const string PublisherNotFound = "Publisher not found.";

        // Author
        public const string AuthorNotFound = "Author not found.";

        // Category
        public const string CategoryNotFound = "Category not found.";

        // Book
        public const string BookNotFound = "Book not found.";
        public const string NoCopiesAvailable = "No copies of this book are currently available.";

        // Member
        public const string MemberNotFound = "Member not found.";

        // SystemUser
        public const string UserNotFound = "User not found.";
        public const string EmailAlreadyExists = "A user with this email already exists.";

        // Loan
        public const string LoanNotFound = "Loan not found.";
        public const string LoanAlreadyReturned = "This loan has already been returned.";
        public const string BorrowConflictRetryExceeded = "Could not complete the borrow request due to a conflicting update — please try again.";
        public const string ReturnConflictRetryExceeded = "Could not complete the return request due to a conflicting update — please try again.";
        public const string EntityNotFound = "The requested item was not found.";
        public const string InvalidCredentials = "Invalid email or password.";
        public const string InvalidRefreshToken = "Invalid refresh token.";
        public const string RefreshTokenExpired = "Refresh token has expired. Please log in again.";
    }
}
