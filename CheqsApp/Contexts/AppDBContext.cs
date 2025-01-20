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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la relación entre User y Business
            modelBuilder.Entity<Business>()
                .HasOne(b => b.User)  // Relación con el User (quién creó el negocio)
                .WithMany(u => u.CreatedBusinesses)  // Relación inversa (un User tiene varios negocios)
                .HasForeignKey(b => b.UserId)  // Clave foránea
                .OnDelete(DeleteBehavior.Restrict);  // Configura la eliminación en cascada (cuando se elimine un User, se eliminan sus Business)

            // Configuración de la relación entre Business y BusinessUser
            modelBuilder.Entity<BusinessUser>()
                .HasOne(bu => bu.User)  // Relación con el User
                .WithMany(u => u.BusinessUsers)  // Relación inversa (un User puede ver varios negocios)
                .HasForeignKey(bu => bu.UserId)
                .OnDelete(DeleteBehavior.Restrict);  // Configura la eliminación en cascada (eliminar BusinessUser cuando se elimine un User)

            modelBuilder.Entity<BusinessUser>()
                .HasOne(bu => bu.Business)  // Relación con el Business
                .WithMany(b => b.BusinessUsers)  // Relación inversa (un Business puede ser visto por varios usuarios)
                .HasForeignKey(bu => bu.BusinessId)
                .OnDelete(DeleteBehavior.Restrict);  // Configura la eliminación en cascada (eliminar BusinessUser cuando se elimine un Business)
        }
    }

}

