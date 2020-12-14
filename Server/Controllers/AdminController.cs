using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace Controllers {
    [ApiController]
    [Route("admin/controls")]
    public class AdminController : ControllerBase {
        private readonly IMapper _mapper;
        private readonly IAdminService _adminService;
        private UserManager<IdentityUser> _userManager;

        public AdminController(IAdminService adminService, UserManager<IdentityUser> userManager) {
            _adminService = adminService;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost("create/competition/fixture")]
        public async Task<IActionResult> CreateCompetitionFixture ([FromBody] Fixture[] fixtures) {
            if (ModelState.IsValid) {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var email = user?.Email;
                var result = await _adminService.CreateCompetitionFixtures(fixtures, email);
                
                if(result.IsSuccess) {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }

        [Authorize]
        [HttpPost("create/competition/")]
        public async Task<IActionResult>  CreateCompetition([FromForm] Competition competition) {
            if (ModelState.IsValid) {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var email = user?.Email;
                var result = await _adminService.CreateCompetition(competition, email, Request);
                return Ok(result);
            }
            return BadRequest("Some properties are not valid");
        }

        [Authorize]
        [HttpGet("golive/{teamId}")]
        public async Task<IActionResult> GoLive(int teamId) {
            if (ModelState.IsValid) {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var email = user?.Email;

                var result = await _adminService.GoLive(teamId, email, Request);
                if(result.IsSuccess) {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }

        [Authorize]
        [HttpPost("create/team")]
        public async Task<IActionResult> CreateTeamAsync ([FromForm] Team team) {
            if (ModelState.IsValid) {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var email = user?.Email;

                var result = await _adminService.CreateTeamAsync(team, email);
                if(result.IsSuccess) {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }

        [Authorize]
        [HttpPost("create/team/member")]
        public async Task<IActionResult> CreateTeamMemberAsync ([FromForm] Players players) {
            if (ModelState.IsValid) {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var email = user?.Email;

                var result = await _adminService.CreateTeamMemberAsync(players, email);
                if(result.IsSuccess) {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }

        [Authorize]
        [HttpPost("create/sport")]
        public async Task<IActionResult> CreateSportAsync ([FromForm] Sport sport) {
            if (ModelState.IsValid) {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var email = user?.Email;

                var result = await _adminService.CreateSportAsync(sport, 
                email);
                
                if(result.IsSuccess) {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }

    }
}