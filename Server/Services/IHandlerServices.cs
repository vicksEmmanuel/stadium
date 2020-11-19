using System;
using System.Threading.Tasks;
using Constantine;
using Models;
using Microsoft.AspNetCore.SignalR;
using Context;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace Services
{
    public interface IHandlerServices {
        Task HandleAdminLiveMessage(
            AdminGroupLiveMessageModel adminGroupLiveMessageModel, 
            GoLiveDto goLiveDto,
            Server dbContext, 
            UserManager<IdentityUser> userManager,
            IHubCallerClients clients
        );
    }
    public class HandlerServices : IHandlerServices
    {
        public async Task HandleAdminLiveMessage(
            AdminGroupLiveMessageModel adminGroupLiveMessageModel, 
            GoLiveDto goLiveDto, Server dbContext, 
            UserManager<IdentityUser> userManager, IHubCallerClients clients
        )
        {
            await clients.Group(goLiveDto.UserTeamGroup)
            .SendAsync("RecieveCommentary", 
                        JsonConvert.SerializeObject(adminGroupLiveMessageModel)
                    );
            
        }
    }
}