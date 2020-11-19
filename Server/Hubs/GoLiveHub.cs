using System;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using AutoMapper.Configuration;
using Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Models;
using Dtos;
using Constantine;

namespace Hubs
{
    [Authorize]
    public class GoLiveHub : Hub {

        private UserManager<IdentityUser> _userManager;
        private Server _dbContext;
        private IMailService _mailService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;
        private readonly IHandlerServices _handlerServices;
        public GoLiveHub(
            IWebHostEnvironment hostEnvironment, 
            Server dbContext, 
            UserManager<IdentityUser> userManager,
            IMailService mailService,
            IMapper mapper,
            IHandlerServices handlerServices
        ) {
            _userManager = userManager;
            _mailService = mailService;
            _dbContext = dbContext;
            _hostEnvironment = hostEnvironment;
            _mapper = mapper;
            _handlerServices = handlerServices;
        }

        public async Task JoinGroup(string name, string connectionId) {
            var user = await _userManager.GetUserAsync(Context.User);
            var email = user?.Email;
            var userId =_dbContext.Users.FirstOrDefault(x => x.Email == email);

            var model = new JoinedLiveModel() {
                Message = $"{userId.Username} just joined at {DateTime.Now.TimeOfDay.ToString()}",
                Type = Types.JOINLIVE,
                JoinTime = DateTime.Now,
                GroupName = name,
                Data = _mapper.Map<UsersDto>(userId)
            };
            var message = JsonConvert.SerializeObject(model);

            await Groups.AddToGroupAsync(connectionId, name);
            await Clients.Group(name).SendAsync("JoinMessage", $"{message}");
        }

        public async Task UnjoinGroup(string name, string connectionId) {
            var user = await _userManager.GetUserAsync(Context.User);
            var email = user?.Email;
            var userId =_dbContext.Users.FirstOrDefault(x => x.Email == email);

            var model = new JoinedLiveModel() {
                Message = $"{userId.Username} just left at {DateTime.Now.TimeOfDay.ToString()}",
                Type = Types.LEFTLIVE,
                JoinTime = DateTime.Now,
                GroupName = name,
                Data = _mapper.Map<UsersDto>(userId)
            };
            var message = JsonConvert.SerializeObject(model);

            await Groups.RemoveFromGroupAsync(connectionId, name);
            await Clients.Group(name).SendAsync("LeftLiveMessage", $"{message}");
        }
        public override async Task OnConnectedAsync() {
            var user = await _userManager.GetUserAsync(Context.User);
            var email = user?.Email;
            var userId =_dbContext.Users.FirstOrDefault(x => x.Email == email).Id;

            var isExist = _dbContext.GoLiveHubModels.FirstOrDefault(x => x.UserEmail == email);

            if(isExist != null) {
                isExist.ConnectionId = Context.ConnectionId;
                isExist.isOpen = true;
                _dbContext.GoLiveHubModels.Update(isExist);
            } else {
                 _dbContext.GoLiveHubModels.Add(new Models.GoLiveHubModel() {
                    UserEmail = email,
                    UserId = userId,
                    isOpen = true,
                    ConnectionId = Context.ConnectionId
                });
            }

            await _dbContext.SaveChangesAsync();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception e) {
            Console.WriteLine(Context.ConnectionId);
            var user = await _userManager.GetUserAsync(Context.User);
            var email = user?.Email;
            var userId =_dbContext.Users.FirstOrDefault(x => x.Email == email).Id;
            var isExist = _dbContext.GoLiveHubModels.FirstOrDefault(x => x.UserEmail == email);

            if(isExist != null) {
                isExist.ConnectionId = Context.ConnectionId;
                isExist.isOpen = false;
                _dbContext.GoLiveHubModels.Update(isExist);
            }
            await _dbContext.SaveChangesAsync();
            await base.OnDisconnectedAsync(e);
        }

        public async Task SendMessageAsync(string message, GoLiveDto golive) {
            var routeObj = JsonConvert.DeserializeObject<dynamic>(message);
            string toClient = routeObj.Type;
            
            if (toClient == Types.ADMINGROUPLIVEMESSAGE) {
                await _handlerServices.HandleAdminLiveMessage(
                    _mapper.Map<AdminGroupLiveMessageModel>(toClient),
                    golive,
                    _dbContext,
                    _userManager,
                    Clients
                );
            }

        }
    }
}