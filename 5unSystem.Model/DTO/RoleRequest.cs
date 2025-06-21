using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _5unSystem.Model.DTO
{
    public class RoleCreateOrUpdateRequest
    {
        public string RoleName { get; set; }
    }

    public class RoleUpdateRequest
    {
        public Guid RoleID { get; set; }
        public string RoleName { get; set; }
    }
}
