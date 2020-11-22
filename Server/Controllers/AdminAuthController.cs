using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace Controllers {
    [ApiController]
    [Route("admin/auth")]
    public class AdminAuthController : ControllerBase {
        private readonly IMapper _mapper;
        private readonly IAdminAuthService _adminAuthService;
        public AdminAuthController(IAdminAuthService adminAuthService) {
            _adminAuthService = adminAuthService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync ([FromForm]Users model) {
            if (ModelState.IsValid) {
                var result = await _adminAuthService.RegisterUserAsync(model);

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
                var result = await _adminAuthService.LoginUserAsync(user);

                if(result.IsSuccess) {
                    return Ok(result);
                }

                return Ok(result);
            }

            return BadRequest("Some properties are not valid");
        }
    }
}