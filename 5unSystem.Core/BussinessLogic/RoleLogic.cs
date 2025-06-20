using _5unSystem.Core.DataAccess;
using _5unSystem.Model.DTO;
using _5unSystem.Model.Entities;
using _5unSystem.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _5unSystem.Core.BussinessLogic
{
    public class RoleLogic
    {
        public static List<RoleResponse> GetRoles(int skip, int take, out int totalData)
        {
            var response = new List<RoleResponse>();
            try
            {
                response = RoleDataAccess.GetRoles(skip, take, out totalData);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return response;
        }

        public static Boolean AddRole(RoleRequest input)
        {
            var response = false;
            try
            {
                if (input == null)
                    throw new ArgumentNullException(RoleResponseMessage.ROLE_IS_NULL);

                if (string.IsNullOrWhiteSpace(input.RoleName))
                    throw new ArgumentException(RoleResponseMessage.ROLE_NAME_EMPTY);
                
                var existingRole = RoleDataAccess.FindRoleByName(input.RoleName.Trim());
                if (existingRole)
                    throw new ArgumentException(RoleResponseMessage.ROLE_ALREADY_EXISTS);

                var role = new Role
                {
                    RoleID = Guid.NewGuid(),
                    RoleName = input.RoleName.Trim(),
                    CreatedAt = DateTime.Now,
                    Deleted = false
                };

                response = RoleDataAccess.AddRole(role);
                return true;
            }
            catch (ArgumentNullException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (ArgumentException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static Boolean DeleteRole(Guid roleID)
        {
            try
            {
                return RoleDataAccess.DeleteRole(roleID);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
