using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace Controllers {
    [ApiController]
    [Route("/")]
    public class BothController : ControllerBase {
        private readonly IMapper _mapper;
        private readonly IAppService _service;
        public BothController(IAppService service) {
            _service = service;
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
            if (ModelState.IsValid) {
                var result = await _service.GetSportAsync(Request, id);

                if(result.IsSuccess) {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }
    }
}