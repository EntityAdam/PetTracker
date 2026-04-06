using Core.Interface.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Interface.Persistence;

public class EfCoreDataFacade : IDataFacade
{
    private readonly PetTrackerDbContext db;

    public EfCoreDataFacade(PetTrackerDbContext db)
    {
        this.db = db;
    }

    public void CreateShelter(Shelter shelter)
    {
        var shelterId = shelter.Id.Id.ToString();
        if (db.Shelters.Any(s => s.ShelterId == shelterId))
        {
            throw new InvalidOperationException("Shelter already exists");
        }

        db.Shelters.Add(new ShelterRow
        {
            ShelterId = shelterId,
            DisplayName = shelter.ShelterDetails.DisplayName,
            Address = shelter.ShelterDetails.Address
        });
        db.SaveChanges();
    }

    public IEnumerable<Shelter> ListShelters()
    {
        return db.Shelters
            .AsNoTracking()
            .Select(s => new Shelter(new ShelterIdentity(Ulid.Parse(s.ShelterId)), new ShelterDetails(s.DisplayName, s.Address)))
            .ToList();
    }

    public void RemoveShelter(ShelterIdentity shelterIdentity)
    {
        var shelterId = shelterIdentity.Id.ToString();
        if (!db.Shelters.Any(s => s.ShelterId == shelterId))
        {
            throw new InvalidOperationException("Shelter does not exist");
        }

        if (db.ShelteredPets.Any(p => p.ShelterId == shelterId))
        {
            throw new InvalidOperationException("Shelter is not empty");
        }

        db.Shelters.RemoveRange(db.Shelters.Where(s => s.ShelterId == shelterId));
        db.SaveChanges();
    }

    public void ListShelteredPet(ShelteredPet shelteredPet)
    {
        var shelterId = shelteredPet.ShelterIdentity.Id.ToString();
        var petId = shelteredPet.Pet.Id.Id.ToString();

        if (db.ShelteredPets.Any(p => p.PetId == petId))
        {
            throw new InvalidOperationException("Pet already exists");
        }

        if (!db.Shelters.Any(s => s.ShelterId == shelterId))
        {
            throw new InvalidOperationException("Developer Guard: The shelter does not exist");
        }

        db.ShelteredPets.Add(new ShelteredPetRow
        {
            PetId = petId,
            PetName = shelteredPet.Pet.PetDetails.Name,
            ShelterId = shelterId
        });
        db.SaveChanges();
    }

    public void TransferPet(ShelteredPet shelteredPet, ShelterIdentity shelterIdentity)
    {
        var targetShelterId = shelterIdentity.Id.ToString();
        var originalShelterId = shelteredPet.ShelterIdentity.Id.ToString();
        var petId = shelteredPet.Pet.Id.Id.ToString();

        if (originalShelterId == targetShelterId)
        {
            throw new InvalidOperationException("Origin shelter is target shelter");
        }

        if (db.FosteredPets.Any(p => p.PetId == petId))
        {
            throw new InvalidOperationException("Pet is already fostered");
        }

        if (db.AdoptedPets.Any(p => p.PetId == petId))
        {
            throw new InvalidOperationException("Pet is already adopted");
        }

        var petRow = db.ShelteredPets.SingleOrDefault(p => p.PetId == petId && p.ShelterId == originalShelterId);
        if (petRow is null)
        {
            throw new InvalidOperationException("Pet does not exist");
        }

        petRow.ShelterId = targetShelterId;
        db.SaveChanges();
    }

    public void UnlistShelteredPet(ShelteredPet shelteredPet)
    {
        var petId = shelteredPet.Pet.Id.Id.ToString();
        var shelterId = shelteredPet.ShelterIdentity.Id.ToString();
        var row = db.ShelteredPets.SingleOrDefault(p => p.PetId == petId && p.ShelterId == shelterId);
        if (row is null)
        {
            throw new InvalidOperationException("Pet does not exist");
        }

        db.ShelteredPets.Remove(row);
        db.SaveChanges();
    }

    public void CreateFosterPerson(FosterPerson fosterPerson)
    {
        var personId = fosterPerson.Id.Id.ToString();
        if (db.FosterPeople.Any(p => p.PersonId == personId))
        {
            throw new InvalidOperationException("Person already exists");
        }

        db.FosterPeople.Add(new FosterPersonRow
        {
            PersonId = personId,
            Name = fosterPerson.fosterPersonDetails.Name,
            MaxPets = fosterPerson.fosterPersonDetails.MaxPets
        });
        db.SaveChanges();
    }

