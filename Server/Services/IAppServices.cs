using System;
using System.Collections;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models;
using Newtonsoft.Json;

namespace Services
{
    public interface IAppService {
        Task<UserManagerResponse> GetAllTeamAsync(HttpRequest request);
        Task<UserManagerResponse> GetTeamBySportAsync(HttpRequest request, int id);
        Task<UserManagerResponse> GetTeamAsync(HttpRequest request, int id);

        Task<UserManagerResponse> GetAllPlayerAsync(HttpRequest request);
        Task<UserManagerResponse> GetPlayerAsync(HttpRequest request, int id);
        Task<UserManagerResponse> GetAllSportAsync(HttpRequest request);
        Task<UserManagerResponse> GetSportAsync(HttpRequest request, int id);

        Task<UserManagerResponse> GetAllNotificationOfUser(string email);
        Task<UserManagerResponse> CreateUserNotification(NotificationModel notification, string email);

        Task<UserManagerResponse> FollowClubInterest(int[] club, string email);

        Task<UserManagerResponse> GetAllCompetition(HttpRequest request, int sport);
        Task<UserManagerResponse> GetCompetition(HttpRequest request, int id);

        Task<UserManagerResponse> GetFixtureBasedOnCompetition(int competitionId,  HttpRequest request);
        Task<UserManagerResponse> GetFixtureBasedOnDate(int competitionId,  HttpRequest request);
        Task<UserManagerResponse> GetFixtureBasedOnTeams(int teamId, HttpRequest request);

    }

    public class AppierService : IAppService
    {
        private UserManager<IdentityUser> _userManager;
        private Server _dbContext;
        private IConfiguration _configuration;
        private IMailService _mailService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;

