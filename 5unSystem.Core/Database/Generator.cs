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
            var tableName = type.Name;
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
            var initialUsers = new List<User>();
            var password = "admin123";
            password = password.HashString();// Menggunakan fungsi HashPassword untuk meng-hash password
        {
                initialUsers.Add(new User
                {
                    UserName = "admin",
                    Password = password,
                    CreatedBy = "System",
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
