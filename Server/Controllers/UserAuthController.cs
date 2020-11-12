using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace Controllers {
    [ApiController]
    [Route("user/auth")]
    public class UserAuthController : ControllerBase {
        private readonly IMapper _mapper;
        private readonly IUserAuthService _userAuthService;
        public UserAuthController(IUserAuthService userAuthService) {
            _userAuthService = userAuthService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync ([FromForm]Users model) {
            if (ModelState.IsValid) {
                var result = await _userAuthService.RegisterUserAsync(model);

                if(result.IsSuccess) {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid"); //Status code 400
        }
        [HttpPost("Signin")]
        public async Task<IActionResult> LoginAsync ([FromBody] LoginModel user) {
            if (ModelState.IsValid) {
                var result = await _userAuthService.LoginUserAsync(user);

                if(result.IsSuccess) {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }
    }
}