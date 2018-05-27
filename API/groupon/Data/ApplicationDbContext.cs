using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using groupon.Models;

namespace groupon.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Group> Groups { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyTeam> CompanyTeam { get; set; }
        public DbSet<GroupTeam> GroupTeam { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Connection> Connections { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);
            builder.Entity<CompanyTeam>().HasKey(t => new {t.CompanyId, t.UserId});
            builder.Entity<GroupTeam>().HasKey(t => new {t.GroupId, t.UserId});

            builder.Entity<Connection>().HasKey(t => new {t.User1Id, t.User2Id});
            builder.Entity<Connection>()
                .HasOne(i => i.User1)
                .WithMany()
                .HasForeignKey(i => i.User1Id)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Connection>()
                .HasOne(i => i.User2)
                .WithMany()
                .HasForeignKey(i => i.User2Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>().HasKey(t => new { t.RecipientId, t.SenderId });
            builder.Entity<Message>()
                .HasOne(i => i.Sender)
                .WithMany()
                .HasForeignKey(i => i.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(i => i.Recipient)
                .WithMany()
                .HasForeignKey(i => i.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);







        }
    }
}
