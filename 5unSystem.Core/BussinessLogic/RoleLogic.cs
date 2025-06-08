using _5unSystem.Core.DataAccess;
using _5unSystem.Model.DTO;
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
