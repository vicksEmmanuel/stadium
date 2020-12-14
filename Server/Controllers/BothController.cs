using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace Controllers {
    [ApiController]
    [Route("/")]
    public class BothController : ControllerBase {
        private readonly IMapper _mapper;
        private readonly IAppService _service;
        private UserManager<IdentityUser> _userManager;

        public BothController(IAppService service, UserManager<IdentityUser> userManager) {
            _service = service;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet("competition/{competitionId}")]
        public async Task<IActionResult> GetCompetitionAsync (int competitionId) {
             var result = await _service.GetCompetition(Request, competitionId);

            if(result.IsSuccess) {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [Authorize]
        [HttpGet("competition/fixtures/{id}")]
        public async Task<IActionResult> GetAllFixturesOfCompetition (int id, string fixture="all") {
            Dtos.UserManagerResponse result;

            if (fixture.ToLower() == "all") {
                result = await _service.GetFixtureBasedOnCompetition(id, Request);
            } else {
                result = await _service.GetFixtureBasedOnDate(id, Request);
            }
            
            if(result.IsSuccess) {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [Authorize]
        [HttpGet("competition/team/fixtures/{id}")]
        public async Task<IActionResult> GetAllFixturesOfTeam (int id) {
            var result = await _service.GetFixtureBasedOnTeams(id, Request);
            if(result.IsSuccess) {
                return Ok(result);
            }
            return BadRequest(result);
        }


        [Authorize]
        [HttpGet("notification")]
        public async Task<IActionResult> GetAllNotificationAsync () {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var email = user?.Email;
            
            var result = await _service.GetAllNotificationOfUser(email);

            if(result.IsSuccess) {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [Authorize]
        [HttpPost("notification")]
        public async Task<IActionResult> CreateUserNotification ([FromBody] NotificationModelDto notification) {
            if (ModelState.IsValid) {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var email = user?.Email;

                var notif = _mapper.Map<NotificationModel>(notification);

                notif.CreatedDate = DateTime.Now;
                notif.IsRecurring = false;
                notif.IsRead = false;

                var result = await _service.CreateUserNotification(notif, email);
                if(result.IsSuccess) {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }

        [Authorize]
        [HttpPost("follow")]
        public async Task<IActionResult> FollowClubInterest ([FromBody] FollowModelDto clubs ) {
            if (ModelState.IsValid) {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var email = user?.Email;

                var result = await _service.FollowClubInterest(clubs.TeamIds, email);
                if(result.IsSuccess) {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }

        [Authorize]
        [HttpGet("team")]
        public async Task<IActionResult> GetAllTeamAsync () {
            if (ModelState.IsValid) {
                var result = await _service.GetAllTeamAsync(Request);

                if(result.IsSuccess) {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }

        [Authorize]
        [HttpGet("team/sport/{id}")]
        public async Task<IActionResult> GetTeamBySportAsync (int id) {
            if (ModelState.IsValid) {
                var result = await _service.GetTeamBySportAsync(Request, id);

                if(result.IsSuccess) {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }

        [Authorize]
        [HttpGet("members")]
        public async Task<IActionResult> GetAllMembersAsync () {
            if (ModelState.IsValid) {
                var result = await _service.GetAllPlayerAsync(Request);

                if(result.IsSuccess) {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }

        [Authorize]
        [HttpGet("sport")]
        public async Task<IActionResult> GetAllSportAsync () {
            if (ModelState.IsValid) {
                var result = await _service.GetAllSportAsync(Request);

                if(result.IsSuccess) {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }

        [Authorize]
        [HttpGet("team/{id}")]
        public async Task<IActionResult> GetTeamAsync (int id) {
            if (ModelState.IsValid) {
                var result = await _service.GetTeamAsync(Request, id);

                if(result.IsSuccess) {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }

        [Authorize]
        [HttpGet("members/{id}")]
        public async Task<IActionResult> GetMembersAsync (int id) {
            if (ModelState.IsValid) {
                var result = await _service.GetPlayerAsync(Request, id);

                if(result.IsSuccess) {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }


        [Authorize]
        [HttpGet("sport/{id}")]
        public async Task<IActionResult> GetSportAsync (int id) {
            var result = await _service.GetSportAsync(Request, id);

            if(result.IsSuccess) {
                return Ok(result);
            }

            return BadRequest(result);
        }
        
        [Authorize]
        [HttpGet("sport/competition/{sportid}")]
        public async Task<IActionResult> GetAllCompetitionAsync (int sportid) {
             var result = await _service.GetAllCompetition(Request, sportid);

            if(result.IsSuccess) {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}