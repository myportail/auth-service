using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace authService.Controllers
{
    [Route("api/users")]
    public class UsersController : Controller
    {
        private Services.IUsersService UsersService { get; }

        public UsersController(Services.IUsersService usersService)
        {
            UsersService = usersService;
        }

        [Route("create")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateUser([FromBody] Model.Api.User user)
        {
            try
            {
                // test line to query claim data
                var userId = HttpContext.User.Claims.First(x => x.Type == "id")?.Value;
                
                var dbUser = await UsersService.AddUser(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            return StatusCode(StatusCodes.Status200OK);
        }

        [Route("")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ListUsers()
        {
            try
            {
                var users = await UsersService.listUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
        }
    }
}
