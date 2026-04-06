using Microsoft.EntityFrameworkCore;

namespace Core.Interface.Persistence;

public class PetTrackerDbContext : DbContext
{
    public DbSet<ShelterRow> Shelters => Set<ShelterRow>();
    public DbSet<ShelteredPetRow> ShelteredPets => Set<ShelteredPetRow>();
    public DbSet<FosterPersonRow> FosterPeople => Set<FosterPersonRow>();
    public DbSet<AdopterPersonRow> AdopterPeople => Set<AdopterPersonRow>();
    public DbSet<FosteredPetRow> FosteredPets => Set<FosteredPetRow>();
    public DbSet<AdoptedPetRow> AdoptedPets => Set<AdoptedPetRow>();

    public DbSet<ShelteredPetEventRow> ShelteredPetEvents => Set<ShelteredPetEventRow>();
    public DbSet<ShelterEventRow> ShelterEvents => Set<ShelterEventRow>();
    public DbSet<FosterPersonEventRow> FosterPersonEvents => Set<FosterPersonEventRow>();
    public DbSet<AdopterPersonEventRow> AdopterPersonEvents => Set<AdopterPersonEventRow>();

    public PetTrackerDbContext(DbContextOptions<PetTrackerDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShelterRow>().HasKey(x => x.ShelterId);
        modelBuilder.Entity<ShelteredPetRow>().HasKey(x => x.PetId);
        modelBuilder.Entity<FosterPersonRow>().HasKey(x => x.PersonId);
        modelBuilder.Entity<AdopterPersonRow>().HasKey(x => x.PersonId);
        modelBuilder.Entity<FosteredPetRow>().HasKey(x => x.PetId);
        modelBuilder.Entity<AdoptedPetRow>().HasKey(x => x.PetId);

        modelBuilder.Entity<ShelteredPetRow>().HasIndex(x => x.ShelterId);

        modelBuilder.Entity<ShelteredPetEventRow>().HasKey(x => x.Id);
        modelBuilder.Entity<ShelterEventRow>().HasKey(x => x.Id);
        modelBuilder.Entity<FosterPersonEventRow>().HasKey(x => x.Id);
        modelBuilder.Entity<AdopterPersonEventRow>().HasKey(x => x.Id);

        modelBuilder.Entity<ShelteredPetEventRow>().HasIndex(x => x.PetId);
        modelBuilder.Entity<ShelterEventRow>().HasIndex(x => x.ShelterId);
        modelBuilder.Entity<FosterPersonEventRow>().HasIndex(x => x.PersonId);
        modelBuilder.Entity<AdopterPersonEventRow>().HasIndex(x => x.PersonId);
    }
}

public class ShelterRow
{
    public string ShelterId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

public class ShelteredPetRow
{
    public string PetId { get; set; } = string.Empty;
    public string PetName { get; set; } = string.Empty;
    public string ShelterId { get; set; } = string.Empty;
}

public class FosterPersonRow
{
    public string PersonId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int MaxPets { get; set; }
}

public class AdopterPersonRow
{
    public string PersonId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class FosteredPetRow
{
    public string PetId { get; set; } = string.Empty;
    public string PersonId { get; set; } = string.Empty;
    public string ShelterId { get; set; } = string.Empty;
    public string PetName { get; set; } = string.Empty;
    public DateTimeOffset AssignedAt { get; set; }
}

public class AdoptedPetRow
{
    public string PetId { get; set; } = string.Empty;
    public string PersonId { get; set; } = string.Empty;
    public string ShelterId { get; set; } = string.Empty;
    public string PetName { get; set; } = string.Empty;
    public DateTimeOffset AdoptedAt { get; set; }
}

public class ShelteredPetEventRow
{
    public Guid Id { get; set; }
    public string PetId { get; set; } = string.Empty;
    public string EventKind { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }
}

public class ShelterEventRow
{
    public Guid Id { get; set; }
    public string ShelterId { get; set; } = string.Empty;
    public string EventKind { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }
}

public class FosterPersonEventRow
{
    public Guid Id { get; set; }
    public string PersonId { get; set; } = string.Empty;
    public string EventKind { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }
}

public class AdopterPersonEventRow
{
    public Guid Id { get; set; }
    public string PersonId { get; set; } = string.Empty;
    public string EventKind { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }
}
