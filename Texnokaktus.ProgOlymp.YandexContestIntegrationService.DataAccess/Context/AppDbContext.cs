using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Entities;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Context;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
     public DbSet<ContestStageApplication> ContestStageApplications => Set<ContestStageApplication>();

     protected override void OnModelCreating(ModelBuilder modelBuilder)
     {
          modelBuilder.Entity<ContestStageApplication>(builder =>
          {
               builder.HasKey(application => application.Id);
               builder.Property(application => application.Id).UseIdentityColumn();

               builder.HasAlternateKey(application => new { application.AccountId, ContextStageId = application.ContestStageId });

               builder.Property(application => application.CreatedUtc)
                      .HasConversion(time => time.ToUniversalTime(),
                                     time => DateTime.SpecifyKind(time, DateTimeKind.Utc));
          });

          base.OnModelCreating(modelBuilder);
     }
}
