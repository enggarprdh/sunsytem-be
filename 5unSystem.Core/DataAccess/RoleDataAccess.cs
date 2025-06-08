using _5unSystem.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _5unSystem.Core.DataAccess
{
    public class RoleDataAccess
    {
        public static List<RoleResponse> GetRoles(int skip, int take, out int totalData)
        {
            var response = new List<RoleResponse>();
            try
            {
                using (var db = new DataContext())
                {
                    response = db.Role
                                .Where(r => !r.Deleted)
                                .OrderBy(r => r.RoleName)
                                .Skip(skip)
                                .Take(take)
                                .Select(role => new RoleResponse
                                {
                                    RoleID = role.RoleID,
                                    RoleName = role.RoleName ?? string.Empty
                                })
                                .ToList();

                    totalData = db.Role.Count(r => !r.Deleted);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return response;
        }

        public static Boolean DeleteRole(Guid roleID)
        {
            try
            {
                using (var db = new DataContext())
                {
                    var role = db.Role.FirstOrDefault(r => r.RoleID == roleID);
                    if (role == null)
                        throw new Exception("Role not found");
                    role.Deleted = true;
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
