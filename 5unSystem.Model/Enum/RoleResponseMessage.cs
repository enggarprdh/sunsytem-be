using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _5unSystem.Model.Enum
{
    public class RoleResponseMessage
    {
        public const string ROLE_IS_NULL = ResponseMessage.PAYLOAD_IS_EMPTY;
        public const string ROLE_NOT_FOUND = "Role not found";
        public const string ROLE_NAME_EMPTY = "Role name cannot be empty";
        public const string ROLE_CREATED_SUCCESSFULLY = "Role created successfully";
        public const string ROLE_UPDATED_SUCCESSFULLY = "Role updated successfully";
        public const string ROLE_DELETED_SUCCESSFULLY = "Role deleted successfully";
        public const string ROLE_ALREADY_EXISTS = "Role already exists";
        public const string ROLE_ID_EMPTY = "Role ID cannot be empty";
        public const string ROLE_INVALID_DATA = "Invalid role data";
    }
}
