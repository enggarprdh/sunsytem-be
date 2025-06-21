using _5unSystem.Core.DataAccess;
using _5unSystem.Model.DTO;
using _5unSystem.Model.Entities;
using _5unSystem.Model.Enum;
using _5unSystem.Utility;
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

        public static RoleResponse GetRoleById(Guid roleId)
        {
            try
            {
                if (roleId == Guid.Empty)
                    throw new ArgumentException(RoleResponseMessage.ROLE_ID_EMPTY);
                var role = RoleDataAccess.FindRoleById(roleId);
                if (role == null)
                    throw new Exception(RoleResponseMessage.ROLE_NOT_FOUND);
                return new RoleResponse
                {
                    RoleID = role.RoleID,
                    RoleName = role.RoleName ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static Boolean AddRole(RoleCreateOrUpdateRequest input)
        {
            var response = false;
            try
            {
                if (input == null)
                    throw new Exception(RoleResponseMessage.ROLE_IS_NULL);

                if (string.IsNullOrWhiteSpace(input.RoleName))
                    throw new Exception(RoleResponseMessage.ROLE_NAME_EMPTY);

                var existingRole = RoleDataAccess.FindRoleByName(input.RoleName.Trim());
                if (existingRole)
                    throw new Exception(RoleResponseMessage.ROLE_ALREADY_EXISTS);

                var role = new Role
                {
                    RoleID = Guid.NewGuid(),
                    RoleName = input.RoleName.Trim(),
                    CreatedAt = DateTime.Now,
                    CreatedBy = UserClaimHelper.UserName(),
                    Deleted = false
                };

                response = RoleDataAccess.AddRole(role);
                return true;
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

        public static void UpdateRole(Guid roleId,RoleCreateOrUpdateRequest input)
        {
            try
            {
                if (input == null)
                    throw new Exception(RoleResponseMessage.ROLE_IS_NULL);

                if (string.IsNullOrWhiteSpace(input.RoleName))
                    throw new Exception(RoleResponseMessage.ROLE_NAME_EMPTY);

                var existingRole = RoleDataAccess.FindRoleById(roleId);

                existingRole.RoleName = input.RoleName.Trim();
                existingRole.ModifiedBy = string.Empty; // Assuming you want to set it to a default value, otherwise set it to the current user's ID


                var role = new Role
                {
                    RoleID = Guid.NewGuid(), // Assuming you want to create a new role, otherwise fetch the existing one
                    RoleName = input.RoleName.Trim(),
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = UserClaimHelper.UserName(), // Assuming you want to set the modified by to the current user's name
                    Deleted = false
                };

                var isUpdated = RoleDataAccess.UpdateRole(role);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }   
        }
    }
}
