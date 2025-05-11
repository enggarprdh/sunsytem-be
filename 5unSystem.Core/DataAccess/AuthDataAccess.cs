using System;
using _5unSystem.Model.Entities;
using _5unSystem.Model.Enum;
using _5unSystem.Model.Shared;
using Microsoft.Extensions.Configuration;

namespace _5unSystem.Core.DataAccess;

public class AuthDataAccess
{
    public static User Login(User data)
    {
        using (var db = new DataContext())
        {
            var user = db.Users.FirstOrDefault(x => x.UserName == data.UserName);
            if (user == null)
                throw new Exception(ResponseLoginMessage.USER_NOT_FOUND);

            if (user.Password != data.Password)
                throw new Exception(ResponseLoginMessage.INVALID_CREDENTIALS);

            return user;
        }
    }

}
