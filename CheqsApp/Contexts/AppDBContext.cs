using CheqsApp.Models;
using Microsoft.EntityFrameworkCore;
using Type = CheqsApp.Models.Type;

namespace CheqsApp.Contexts
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<Cheq> Cheqs { get; set; }

        public DbSet<Entity> Entities { get; set; }

        public DbSet<State> States { get; set; }

        public DbSet<Type> Types { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Business> Businesses { get; set; }

        public DbSet<BusinessUser> BusinessUser { get; set; }




    }
}
