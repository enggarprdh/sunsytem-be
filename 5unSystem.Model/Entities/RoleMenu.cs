using System.ComponentModel.DataAnnotations;

namespace _5unSystem.Model.Entities
{
    public class RoleMenu
    {
        public const string TableName = "RoleMenu";
        public const string RoleIDField = "RoleID";
        public const string MenuIDField = "MenuID";
        public const string CreatedByField = "CreatedBy";
        public const string CreatedAtField = "CreatedAt";
        public const string ModifiedByField = "ModifiedBy";
        public const string ModifiedAtField = "ModifiedAt";
        public const string DeletedField = "Deleted";

        [Key]
        public Guid ID { get; set; }
        public Guid RoleID { get; set; }
        public String MenuID { get; set; }
        public bool isView { get; set; }
        public bool isAdd { get; set; }
        public bool isEdit { get; set; }
        public bool isDelete { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool Deleted { get; set; }
    }
}
