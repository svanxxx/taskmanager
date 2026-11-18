using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models;

namespace TaskManager.Api.Data;

/// <summary>
/// PERSONS is in the TASKS catalog, separate from the TT_RES connection used by <see cref="TaskDbContext"/>.
/// </summary>
public sealed class PersonsDbContext : DbContext
{
    public PersonsDbContext(DbContextOptions<PersonsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Person> Persons => Set<Person>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>(entity =>
        {
            entity.ToTable("PERSONS", "dbo", table => table.ExcludeFromMigrations());
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("PERSON_ID").ValueGeneratedNever();
            entity.Property(e => e.WorkEmail).HasColumnName("WORK_EMAIL");
            entity.Property(e => e.Retired).HasColumnName("RETIRED");
        });
    }
}
