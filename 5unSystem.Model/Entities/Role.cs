using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _5unSystem.Model.Entities
{
    public class Role
    {
        public const string TableName = "Role";
        public const string RoleIDField = "RoleID";
        public const string RoleNameField = "RoleName";
        public const string CreatedByField = "CreatedBy";
        public const string CreatedAtField = "CreatedAt";
        public const string ModifiedByField = "ModifiedBy";
        public const string ModifiedAtField = "ModifiedAt";
        public const string DeletedField = "Deleted";

        [Key]
        public Guid RoleID { get; set; }
        public string? RoleName { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool Deleted { get; set; }
    }
}
