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

        public DbSet<Bank> Banks { get; set; }

        public DbSet<BankBusiness> BankBusinesses { get; set; }

        public DbSet<BankBusinessUser> BankBusinessUsers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la relación entre User y Business
            modelBuilder.Entity<Business>()
                .HasOne(b => b.User)  // Relación con el User (quién creó el negocio)
                .WithMany(u => u.CreatedBusinesses)  
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de la relación entre User y BankBusiness
            modelBuilder.Entity<BankBusiness>()
                .HasOne(b => b.User)  // Relación con el User (quién actualizó el saldo)
                .WithMany(u => u.BankBusinesses)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de la relación entre Business y BusinessUser
            modelBuilder.Entity<BankBusiness>()
                .HasOne(bb => bb.Bank)  // Relación con el Bank
                .WithMany(u => u.BankBusinesses)  
                .HasForeignKey(bu => bu.BankId)
                .OnDelete(DeleteBehavior.Restrict);  

            modelBuilder.Entity<BankBusiness>()
                .HasOne(bu => bu.Business)  // Relación con el Business
                .WithMany(b => b.BankBusinesses)  // Relación inversa (un Business puede ser visto por varios usuarios)
                .HasForeignKey(bu => bu.BusinessId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<BankBusinessUser>()
                .HasOne(bu => bu.User)
                .WithMany(b => b.BankBusinessUsers)
                .HasForeignKey(bu => bu.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BankBusinessUser>()
                .HasOne(bu => bu.BankBusiness)
                .WithMany(b => b.BankBusinessUsers)
                .HasForeignKey(bu => bu.BankBusinessId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}

