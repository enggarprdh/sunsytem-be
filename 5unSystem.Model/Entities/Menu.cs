using System.ComponentModel.DataAnnotations;

namespace _5unSystem.Model.Entities
{
    public class Menu
    {
        public const string TableName = "Menu";
        public const string MenuIDField = "MenuID";
        public const string DisplayNameField = "DisplayName";
        public const string MenuIconField = "MenuIcon";
        public const string ParentMenuIDField = "ParentMenuID";
        public const string IsHasSubMenuField = "IsHasSubMenu";
        public const string PathField = "Path";
        public const string CreatedByField = "CreatedBy";
        public const string CreatedAtField = "CreatedAt";
        public const string ModifiedByField = "ModifiedBy";
        public const string ModifiedAtField = "ModifiedAt";
        public const string DeletedField = "Deleted";
        public const string SequenceField = "Sequence";


        [Key]
        public string MenuID { get; set; }
        public string? DisplayName { get; set; }
        public string? MenuIcon { get; set; }
        public string? ParentMenuID { get; set; }
        public string? Path { get; set; }
        public int? Sequence { get; set; }
        public bool IsHasSubMenu { get; set; } = false;
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool Deleted { get; set; }
    }
}
