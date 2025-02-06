using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Entities;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Context;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
     public DbSet<ContestUser> ContestUsers => Set<ContestUser>();

     protected override void OnModelCreating(ModelBuilder modelBuilder)
     {
          modelBuilder.Entity<ContestUser>(builder =>
          {
               builder.HasKey(user => user.Id);
               builder.Property(user => user.Id).UseIdentityColumn();

               builder.HasAlternateKey(user => new { user.ContestStageId, user.YandexIdLogin });
          });

          base.OnModelCreating(modelBuilder);
     }
}
