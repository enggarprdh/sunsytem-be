using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using _5unSystem.Core.BussinessLogic;
using _5unSystem.Model.Shared;
using _5unSystem.Model.DTO;
using Microsoft.AspNetCore.Authorization;
using _5unSystem.Model.Enum;

namespace _5unSystem.API.Controllers
{
    [Route("api/role")]
    [ApiController]
    [Authorize]
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

        [HttpGet("{roleId}")]
        public ActionResult GetRoleById(Guid roleId)
        {
            try
            {
                if (roleId == Guid.Empty)
                    throw new ArgumentException(RoleResponseMessage.ROLE_INVALID_DATA);
                var response = RoleLogic.GetRoleById(roleId);
                var result = new Result<RoleResponse>
                {
                    Data = response
                };

                return Ok(result);
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
        [HttpDelete("{roleId}")]
        public ActionResult DeleteRole(Guid roleId)
        {
            try
            {
                if (roleId == Guid.Empty)
                    throw new ArgumentException("Role ID cannot be empty");
                var isDeleted = RoleLogic.DeleteRole(roleId);
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
        [HttpPost]
        public ActionResult AddRole([FromBody] RoleCreateOrUpdateRequest input)
        {
            try
            {

                RoleLogic.AddRole(input);
                return Ok(new Result<string> { Message = RoleResponseMessage.ROLE_CREATED_SUCCESSFULLY });
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
        [HttpPut("{roleId}")]
        public ActionResult UpdateRole(Guid roleId, [FromBody] RoleCreateOrUpdateRequest input)
        {
            try
            {
                if (input == null || roleId == Guid.Empty)
                    throw new ArgumentException(RoleResponseMessage.ROLE_INVALID_DATA);

                RoleLogic.UpdateRole(roleId, input);

                return Ok(new Result<string> { Message = RoleResponseMessage.ROLE_UPDATED_SUCCESSFULLY });
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
