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
            builder.HasKey(user => new { user.ContestStageId, user.YandexIdLogin });
        });

        base.OnModelCreating(modelBuilder);
    }
}
