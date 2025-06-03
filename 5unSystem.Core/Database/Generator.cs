using System;
using _5unSystem.Model.Entities;
using _5unSystem.Core;
using Microsoft.EntityFrameworkCore;
using _5unSystem.Utility;
using _5unSystem.Model.DTO;
using Microsoft.Data.SqlClient;
namespace _5unSystem.Core;

public class GeneratorDB
{
    private Guid SYSTEM_ADMIN_ROLE_ID = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private const string SYSTEM_ADMIN_ROLE_NAME = "System Admin";
    private const string SYSTEM_ADMIN_USER_NAME = "admin";
    private const string SYSTEM_ADMIN_USER_PASSWORD = "admin123"; // Default password for the admin user
    private const string GENERATED_BY = "System";

    public void EnsureDatabaseCreated()
    {
        using (var context = new DataContext())
        {
            context.Database.EnsureCreated();
            CreateTablesIfNotExist(context);
            SeedInitialData(context);
            SeedMenu(context);
            SeedRoleMenu(context);
        }
    }

    private void CreateTablesIfNotExist(DataContext context)
    {
        if (!context.Database.CanConnect())
        {
            context.Database.EnsureCreated();
        }

        //var modelTypes = typeof(DataContext).Assembly.GetTypes()
        //    .Where(t => t.Namespace == "_5unSystem.Model.Entities" && t.IsClass).ToList();

        var modelTypes = typeof(EntitiesList).Assembly.GetTypes()
            .Where(t => t.Namespace == "_5unSystem.Model.Entities" && t.IsClass)
            .ToList();

        foreach (var type in modelTypes)
        {
            if (type.Name == "EntitiesList")
                continue; // Skip the EntitiesList class
            var classObj = type.Name;
            var tableName = type.GetField("TableName")?.GetValue(null)?.ToString() ?? classObj;
            using var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = $"SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName";
            command.Parameters.Add(new SqlParameter("@tableName", tableName));
            context.Database.OpenConnection();
            var result = command.ExecuteScalar();
            bool tableExists = result != null;
            if (!tableExists)
            {
                var createTableSql = $"CREATE TABLE {tableName} (";
                var properties = type.GetProperties();
                foreach (var property in properties)
                {
                    var propertyName = property.Name;
                    var propertyType = GetSqlType(property.PropertyType);
                    createTableSql += $"{propertyName} {propertyType},";
                }
                createTableSql = createTableSql.TrimEnd(',') + ")";
                context.Database.ExecuteSqlRaw(createTableSql);
            }
            else if (tableExists)
            {
                // Optionally, you can check for columns and add them if they don't exist
                foreach (var property in type.GetProperties())
                {
                    var columnName = property.Name;
                    command.CommandText = $"SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName";
                    command.Parameters.Clear(); // Clear previous parameters
                    command.Parameters.Add(new SqlParameter("@tableName", tableName));
                    command.Parameters.Add(new SqlParameter("@columnName", columnName));
                    context.Database.OpenConnection();
                    var columnExists = command.ExecuteScalar() != null;
                    var columnType = GetSqlType(property.PropertyType);
                    if (!columnExists)
                    {
                        context.Database.ExecuteSqlRaw($"ALTER TABLE {tableName} ADD {columnName} {columnType}");
                    }
                }
            }
        }


    }

