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
            builder.HasKey(user => user.ParticipantId);
            builder.HasAlternateKey(user => new { user.ContestStageId, user.ParticipantId });
            builder.HasAlternateKey(user => new { user.ContestStageId, user.YandexIdLogin });

            builder.Property(user => user.ParticipantId).ValueGeneratedNever();
        });

        base.OnModelCreating(modelBuilder);
    }
}
