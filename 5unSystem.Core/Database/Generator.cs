using System;
using _5unSystem.Model.Entities;
using _5unSystem.Core;
using Microsoft.EntityFrameworkCore;
using _5unSystem.Utility;
namespace _5unSystem.Core;

public class GeneratorDB
{
    public void EnsureDatabaseCreated()
    {
        using (var context = new DataContext())
        {
            context.Database.EnsureCreated();
            CreateTablesIfNotExist(context);
        }
    }

    private void CreateTablesIfNotExist(DataContext context)
    {
        if (!context.Database.CanConnect())
        {
            context.Database.EnsureCreated();
        }

        var modelTypes = typeof(DataContext).Assembly.GetTypes()
            .Where(t => t.Namespace == "_5unSystem.Model.Entities" && t.IsClass);

        foreach (var type in modelTypes)
        {
            var classObj = type.Name;
            var tableName = type.GetField("TableName")?.GetValue(null)?.ToString() ?? classObj;
            var tableExists = context.Database.ExecuteSqlRaw($"SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}'");
            if (tableExists == 0)
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
            else if (tableExists > 0)
            {
                // Optionally, you can check for columns and add them if they don't exist
                foreach (var property in type.GetProperties())
                {
                    var columnName = property.Name;
                    var columnType = GetSqlType(property.PropertyType);
                    var columnExists = context.Database.ExecuteSqlRaw($"SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}' AND COLUMN_NAME = '{columnName}'");
                    if (columnExists == 0)
                    {
                        context.Database.ExecuteSqlRaw($"ALTER TABLE {tableName} ADD {columnName} {columnType}");
                    }
                }
            }
        }

        SeedInitialData(context);

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



    private void SeedInitialData(DataContext context)
    {


        if (!context.Users.Any())
        {
            // var encrypt = new Encrypt();
            var roleID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var initialRoles = new List<Role>
            {
                new Role
                {
                    RoleID = roleID, // Ganti dengan ID Role yang sesuai
                    RoleName = "System Admin",
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedBy = null,
                    ModifiedAt = null,
                    Deleted = false
                }
            };

            context.Role.AddRange(initialRoles);
            
            var initialUsers = new List<User>();
            var password = "admin123";
            password = password.HashString();// Menggunakan fungsi HashPassword untuk meng-hash password
        {
                initialUsers.Add(new User
                {
                    UserName = "admin",
                    Password = password,
                    CreatedBy = "System",
                    Role_RoleID = roleID, // Ganti dengan ID Role yang sesuai
                    CreatedAt = DateTime.UtcNow,
                    ModifiedBy = null,
                    ModifiedAt = null,
                    Deleted = false
                });
        };

            context.Users.AddRange(initialUsers);
            context.SaveChanges();
        }
    }

}
