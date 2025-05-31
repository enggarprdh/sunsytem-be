using System.ComponentModel.DataAnnotations;

namespace _5unSystem.Model.Entities
{
    public class User
    {
        public const string TableName = "Users";    
        public const string UserNameField = "UserName";
        public const string PasswordField = "Password";
        public const string CreatedByField = "CreatedBy";
        public const string CreatedAtField = "CreatedAt";
        public const string ModifiedByField = "ModifiedBy";
        public const string ModifiedAtField = "ModifiedAt";
        public const string Role_RoleIDField = "Role_RoleID";
        public const string DeletedField = "Deleted";

        [Key]
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public Guid? Role_RoleID { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool Deleted { get; set; }
    }
}