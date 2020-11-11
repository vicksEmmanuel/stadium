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
    }
}