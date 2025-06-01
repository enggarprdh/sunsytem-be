using System;
using _5unSystem.Model.DTO;
using _5unSystem.Model.Entities;
using _5unSystem.Model.Enum;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace _5unSystem.Core.DataAccess;

public class AuthDataAccess
{
    public static LoginResponse Login(User data)
    {
        using (var db = new DataContext())
        {
            var sql = $@"SELECT U.{User.UserNameField}
                        , U.{User.PasswordField}
                        , R.{Role.RoleNameField}
                        FROM {User.TableName} U
                        INNER JOIN {Role.TableName} R ON U.{User.Role_RoleIDField} = R.{Role.RoleIDField}
                        WHERE U.{User.UserNameField} = @UserName 
                        AND U.{User.DeletedField} = 0";

            var command = db.Database.GetDbConnection().CreateCommand();
            command.CommandText = sql;
            command.Parameters.Add(new SqlParameter
            {
                ParameterName = "@UserName",
                Value = data.UserName ?? string.Empty
            });

            var dt = new System.Data.DataTable();
            using (var adapter = new SqlDataAdapter((SqlCommand)command))
            {
                adapter.Fill(dt);
            }

            if (dt.Rows.Count == 0)
                throw new Exception(ResponseLoginMessage.INVALID_CREDENTIALS);

            var loginresponse = new LoginResponse();
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                if (row[User.PasswordField].ToString() != data.Password)
                    throw new Exception(ResponseLoginMessage.INVALID_CREDENTIALS);

                loginresponse.UserName = row[User.UserNameField].ToString();
                loginresponse.Role = row[Role.RoleNameField].ToString();
            }
            return loginresponse;
        }
    }
}
