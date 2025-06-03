using System;
using System.Collections.Generic;
using System.Data;
using _5unSystem.Model.DTO;
using _5unSystem.Model.Entities;
using _5unSystem.Model.Enum;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;
namespace _5unSystem.Core.DataAccess;

public class AuthDataAccess
{
    public static LoginResponse Login(User data, out Guid roleID)
    {
        using (var db = new DataContext())
        {
            var sql = $@"SELECT U.{User.UserNameField}
                        , U.{User.PasswordField}
                        , R.{Role.RoleNameField}
                        , R.{Role.RoleIDField}
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

                roleID = Guid.Parse(row[Role.RoleIDField].ToString());
                loginresponse.UserName = row[User.UserNameField].ToString();
                loginresponse.Role = row[Role.RoleNameField].ToString();
            } else
            {
                roleID = Guid.Empty;
                loginresponse.UserName = string.Empty;
                loginresponse.Role = string.Empty;
            }
            return loginresponse;
        }
    }

    public static List<MenuResponse> GetMenu(Guid roleID)
    {
        var result = new List<MenuResponse>();
        using (var db = new DataContext())
        {
            var queryMenu = $@"SELECT M.{Menu.MenuIDField}
                        , M.{Menu.DisplayNameField}
                        , M.{Menu.MenuIconField}
                        , M.{Menu.PathField}
                        , M.{Menu.ParentMenuIDField}
                        , M.{Menu.IsHasSubMenuField}
                        , RM.{RoleMenu.IsViewField}
                        , RM.{RoleMenu.IsAddField}
                        , RM.{RoleMenu.IsEditField}
                        , RM.{RoleMenu.IsDeleteField}
                        , M.{Menu.SequenceField}
                        FROM {Menu.TableName} M
                        INNER JOIN {RoleMenu.TableName} RM ON M.{Menu.MenuIDField} = RM.{RoleMenu.MenuIDField}
                        WHERE RM.{RoleMenu.RoleIDField} = @RoleID
                        AND M.{Menu.DeletedField} = 0";

            var commandMenu = db.Database.GetDbConnection().CreateCommand();
            commandMenu.CommandText = queryMenu;
            commandMenu.Parameters.Add(new SqlParameter
            {
                ParameterName = "@RoleID",
                Value = roleID
            });
            var dtMenu = new System.Data.DataTable();
            using (var adapter = new SqlDataAdapter((SqlCommand)commandMenu))
            {
                adapter.Fill(dtMenu);
            }


            if (dtMenu.Rows.Count > 0)
            {
                foreach (DataRow row in dtMenu.Rows)
                {
                    var response = new MenuResponse();

                    if (!string.IsNullOrEmpty(row[Menu.ParentMenuIDField].ToString()))
                        continue;

                    response.MenuID = row[Menu.MenuIDField].ToString();
                    response.DisplayName = row[Menu.DisplayNameField].ToString();
                    response.Icon = row[Menu.MenuIconField].ToString();
                    response.Path = row[Menu.PathField].ToString();
                    response.Sequence = row[Menu.SequenceField] != DBNull.Value ? Convert.ToInt32(row[Menu.SequenceField]) : 0;
                    response.IsView = Convert.ToBoolean(row[RoleMenu.IsViewField]?.ToString());
                    response.IsAdd = Convert.ToBoolean(row[RoleMenu.IsAddField]?.ToString());
                    response.IsEdit = Convert.ToBoolean(row[RoleMenu.IsEditField]?.ToString());
                    response.IsDelete = Convert.ToBoolean(row[RoleMenu.IsDeleteField]?.ToString());
                    response.IsHasSubMenu = Convert.ToBoolean(row[Menu.IsHasSubMenuField]);
                    
                    if (response.IsHasSubMenu)
                    {
                        var subMenuObj = dtMenu.Select($"{Menu.ParentMenuIDField} = '{response.MenuID}'");
                        foreach (DataRow subMenuRow in subMenuObj)
                        {
                            var subMenuResponse = new SubMenu();
                            subMenuResponse.MenuID = subMenuRow[Menu.MenuIDField].ToString();
                            subMenuResponse.DisplayName = subMenuRow[Menu.DisplayNameField].ToString();
                            subMenuResponse.Sequence = subMenuRow[Menu.SequenceField] != DBNull.Value ? Convert.ToInt32(subMenuRow[Menu.SequenceField]) : 0;
                            subMenuResponse.Path = subMenuRow[Menu.PathField].ToString();
                            subMenuResponse.IsView = Convert.ToBoolean(subMenuRow[RoleMenu.IsViewField]?.ToString());
                            subMenuResponse.IsAdd = Convert.ToBoolean(subMenuRow[RoleMenu.IsAddField]?.ToString());
                            subMenuResponse.IsEdit = Convert.ToBoolean(subMenuRow[RoleMenu.IsEditField]?.ToString());
                            subMenuResponse.IsDelete = Convert.ToBoolean(subMenuRow[RoleMenu.IsDeleteField]?.ToString());
                            if (response.SubMenu == null)
                            {
                                response.SubMenu = new List<SubMenu>();
                            }
                            response.SubMenu.Add(subMenuResponse);
                        }
                    }
                    result.Add(response);
                }
            }
        }

        if (result.Count > 0)
        {
            result = result.OrderBy(x => x.Sequence).ToList();
            foreach (var menu in result)
            {
                if (menu.SubMenu != null && menu.SubMenu.Count > 0)
                    menu.SubMenu = menu.SubMenu.OrderBy(x => x.Sequence).ToList();
                
            }

        }

        return result;
    }
}