    public bool CheckDatabaseConnection()
    {
        using (var context = new DataContext())
        {
            try
            {
                context.Database.OpenConnection();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    private string GetSqlType(Type type)
    {
        if (type == typeof(string))
            return "NVARCHAR(MAX)";
        else if (type == typeof(int))
            return "INT";
        else if (type == typeof(long))
            return "BIGINT";
        else if (type == typeof(bool))
            return "BIT";
        else if (type == typeof(DateTime))
            return "DATETIME";
        else if (type == typeof(Guid))
            return "UNIQUEIDENTIFIER";
        else
            return "NVARCHAR(MAX)";
    }

    private void SeedMenu(DataContext context)
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "menu.json");
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Menu JSON file not found: {filePath}");
        var json = File.ReadAllText(filePath);

        var menus = System.Text.Json.JsonSerializer.Deserialize<List<MenuResponse>>(json, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (menus == null || !menus.Any())
            throw new InvalidOperationException("No menus found in the JSON file.");
        foreach (var menu in menus)
        {
            // Cek apakah menu sudah ada berdasarkan MenuID
            var existingMenu = context.Menu.FirstOrDefault(m => m.MenuID == menu.MenuID);


            var objParent = new Menu();
            if (existingMenu != null)
                objParent = existingMenu; // Jika menu sudah ada, update data yang ada


            objParent.MenuID = menu.MenuID;
            objParent.DisplayName = menu.DisplayName;
            objParent.MenuIcon = menu.Icon;
            objParent.Path = menu.Path;
            objParent.IsHasSubMenu = menu.IsHasSubMenu;
            objParent.Sequence = menu.Sequence;

            objParent.Deleted = false;

            if (existingMenu != null)
            {
                // Update existing menu
                objParent.ModifiedBy = GENERATED_BY;
                objParent.ModifiedAt = DateTime.UtcNow;
                context.Menu.Update(objParent);
            }
            else
            {
                // Add new menu
                objParent.CreatedBy = GENERATED_BY;
                objParent.CreatedAt = DateTime.UtcNow;
                context.Menu.Add(objParent);
            }

            if (objParent.IsHasSubMenu)
            {
                foreach (var sub in menu.SubMenu)
                {
                    // Cek apakah submenu sudah ada berdasarkan MenuID
                    var existingSubMenu = context.Menu.FirstOrDefault(m => m.MenuID == sub.MenuID);
                    var objSub = new Menu();

                    if (existingSubMenu != null)
                        objSub = existingSubMenu; // Jika submenu sudah ada, update data yang ada

                    objSub.MenuID = sub.MenuID;
                    objSub.ParentMenuID = objParent.MenuID; // Set ParentMenuID to the parent menu
                    objSub.DisplayName = sub.DisplayName;
                    objSub.Path = sub.Path;
                    objSub.IsHasSubMenu = false; // Submenu tidak memiliki submenu lagi
                    objSub.Deleted = false;
                    objSub.Sequence = sub.Sequence;

                    if (existingSubMenu != null)
                    {
                        // Update existing submenu
                        objSub.ModifiedBy = GENERATED_BY;
                        objSub.ModifiedAt = DateTime.UtcNow;
                        context.Menu.Update(objSub);
                    }
                    else
                    {
                        // Add new submenu
                        objSub.CreatedBy = GENERATED_BY;
                        objSub.CreatedAt = DateTime.UtcNow;
                        context.Menu.Add(objSub);
                    }

                }
            }
        }

        try
        {
            context.SaveChanges();
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            Console.WriteLine($"Error saving menu data: {ex.Message}");
        }

    }
    private void SeedInitialData(DataContext context)
    {


        if (!context.Users.Any())
        {
            // var encrypt = new Encrypt();
            var roleID = SYSTEM_ADMIN_ROLE_ID;
            var initialRoles = new List<Role>
            {
                new Role
                {
                    RoleID = roleID, // Ganti dengan ID Role yang sesuai
                    RoleName = SYSTEM_ADMIN_ROLE_NAME,
                    CreatedBy = GENERATED_BY,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedBy = null,
                    ModifiedAt = null,
                    Deleted = false
                }
            };

            context.Role.AddRange(initialRoles);

            var initialUsers = new List<User>();
            var password = SYSTEM_ADMIN_USER_PASSWORD;
            password = password.HashString();// Menggunakan fungsi HashPassword untuk meng-hash password

            initialUsers.Add(new User
            {
                UserName = SYSTEM_ADMIN_USER_NAME,
                Password = password,
                CreatedBy = GENERATED_BY,
                Role_RoleID = roleID, // Ganti dengan ID Role yang sesuai
                CreatedAt = DateTime.UtcNow,
                ModifiedBy = null,
                ModifiedAt = null,
                Deleted = false
            });

            context.Users.AddRange(initialUsers);
            context.SaveChanges();
        }
    }
    private void SeedRoleMenu(DataContext context)
    {

        var menus = context.Menu.ToList();
        var menuParent = menus.Where(m => m.ParentMenuID == null).ToList();
        var roleMenuExists = context.RoleMenu.Where(rm => rm.RoleID == SYSTEM_ADMIN_ROLE_ID).ToList();
        if (roleMenuExists.Count > 0)
        {
            context.RemoveRange(roleMenuExists); // Hapus RoleMenu yang sudah ada untuk Role Admin
        }
        var roleMenus = new List<RoleMenu>();

        foreach (var parent in menuParent)
        {
            var roleMenu = new RoleMenu
            {
                ID = Guid.NewGuid(), // Generate a new ID for the RoleMenu
                RoleID = SYSTEM_ADMIN_ROLE_ID, // Ganti dengan ID Role yang sesuai
                MenuID = parent.MenuID,
                isView = true, // Atur sesuai kebutuhan
                isAdd = true, // Atur sesuai kebutuhan
                isEdit = true, // Atur sesuai kebutuhan
                isDelete = true, // Atur sesuai kebutuhan
                CreatedBy = GENERATED_BY,
                CreatedAt = DateTime.UtcNow,
                ModifiedBy = null,
                ModifiedAt = null,
                Deleted = false
            };

            roleMenus.Add(roleMenu);
            // Cek apakah parent menu memiliki sub-menu
            if (parent.IsHasSubMenu)
            {
                var subMenu = menus.Where(m => m.ParentMenuID == parent.MenuID).ToList();
                foreach (var sub in subMenu)
                {
                    // Cek apakah sub-menu sudah ada dalam RoleMenu
                    var existingRoleMenu = roleMenus.FirstOrDefault(rm => rm.MenuID == sub.MenuID && rm.RoleID == SYSTEM_ADMIN_ROLE_ID);
                    if (existingRoleMenu != null)
                        continue; // Jika sudah ada, lewati penambahan RoleMenu untuk sub-menu

                    var subRoleMenu = new RoleMenu
                    {
                        ID = Guid.NewGuid(), // Generate a new ID for the RoleMenu
                        RoleID = SYSTEM_ADMIN_ROLE_ID, // Ganti dengan ID Role yang sesuai
                        MenuID = sub.MenuID,
                        isView = true, // Atur sesuai kebutuhan
                        isAdd = true, // Atur sesuai kebutuhan
                        isEdit = true, // Atur sesuai kebutuhan
                        isDelete = true, // Atur sesuai kebutuhan
                        CreatedBy = GENERATED_BY,
                        CreatedAt = DateTime.UtcNow,
                        ModifiedBy = null,
                        ModifiedAt = null,
                        Deleted = false
                    };
                    roleMenus.Add(subRoleMenu);
                }
            }
        }
        context.RoleMenu.AddRange(roleMenus);
        try
        {
            context.SaveChanges();
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            Console.WriteLine($"Error saving role menu data: {ex.Message}");
        }


    }

}