    public void FosterPetAssign(FosterPetAssignment fosterPetAssignment)
    {
        var (pet, person, timestamp) = fosterPetAssignment;
        var petId = pet.Pet.Id.Id.ToString();

        if (db.AdoptedPets.Any(p => p.PetId == petId))
        {
            throw new InvalidOperationException("Adopted pets can't be assigned to a foster");
        }

        if (db.FosteredPets.Any(p => p.PetId == petId))
        {
            throw new InvalidOperationException("Pet is already fostered");
        }

        db.FosteredPets.Add(new FosteredPetRow
        {
            PetId = petId,
            PersonId = person.Id.ToString(),
            ShelterId = pet.ShelterIdentity.Id.ToString(),
            PetName = pet.Pet.PetDetails.Name,
            AssignedAt = timestamp
        });

        db.ShelteredPets.RemoveRange(db.ShelteredPets.Where(p => p.PetId == petId));
        db.SaveChanges();
    }

    public void RemoveFosterPerson(PersonIdentity personIdentity)
    {
        var personId = personIdentity.Id.ToString();
        if (db.FosteredPets.Any(p => p.PersonId == personId))
        {
            throw new InvalidOperationException("Foster fosterPerson is in custody of pets that must be returned");
        }

        db.FosterPeople.RemoveRange(db.FosterPeople.Where(p => p.PersonId == personId));
        db.SaveChanges();
    }

    public void CreateAdopterPerson(AdopterPerson adopterPerson)
    {
        db.AdopterPeople.Add(new AdopterPersonRow
        {
            PersonId = adopterPerson.Id.Id.ToString(),
            Name = adopterPerson.adopterPersonDetails.Name
        });
        db.SaveChanges();
    }

    public void AdopterPetAssign(AdopterPetAssignment adopterPetAssignment)
    {
        var (pet, person, timestamp) = adopterPetAssignment;
        var petId = pet.Pet.Id.Id.ToString();

        if (db.AdoptedPets.Any(p => p.PetId == petId))
        {
            throw new InvalidOperationException("Pet is already adopted");
        }

        if (db.FosteredPets.Any(p => p.PetId == petId))
        {
            throw new InvalidOperationException("Pet is currently fostered");
        }

        db.AdoptedPets.Add(new AdoptedPetRow
        {
            PetId = petId,
            PersonId = person.Id.ToString(),
            ShelterId = pet.ShelterIdentity.Id.ToString(),
            PetName = pet.Pet.PetDetails.Name,
            AdoptedAt = timestamp
        });

        db.ShelteredPets.RemoveRange(db.ShelteredPets.Where(p => p.PetId == petId));
        db.SaveChanges();
    }

    public void RemoveAdopterPerson(PersonIdentity personIdentity)
    {
        var personId = personIdentity.Id.ToString();
        db.AdoptedPets.RemoveRange(db.AdoptedPets.Where(p => p.PersonId == personId));
        db.AdopterPeople.RemoveRange(db.AdopterPeople.Where(p => p.PersonId == personId));
        db.SaveChanges();
    }

    public ShelteredPet? GetShelteredPetDetails(ShelterIdentity shelterIdentity, PetIdentity petIdentity)
    {
        var row = db.ShelteredPets
            .AsNoTracking()
            .SingleOrDefault(p => p.ShelterId == shelterIdentity.Id.ToString() && p.PetId == petIdentity.Id.ToString());

        return row is null ? null : MapShelteredPet(row);
    }

    public IEnumerable<ShelteredPet> GetShelteredPets(ShelterIdentity shelterIdentity)
    {
        return db.ShelteredPets
            .AsNoTracking()
            .Where(p => p.ShelterId == shelterIdentity.Id.ToString())
            .Select(MapShelteredPet)
            .ToList();
    }

    private static ShelteredPet MapShelteredPet(ShelteredPetRow row)
    {
        var petIdentity = new PetIdentity(Ulid.Parse(row.PetId), null);
        var pet = new Pet(petIdentity, new PetDetails(row.PetName));
        return new ShelteredPet(pet, new ShelterIdentity(Ulid.Parse(row.ShelterId)));
    }
}
