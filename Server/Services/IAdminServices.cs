using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Context;
using Dtos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models;

namespace Services
{
    public interface IAdminService {
        Task<UserManagerResponse> CreateTeamAsync(Team model);
        Task<UserManagerResponse> CreateSportAsync(Sport model);
        Task<UserManagerResponse> GetAllTeamAsync();
        Task<UserManagerResponse> GetTeamAsync(int id);
        Task<UserManagerResponse> GetAllSportAsync();
        Task<UserManagerResponse> GetSportAsync(int id);
        // Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token);
        // Task<UserManagerResponse> ForgotPasswordAsync(string email);
        // Task<UserManagerResponse> ResetPasswordAsync(ResetPassword model);
    }

    public class AdminService : IAdminService
    {
        private UserManager<IdentityUser> _userManager;
        private Server _dbContext;
        private IConfiguration _configuration;
        private IMailService _mailService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;

        public AdminService(
            IWebHostEnvironment hostEnvironment, 
            Server dbContext, 
            UserManager<IdentityUser> userManager, 
            IConfiguration configuration, 
            IMailService mailService,
            IMapper mapper
        )
        {
            _userManager = userManager;
            _configuration = configuration;
            _mailService = mailService;
            _dbContext = dbContext;
            _hostEnvironment = hostEnvironment;
            _mapper = mapper;
        }

        [NonAction]
        public async Task<string> SaveImage(IFormFile imageFile) {
            string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ', '-');
            imageName = imageName+DateTime.Now.ToString("yymmssfff") + Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images\\Team", imageName);
            using (var fileStream = new FileStream(imagePath, FileMode.Create)) {
                await imageFile.CopyToAsync(fileStream);
            }

            return imageName;
        }

        [NonAction]
        public bool CheckIfAdmin(string email) {
            var isAdmin = _dbContext.Users.FirstOrDefault(e => e.Email.ToLower() == email.ToLower());

            if(isAdmin == null) 
                return false;

            return isAdmin.UserType == "Admin";
        }

        public Task<UserManagerResponse> CreateTeamAsync(Team model)
        {
            throw new NotImplementedException();
        }

        public Task<UserManagerResponse> CreateSportAsync(Sport model)
        {
            throw new NotImplementedException();
        }

        public Task<UserManagerResponse> GetAllTeamAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserManagerResponse> GetTeamAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<UserManagerResponse> GetAllSportAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserManagerResponse> GetSportAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}