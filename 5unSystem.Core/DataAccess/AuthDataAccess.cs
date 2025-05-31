using System;
using _5unSystem.Model.Entities;
using _5unSystem.Model.Enum;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace _5unSystem.Core.DataAccess;

public class AuthDataAccess
{
    public static User Login(User data)
    {
        using (var db = new DataContext())
        {
            var sql = $@"SELECT * FROM {User.TableName} WHERE {User.UserNameField} = @UserName 
                        AND {User.DeletedField} = 0";
            var user = db.Users.FromSqlRaw(sql, new SqlParameter
            {
                ParameterName = "@UserName",
                Value = data.UserName ?? string.Empty
            }).FirstOrDefault(); // Fix: Correct syntax for method chaining

            if (user == null)
                throw new Exception(ResponseLoginMessage.INVALID_CREDENTIALS);

            if (user.Password != data.Password)
                throw new Exception(ResponseLoginMessage.INVALID_CREDENTIALS);

            return user;
        }
    }
}
