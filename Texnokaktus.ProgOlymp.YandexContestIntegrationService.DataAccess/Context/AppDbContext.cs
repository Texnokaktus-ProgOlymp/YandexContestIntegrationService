using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Entities;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Context;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
     public DbSet<ContestStageApplication> ContestStageApplications => Set<ContestStageApplication>();
     public DbSet<ContestStage> ContestStages => Set<ContestStage>();

     protected override void OnModelCreating(ModelBuilder modelBuilder)
     {
          modelBuilder.Entity<ContestStageApplication>(builder =>
          {
               builder.HasKey(application => application.Id);
               builder.Property(application => application.Id).UseIdentityColumn();

               builder.HasAlternateKey(application => new { application.AccountId, application.ContestStageId });

               builder.Property(application => application.CreatedUtc)
                      .HasConversion(time => time.ToUniversalTime(),
                                     time => DateTime.SpecifyKind(time, DateTimeKind.Utc));

               builder.HasOne(application => application.ContestStage)
                      .WithMany()
                      .HasForeignKey(application => application.ContestStageId);
          });

          modelBuilder.Entity<ContestStage>(builder =>
          {
               builder.HasKey(stage => stage.Id);
               builder.Property(stage => stage.Id).ValueGeneratedNever();
          });

          base.OnModelCreating(modelBuilder);
     }
}
