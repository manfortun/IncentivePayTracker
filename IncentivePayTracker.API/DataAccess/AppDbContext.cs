using IncentivePayTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace IncentivePayTracker.API.DataAccess;

public class AppDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Infraction> Infractions { get; set; }
    public DbSet<EmployeeInfraction> EmployeeInfractions { get; set; }
    public DbSet<EmployeeTimeIn> EmployeeTimeIns { get; set; }
    public DbSet<EmploymentDate> EmploymentDates { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Infraction>().HasData(
            new Infraction { Id = 1, Amount = 200, Description = "If you are missing a login and/or logout in Sprout during a work day." },
            new Infraction { Id = 2, Amount = 200, Description = "If you forget to log your hours in JIRA (or equivalent methods in your project) and send the report by 4PM the following work day.\r\n\r\nIf you forget to log your hours before you go on a planned leave." },
            new Infraction { Id = 3, Amount = 200, Description = "If your login time in Sprout is past your latest allowed login time." },
            new Infraction { Id = 4, Amount = 500, Description = "If you take a leave of absence (vacation leave) without respecting the proper advance notice period. \r\n\r\nIf you take a leave of absence with no notification at all (AWOL)" },
            new Infraction { Id = 5, Amount = 300, Description = "If you take an undertime, SL, or Compensation, but did not file it or clearly declare whether it's for deduction or compensation, that Marifil has to chase you for it.\r\n\r\n(Check https://tinyurl.com/p862fcx3 for proper filing of these absences)." },
            new Infraction { Id = 6, Amount = 500, Description = "If you were sick for 2 days without a medical certificate, or submit it late that Jobi has to chase you for it. \r\n\r\nIf you file your SL late (over 1 working day after returning from SL)." },
            new Infraction { Id = 7, Amount = 1000, Description = "If you are unavailable during project activities without giving timely notification." },
            new Infraction { Id = 8, Amount = 200, Description = "If you miss submitting an evaluation feedback (including self-evaluation) before a deadline." },
            new Infraction { Id = 9, Amount = 500, Description = "If you miss submitting an evaluation feedback (including self-evaluation) before a second or an extended deadline." });

        modelBuilder.Entity<EmployeeTimeIn>()
            .HasKey(e => new { e.EmployeeId, e.Month, e.Year });

        modelBuilder.Entity<EmployeeInfraction>()
            .HasKey(e => new { e.EmployeeId, e.InfractionId, e.Month, e.Year });
    }
}
