using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Context
{
    public class Server : IdentityDbContext
    {
        public Server(DbContextOptions<Server> options) : base(options)
        {
            
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Sport> Sports { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Players> Players { get; set; }
        public DbSet<NotificationModel> NotificationModel { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<NotificationHubModel> NotificationHubModels {get; set;}
        public DbSet<GoLiveHubModel> GoLiveHubModels {get; set;}
    }
}