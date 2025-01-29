using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Entities;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Context;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
     public DbSet<ContestStage> ContestStages => Set<ContestStage>();
     public DbSet<ContestUser> ContestUsers => Set<ContestUser>();

     protected override void OnModelCreating(ModelBuilder modelBuilder)
     {
          modelBuilder.Entity<ContestStage>(builder =>
          {
               builder.HasKey(stage => stage.Id);
               builder.Property(stage => stage.Id).ValueGeneratedNever();

               builder.HasIndex(stage => stage.YandexContestId)
                      .HasFilter($"[{nameof(ContestStage.YandexContestId)}] IS NOT NULL")
                      .IsUnique();
          });

          modelBuilder.Entity<ContestUser>(builder =>
          {
               builder.HasKey(user => user.Id);
               builder.Property(user => user.Id).UseIdentityColumn();

               builder.HasAlternateKey(user => new { user.ContestStageId, user.YandexIdLogin });

               builder.HasOne(user => user.ContestStage)
                      .WithMany()
                      .HasForeignKey(user => user.ContestStageId);
          });

          base.OnModelCreating(modelBuilder);
     }
}
