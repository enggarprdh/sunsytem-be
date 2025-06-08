using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using _5unSystem.Core.BussinessLogic;
using _5unSystem.Model.Shared;
using _5unSystem.Model.DTO;

namespace _5unSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public RoleController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public ActionResult GetRoles([FromQuery] int page = 1, int row = 10)
        {
            try
            {
                if (page < 1)
                    page = 1;

                var skip = (page - 1) * row;
                var totalData = 0;
                var response = RoleLogic.GetRoles(skip, row, out totalData);
                var result = new ResultList<RoleResponse>();
                result.Data = response;
                result.DataLength = totalData;
                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = new Result<List<string>>
                {
                    Message = ex.Message
                };
                return BadRequest(result);
            }
        }

        [HttpDelete("{roleID}")]
        public ActionResult DeleteRole(Guid roleID)
        {
            try
            {
                if (roleID == Guid.Empty)
                    throw new ArgumentException("Role ID cannot be empty");
                var isDeleted = RoleLogic.DeleteRole(roleID);
                if (!isDeleted)
                    throw new Exception("Failed to delete role");
                return Ok(new Result<string> { Message = "Role deleted successfully" });
            }
            catch (Exception ex)
            {
                var result = new Result<string>
                {
                    Message = ex.Message
                };
                return BadRequest(result);
            }



        }
    }
}
