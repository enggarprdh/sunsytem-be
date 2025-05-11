
using Microsoft.AspNetCore.Mvc;
using _5unSystem.Model.Shared;
using _5unSystem.Model.DTO;
using _5unSystem.Core.BussinessLogic;


namespace _5unSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public ActionResult Login(LoginRequest input)
        {
            try
            {
                var response = AuthLogic.AuthProcess(input.UserName, input.Password);
                var result = new Result<LoginResponse>
                {
                    Data = response
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = new Result<LoginResponse>{
                    Message = ex.Message
                };
                return BadRequest(result);
            }

        }
    }
}

