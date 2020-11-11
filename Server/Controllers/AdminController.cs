using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace Controllers {
    [ApiController]
    [Route("admin/controls")]
    public class AdminController : ControllerBase {
        private readonly IMapper _mapper;
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService) {
            _adminService = adminService;
        }

    }
}