        public AppierService(
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

        public async Task<UserManagerResponse> FollowClubInterest(int[] clubs, string email) {
            var isUser = _dbContext.Users.FirstOrDefault(e => e.Email.ToLower() == email.ToLower());

            if(isUser == null) {
                return new UserManagerResponse {
                    Message = "Not a User",
                    IsSuccess = false
                };
            }

            foreach(var club in clubs) {
                var isTeam = _dbContext.Teams.FirstOrDefault(e => e.Id == club);

                if(isTeam != null) {
                    _dbContext.Follows.Add(new Follow() {
                        UserId = isUser.Id,
                        TeamId = isTeam.Id
                    });
                }
            }

            await _dbContext.SaveChangesAsync();

            return  new UserManagerResponse {
                Message = "Followed teams",
                IsSuccess = true
            };
        }

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

        public async Task<UserManagerResponse> GetAllNotificationOfUser(string email) {
             var isUser = await _dbContext.Users.FirstOrDefaultAsync(e => e.Email.ToLower() == email.ToLower());

            if(isUser == null) {
                return new UserManagerResponse {
                    Message = "Not a User",
                    IsSuccess = false
                };
            }

            var data = _dbContext.NotificationModel.Where(x => x.UserId == isUser.Id);

            if(data == null) {
                return new UserManagerResponse{
                    Message = "Does not exist",
                    IsSuccess = false,
                    Data = new int[0]
                };
            }

            return new UserManagerResponse{
                Message = "Notifications",
                IsSuccess = false,
                Data = _mapper.Map<IEnumerable<NotificationModelDto>>(data)
            };

        }

        public async Task<UserManagerResponse> GetAllTeamAsync(HttpRequest request)
        {
            var data = await  _dbContext.Teams.Select(x => new Team() {
                Id = x.Id,
                Name = x.Name,
                SportId = x.SportId,
                ImageName = String.Format("{0}://{1}{2}/Images/Team/{3}", request.Scheme, request.Host, request.PathBase,x.ImageName)
            })
            .ToListAsync();
            
            var x  = new UserManagerResponse{
                Message = "All Teams",
                IsSuccess = true,
                Data = _mapper.Map<IEnumerable<TeamDto>>(data)
            };

            return x;
        }

        public async Task<UserManagerResponse> GetTeamBySportAsync(HttpRequest request, int id)
        {
            var data = await  _dbContext.Teams.Where(x => x.SportId == id).Select(x => new Team() {
                Id = x.Id,
                Name = x.Name,
                SportId = x.SportId,
                ImageName = String.Format("{0}://{1}{2}/Images/Team/{3}", request.Scheme, request.Host, request.PathBase,x.ImageName)
            })
            .ToListAsync();
            
            var x  = new UserManagerResponse{
                Message = "All Teams",
                IsSuccess = true,
                Data = _mapper.Map<IEnumerable<TeamDto>>(data)
            };

            return x;
        }

        public async Task<UserManagerResponse> GetTeamAsync(HttpRequest request, int id)
        {
            var data = await _dbContext.Teams.FirstOrDefaultAsync<Team>(x => x.Id == id);

            if(data == null) {
                return new UserManagerResponse{
                    Message = "Does not exist",
                    IsSuccess = false,
                };
            }

            data.ImageName = String.Format("{0}://{1}{2}/Images/Team/{3}", request.Scheme, request.Host, request.PathBase,data.ImageName);

            return new UserManagerResponse{
                Message = "Does not exist",
                IsSuccess = false,
                Data = _mapper.Map<TeamDto>(data)
            };
        }

        public async Task<UserManagerResponse> GetAllPlayerAsync(HttpRequest request)
        {
            var data = await  _dbContext.Players.Select(x => new Players() {
                Id = x.Id,
                Name = x.Name,
                TeamId = x.TeamId,
                ImageName = String.Format("{0}://{1}{2}/Images/Team/{3}", request.Scheme, request.Host, request.PathBase,x.ImageName)
            })
            .ToListAsync();
            
            var x  = new UserManagerResponse{
                Message = "All Players",
                IsSuccess = true,
                Data = _mapper.Map<IEnumerable<PlayersDto>>(data)
            };

            return x;
        }

        public async Task<UserManagerResponse> GetPlayerAsync(HttpRequest request, int id)
        {
            var data = await _dbContext.Players.FirstOrDefaultAsync<Players>(x => x.Id == id);

            if(data == null) {
                return new UserManagerResponse{
                    Message = "Does not exist",
                    IsSuccess = false,
                };
            }

            data.ImageName = String.Format("{0}://{1}{2}/Images/Team/{3}", request.Scheme, request.Host, request.PathBase,data.ImageName);

            return new UserManagerResponse{
                Message = "Does not exist",
                IsSuccess = false,
                Data = _mapper.Map<PlayersDto>(data)
            };
        }

        public async Task<UserManagerResponse> GetAllSportAsync(HttpRequest request)
        {
            var data = await  _dbContext.Sports.Select(x => new Sport() {
                Id = x.Id,
                Name = x.Name,
                ImageName = String.Format("{0}://{1}{2}/Images/Sport/{3}", request.Scheme, request.Host, request.PathBase,x.ImageName)
            })
            .ToListAsync();
            
            var x  = new UserManagerResponse{
                Message = "All Sports",
                IsSuccess = true,
                Data = _mapper.Map<IEnumerable<SportDto>>(data)
            };

            return x;
        }

        public async Task<UserManagerResponse> GetSportAsync(HttpRequest request, int id)
        {
            var data = await _dbContext.Sports.FirstOrDefaultAsync<Sport>(x => x.Id == id);

            if(data == null) {
                return new UserManagerResponse{
                    Message = "Does not exist",
                    IsSuccess = false,
                };
            }

            data.ImageName = String.Format("{0}://{1}{2}/Images/Team/{3}", request.Scheme, request.Host, request.PathBase,data.ImageName);

            return new UserManagerResponse{
                Message = "Done",
                IsSuccess = true,
                Data = _mapper.Map<SportDto>(data)
            };
        }

        public async Task<UserManagerResponse> GetAllCompetition(HttpRequest request, int sportId)
        {
            var sport = await _dbContext.Sports.FirstOrDefaultAsync(x => x.Id == sportId);
            if(sport == null) {
                return new UserManagerResponse{
                    Message = "Does not exist",
                    IsSuccess = false,
                };
            }

            var data = _dbContext.Competition.Where(x => x.SportId == sport.Id).Select(x => new CompetitionDto() {
                Id = x.Id,
                Name = x.Name,
                Current = x.Current,
                Season = x.Season,
                SportId = x.SportId,
                ImageName = String.Format("{0}://{1}{2}/Images/Competition/Image/{3}", request.Scheme, request.Host, request.PathBase,x.ImageName),
                CoverImage = String.Format("{0}://{1}{2}/Images/Competition/Cover/{3}", request.Scheme, request.Host, request.PathBase,x.CoverImage)
            });

            if(data == null) {
                return new UserManagerResponse{
                    Message = $"{sport.Name} Competitions",
                    IsSuccess = true,
                    Data = new Competition [0]
                };
            }

            return new UserManagerResponse{
                Message = $"{sport.Name} Competitions",
                IsSuccess = true,
                Data = data
            };
        }

        public async Task<UserManagerResponse> GetCompetition(HttpRequest request, int id)
        {
            var data = await _dbContext.Competition.FirstOrDefaultAsync<Competition>(x => x.Id == id);

            if(data == null) {
                return new UserManagerResponse{
                    Message = "Does not exist",
                    IsSuccess = false,
                };
            }

            data.ImageName = String.Format("{0}://{1}{2}/Images/Competition/Image/{3}", request.Scheme, request.Host, request.PathBase,data.ImageName);
            data.CoverImage = String.Format("{0}://{1}{2}/Images/Competition/Cover/{3}", request.Scheme, request.Host, request.PathBase,data.CoverImage);

            return new UserManagerResponse{
                Message = "Done",
                IsSuccess = true,
                Data = _mapper.Map<CompetitionDto>(data)
            };
        }

        public async Task<UserManagerResponse> GetFixtureBasedOnCompetition(int competitionId, HttpRequest request)
        {
            var isCompetition = await _dbContext.Competition.FirstOrDefaultAsync(x => x.Id == competitionId);

            if (isCompetition == null) {
                return new UserManagerResponse{
                    Message = "Does not exist",
                    IsSuccess = false,
                };
            }

            var data = _dbContext.Fixtures.Where(x => x.CompetitionId == isCompetition.Id).Select(x => new Fixture {
                Competition = isCompetition,
                CompetitionId = x.CompetitionId,
                EventTime = x.EventTime,
                Id = x.Id,
                Label = x.Label,
                Team1 = teamData(x.Team1Id, request),
                Team2 = teamData(x.Team2Id, request),
                Team1Id = x.Team1Id,
                Team2Id = x.Team2Id,
                Team1Score = x.Team1Score,
                Team2Score = x.Team2Score
            });

            if(data == null) {
                return new UserManagerResponse{
                    Message = $"{isCompetition.Name} {isCompetition.Season} Competitions",
                    IsSuccess = true,
                    Data = new Fixture [0]
                };
            }

            return new UserManagerResponse{
                Message = $"{isCompetition.Name} {isCompetition.Season} Competitions",
                IsSuccess = true,
                Data = data
            };


        }

        public async Task<UserManagerResponse> GetFixtureBasedOnDate(int competitionId, HttpRequest request)
        {
            var isCompetition = await _dbContext.Competition.FirstOrDefaultAsync(x => x.Id == competitionId);

            if (isCompetition == null) {
                return new UserManagerResponse{
                    Message = "Does not exist",
                    IsSuccess = false,
                };
            }

            var data = _dbContext.Fixtures.Where(x => x.EventTime.Date.CompareTo(DateTime.Now.Date) == 0 && x.Id == isCompetition.Id).Select(x => new Fixture {
                Competition = isCompetition,
                CompetitionId = x.CompetitionId,
                EventTime = x.EventTime,
                Id = x.Id,
                Label = x.Label,
                Team1 = teamData(x.Team1Id, request),
                Team2 = teamData(x.Team2Id, request),
                Team1Id = x.Team1Id,
                Team2Id = x.Team2Id,
                Team1Score = x.Team1Score,
                Team2Score = x.Team2Score
            });

            if(data == null) {
                return new UserManagerResponse{
                    Message = $"{isCompetition.Name} {isCompetition.Season} Competitions",
                    IsSuccess = true,
                    Data = new Fixture [0]
                };
            }

            return new UserManagerResponse{
                Message = $"{isCompetition.Name} {isCompetition.Season} Competitions",
                IsSuccess = true,
                Data = data
            };
            
        }

        private Team teamData (int teamId,  HttpRequest request){
            var team = _dbContext.Teams.FirstOrDefault(y => y.Id == teamId);
            if (team != null)
                return new Team{
                    Id = team.Id,
                    ImageName = String.Format("{0}://{1}{2}/Images/Team/{3}", request.Scheme, request.Host, request.PathBase, team.ImageName),
                    Name = team.Name,
                    SportId = team.SportId
                };
            return null;
        }

        public async Task<UserManagerResponse> GetFixtureBasedOnTeams(int teamId, HttpRequest request)
        {
            var isTeam = await _dbContext.Teams.FirstOrDefaultAsync(x => x.Id == teamId);

            if (isTeam == null) {
                return new UserManagerResponse{
                    Message = "Does not exist",
                    IsSuccess = false,
                };
            }

            var data = _dbContext.Fixtures.Where(x => x.Team1Id == isTeam.Id || x.Team2Id == isTeam.Id).Select(x => new Fixture {
                Competition = _dbContext.Competition.FirstOrDefault(y => y.Id == x.CompetitionId),
                CompetitionId = x.CompetitionId,
                EventTime = x.EventTime,
                Id = x.Id,
                Label = x.Label,
                Team1 = teamData(x.Team1Id, request),
                Team2 = teamData(x.Team2Id, request),
                Team1Id = x.Team1Id,
                Team2Id = x.Team2Id,
                Team1Score = x.Team1Score,
                Team2Score = x.Team2Score
            });

            if(data == null) {
                return new UserManagerResponse{
                    Message = $"{isTeam.Name} fixtures",
                    IsSuccess = true,
                    Data = new Fixture [0]
                };
            }

            return new UserManagerResponse{
                Message = $"{isTeam.Name} fixtures",
                IsSuccess = true,
                Data = data
            };
            
        }
    }
}