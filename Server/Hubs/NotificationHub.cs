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

namespace Hubs
{
    [Authorize]
    public class NotificationHub : Hub {

        private UserManager<IdentityUser> _userManager;
        private Server _dbContext;
        private IMailService _mailService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;
        
        public NotificationHub(
            IWebHostEnvironment hostEnvironment, 
            Server dbContext, 
            UserManager<IdentityUser> userManager,
            IMailService mailService,
            IMapper mapper
        ) {
            _userManager = userManager;
            _mailService = mailService;
            _dbContext = dbContext;
            _hostEnvironment = hostEnvironment;
            _mapper = mapper;
        }

        public async Task JoinGroup(string name, string user) {
            await Groups.AddToGroupAsync(user, name);
            await Clients.Group(name).SendAsync("RecieveMessage", $"{user} joined");
        }
        public override async Task OnConnectedAsync() {
            Console.WriteLine(Context.ConnectionId);
            var user = await _userManager.GetUserAsync(Context.User);
            var email = user?.Email;
            var userId =_dbContext.Users.FirstOrDefault(x => x.Email == email).Id;

            var isExist = _dbContext.NotificationHubModels.FirstOrDefault(x => x.UserEmail == email);

            if(isExist != null) {
                isExist.ConnectionId = Context.ConnectionId;
                isExist.isOpen = true;
                _dbContext.NotificationHubModels.Update(isExist);
            } else {
                 _dbContext.NotificationHubModels.Add(new Models.NotificationHubModel() {
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
            var isExist = _dbContext.NotificationHubModels.FirstOrDefault(x => x.UserEmail == email);

            if(isExist != null) {
                isExist.ConnectionId = Context.ConnectionId;
                isExist.isOpen = false;
                _dbContext.NotificationHubModels.Update(isExist);
            }
            await _dbContext.SaveChangesAsync();
            await base.OnDisconnectedAsync(e);
        }

        public async Task SendMessageAsync(string message) {
            var routeObj = JsonConvert.DeserializeObject<dynamic>(message);
            string toClient = routeObj.Type;
            int TeamId = routeObj.TeamId;
            int UserId = routeObj.UserId;
            Console.WriteLine("Message Recieved on: "+ Context.ConnectionId);

            if(toClient == "GO_LIVE") {
                var usersWhoFollowsTeam = _dbContext.Follows.Where(x => x.TeamId == TeamId && x.UserId == UserId).ToArray();
                if(usersWhoFollowsTeam.Length >= 1)
                    await Clients.Client(Context.ConnectionId).SendAsync("RecieveMessage", message);
            }
        }
    }
}