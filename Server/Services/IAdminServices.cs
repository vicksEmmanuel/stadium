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
        Task<UserManagerResponse> CreateTeamAsync(Team model, string email);
        Task<UserManagerResponse> CreateSportAsync(Sport model, string email);
        Task<UserManagerResponse> CreateTeamMemberAsync(Players model, string email);
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
        public async Task<string> SaveImage(IFormFile imageFile, string spec) {
            string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ', '-');
            imageName = imageName+DateTime.Now.ToString("yymmssfff") + Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images\\"+spec, imageName);
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

        public async Task<UserManagerResponse> CreateTeamAsync(Team model, string email)
        {
            if(!CheckIfAdmin(email)) {
                return new UserManagerResponse {
                    Message = "Not an Admin",
                    IsSuccess = false
                };
            }

            if (model.ImageFile != null) {
                model.ImageName = await SaveImage(model.ImageFile, "Team");
                _dbContext.Teams.Add(model);
                await _dbContext.SaveChangesAsync();
                return new UserManagerResponse {
                    Message = "Team created",
                    IsSuccess = true
                };
            } else {
                return new UserManagerResponse {
                    Message = "Add an Image",
                    IsSuccess = false
                };
            }
        }

        public async Task<UserManagerResponse> CreateSportAsync(Sport model, string email)
        {
             if(!CheckIfAdmin(email)) {
                return new UserManagerResponse {
                    Message = "Not an Admin",
                    IsSuccess = false
                };
            }

            if (model.ImageFile != null) {
                model.ImageName = await SaveImage(model.ImageFile,"Sport");
                _dbContext.Sports.Add(model);
                await _dbContext.SaveChangesAsync();
                return new UserManagerResponse {
                    Message = "Sport created",
                    IsSuccess = true
                };
            } else {
                return new UserManagerResponse {
                    Message = "Add an Image",
                    IsSuccess = false
                };
            }
        }

        public async Task<UserManagerResponse> CreateTeamMemberAsync(Players model, string email)
        {
            if(!CheckIfAdmin(email)) {
                return new UserManagerResponse {
                    Message = "Not an Admin",
                    IsSuccess = false
                };
            }

            if (model.ImageFile != null) {
                model.ImageName = await SaveImage(model.ImageFile, "Members");
                _dbContext.Players.Add(model);
                await _dbContext.SaveChangesAsync();
                return new UserManagerResponse {
                    Message = "Member created",
                    IsSuccess = true
                };
            } else {
                return new UserManagerResponse {
                    Message = "Add an Image",
                    IsSuccess = false
                };
            }
        }
    }
}