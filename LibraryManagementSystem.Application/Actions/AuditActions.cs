using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.Actions
{
    public static class AuditActions
    {
        public const string CreateBook = "CREATE_BOOK";
        public const string UpdateBook = "UPDATE_BOOK";
        public const string DeleteBook = "DELETE_BOOK";

        public const string CreateAuthor = "CREATE_AUTHOR";
        public const string UpdateAuthor = "UPDATE_AUTHOR";
        public const string DeleteAuthor = "DELETE_AUTHOR";

        public const string CreateCategory = "CREATE_CATEGORY";
        public const string UpdateCategory = "UPDATE_CATEGORY";
        public const string DeleteCategory = "DELETE_CATEGORY";

        public const string CreatePublisher = "CREATE_PUBLISHER";
        public const string UpdatePublisher = "UPDATE_PUBLISHER";
        public const string DeletePublisher = "DELETE_PUBLISHER";

        public const string CreateMember = "CREATE_MEMBER";
        public const string UpdateMember = "UPDATE_MEMBER";
        public const string DeleteMember = "DELETE_MEMBER";

        public const string CreateSystemUser = "CREATE_SYSTEM_USER";
        public const string UpdateSystemUser = "UPDATE_SYSTEM_USER";
        public const string DeleteSystemUser = "DELETE_SYSTEM_USER";

        public const string ProcessLoan = "PROCESS_LOAN";
        public const string ProcessReturn = "PROCESS_RETURN";
        public const string SystemMarkOverdue = "SYSTEM_MARK_OVERDUE";

        public const string Login = "LOGIN";
        public const string RefreshToken = "REFRESH_TOKEN";
    }
}
