using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Context;
using Dtos;
using Hubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.Net.Http.Server;
using Models;
using Newtonsoft.Json;

namespace Services
{
    public interface IAdminService {
        Task<UserManagerResponse> CreateTeamAsync(Team model, string email);
        Task<UserManagerResponse> CreateSportAsync(Sport model, string email);
        Task<UserManagerResponse> CreateTeamMemberAsync(Players model, string email);
        Task<UserManagerResponse> GoLive(int teamId, string adminEmail, HttpRequest request);
        Task<UserManagerResponse> CreateCompetition(Competition model, string email, HttpRequest request);
        Task<UserManagerResponse> CreateCompetitionFixture(Fixture model);
        Task<UserManagerResponse> CreateCompetitionFixtures(Fixture[] model, string email);
    }

    public class AdminService : IAdminService
    {
        private UserManager<IdentityUser> _userManager;
        private Server _dbContext;
        private IConfiguration _configuration;
        private IMailService _mailService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;

        private readonly IHubContext<NotificationHub> _hubContext;

        public AdminService(
            IWebHostEnvironment hostEnvironment, 
            Server dbContext, 
            UserManager<IdentityUser> userManager, 
            IConfiguration configuration, 
            IMailService mailService,
            IMapper mapper,
            IHubContext<NotificationHub> hubContext
        )
        {
            _userManager = userManager;
            _configuration = configuration;
            _mailService = mailService;
            _dbContext = dbContext;
            _hostEnvironment = hostEnvironment;
            _mapper = mapper;
            _hubContext = hubContext;
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

        [NonAction]
         public async Task<UserManagerResponse> CreateUserNotification(NotificationModel model, string email) {
            var isUser = _dbContext.Users.FirstOrDefault(e => e.Email.ToLower() == email.ToLower());

            if(isUser == null) {
                return new UserManagerResponse {
                    Message = "Not a User",
                    IsSuccess = false
                };
            }

            model.UserId = isUser.Id;
            _dbContext.NotificationModel.Add(model);
            await _dbContext.SaveChangesAsync();
            return new UserManagerResponse {
                Message = "Notification created",
                IsSuccess = true,
                Data = _mapper.Map<NotificationModelDto>(model)
            };
        }

        public async Task<UserManagerResponse> GoLive(int teamId, string adminEmail, HttpRequest request) {
            var isAdmin = _dbContext.Users.FirstOrDefault(e => e.Email.ToLower() == adminEmail.ToLower());
            if(isAdmin == null) 
                return new UserManagerResponse {
                    Message = "Not an Admin",
                    IsSuccess = false
                };
            
            var team = _dbContext.Teams.FirstOrDefault(x => x.Id == teamId);
            if(team == null) {
                return new UserManagerResponse {
                    Message = "Team does not exist",
                    IsSuccess = false
                };
            }
            var usersWhoFollowsTeam = _dbContext.Follows.Where(x => x.TeamId == team.Id).ToArray();
            var sport = _dbContext.Sports.FirstOrDefault(x => x.Id == team.SportId);
            foreach(var user in usersWhoFollowsTeam) {
                var model = new NotificationModel() {
                    Message = $"{team.Name} is now live at {DateTime.Now.TimeOfDay.ToString()}",
                    Type = "GO_LIVE",
                    CreatedDate = DateTime.Now,
                    IsRead = false,
                    UserId = user.Id,
                    IsRecurring = false,
                    TeamId = teamId,
                    Data = new GoLiveDto() {
                        AdminTeamGroup = $"{sport.Name}_{team.Name}_admin",
                        UserChatTeamGroup = $"{sport.Name}_{team.Name}_users_chat",
                        UserTeamGroup = $"{sport.Name}_{team.Name}_users"
                    }
                };
                var message = JsonConvert.SerializeObject(model);
                var registeredNotification = _dbContext.NotificationHubModels
                        .FirstOrDefault(x => x.UserId == user.UserId);
                if (registeredNotification != null & registeredNotification.isOpen) {
                    await _hubContext.Clients
                    .Client(registeredNotification.ConnectionId)
                    .SendAsync("RecieveMessage", message);
                }
                await CreateUserNotification(model, _dbContext.Users.FirstOrDefault(x => x.Id == user.Id).Email);
            }

            return new UserManagerResponse {
                Message = "Gone live",
                IsSuccess = true,
                Data = new NotificationModel() {
                    Message = $"{team.Name} is now live at {DateTime.Now.TimeOfDay.ToString()}",
                    Type = "GO_LIVE",
                    CreatedDate = DateTime.Now,
                    IsRead = false,
                    UserId = isAdmin.Id,
                    IsRecurring = false,
                    TeamId = teamId,
                    Data = new GoLiveDto() {
                        AdminTeamGroup = $"{sport.Name}_{team.Name}_admin",
                        UserChatTeamGroup = $"{sport.Name}_{team.Name}_users_chat",
                        UserTeamGroup = $"{sport.Name}_{team.Name}_users"
                    }
                }
            };

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
                    IsSuccess = true,
                    Data = model
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

        public async Task<UserManagerResponse> CreateCompetition(Competition model, string email, HttpRequest request)
        {
            if(!CheckIfAdmin(email)) {
                return new UserManagerResponse {
                    Message = "Not an Admin",
                    IsSuccess = false
                };
            }
            
            if (model.ImageFile != null) {
                model.ImageName = await SaveImage(model.ImageFile, "Competition");
                _dbContext.Competition.Add(model);
                await _dbContext.SaveChangesAsync();
                model.ImageName = String.Format("{0}://{1}{2}/Images/Competition/{3}", request.Scheme, request.Host, request.PathBase,model.ImageName);

                return new UserManagerResponse {
                    Message = "Competition Created",
                    IsSuccess = true,
                    Data = model
                };
            } else {
                return new UserManagerResponse {
                    Message = "Add an Image and Cover Image",
                    IsSuccess = false
                };
            }
        }

        public async Task<UserManagerResponse> CreateCompetitionFixture(Fixture model)
        {
            var isCompetition = _dbContext.Competition.FirstOrDefault(e => e.Id == model.CompetitionId);
            var isTeam1 = _dbContext.Teams.FirstOrDefault(e => e.Id == model.Team1Id);
            var isTeam2 = _dbContext.Teams.FirstOrDefault(e => e.Id == model.Team2Id);
            if(isCompetition == null || isTeam1 == null || isTeam2 == null) 
                return new UserManagerResponse {
                    Message = $"Competition returned {isCompetition}, Team 1 returned {isTeam1}, Team 2 retured {isTeam2}",
                    IsSuccess = false,
                    Data= model
                };   

            _dbContext.Fixtures.Add(model);
            await _dbContext.SaveChangesAsync();

            return new UserManagerResponse {
                Message = "Fixture Added",
                IsSuccess = true,
                Data = model
            };
        }

        public async Task<UserManagerResponse> CreateCompetitionFixtures(Fixture [] model, string email) {
            if(!CheckIfAdmin(email)) {
                return new UserManagerResponse {
                    Message = "Not an Admin",
                    IsSuccess = false
                };
            }

            var data = new List<UserManagerResponse>();
            foreach(var fixture in model) {
                var result = await CreateCompetitionFixture(fixture);
                data.Add(result);
            }
            return new UserManagerResponse {
                Message = "Done.",
                IsSuccess = true,
                Data = data.ToArray()
            };
        }
    }
}