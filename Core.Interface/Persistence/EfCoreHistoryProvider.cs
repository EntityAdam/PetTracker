using Core.Interface.Events;
using Core.Interface.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Interface.Persistence;

public class EfCoreHistoryProvider : IHistoryProvider
{
    private readonly PetTrackerDbContext db;
    private readonly TimeProvider timeProvider;

    public EfCoreHistoryProvider(PetTrackerDbContext db, TimeProvider timeProvider)
    {
        this.db = db;
        this.timeProvider = timeProvider;
    }

    public void ShelteredPetEventListed(ShelteredPet shelteredPet, DateTimeOffset timestamp)
    {
        AddPetEvent(shelteredPet.Pet.Id, PetEventKind.ListedAtShelter, timestamp);
        AddShelterEvent(shelteredPet.ShelterIdentity, ShelterEventKind.PetListed, timestamp);
        db.SaveChanges();
    }

    public void ShelteredPetEventFostered(ShelteredPet shelteredPet, PersonIdentity personIdentity, DateTimeOffset timestamp)
    {
        AddFosterPersonEvent(personIdentity, FosterPersonEventKind.Fostered, timestamp);
        AddPetEvent(shelteredPet.Pet.Id, PetEventKind.Fostered, timestamp);
        AddShelterEvent(shelteredPet.ShelterIdentity, ShelterEventKind.PetFostered, timestamp);
        db.SaveChanges();
    }

    public void ShelteredPetEventAdopted(ShelteredPet shelteredPet, PersonIdentity personIdentity, DateTimeOffset timestamp)
    {
        AddAdopterPersonEvent(personIdentity, AdopterPersonEventKind.Adopted, timestamp);
        AddPetEvent(shelteredPet.Pet.Id, PetEventKind.Adopted, timestamp);
        AddShelterEvent(shelteredPet.ShelterIdentity, ShelterEventKind.PetAdopted, timestamp);
        db.SaveChanges();
    }

    public void ShelteredPetEventTransferred(PetIdentity petIdentity, ShelterIdentity shelterIdentity)
    {
        var now = timeProvider.GetUtcNow();
        AddPetEvent(petIdentity, PetEventKind.TransferredToAnotherShelter, now);
        AddShelterEvent(shelterIdentity, ShelterEventKind.PetTransferredHere, now);
        db.SaveChanges();
    }

    public void ShelteredPetEventOutcome(ShelteredPet shelteredPet, OutcomeKind outcomeKind, DateTimeOffset timestamp)
    {
        switch (outcomeKind)
        {
            case OutcomeKind.ReturnedToOwner:
                AddPetEvent(shelteredPet.Pet.Id, PetEventKind.ReturnedToOwner, timestamp);
                AddShelterEvent(shelteredPet.ShelterIdentity, ShelterEventKind.PetReturnedToOwner, timestamp);
                break;
            case OutcomeKind.TransferredToRescue:
                AddPetEvent(shelteredPet.Pet.Id, PetEventKind.TransferredToRescue, timestamp);
                AddShelterEvent(shelteredPet.ShelterIdentity, ShelterEventKind.PetTransferredToRescue, timestamp);
                break;
            case OutcomeKind.DiedInCare:
                AddPetEvent(shelteredPet.Pet.Id, PetEventKind.DiedInCare, timestamp);
                AddShelterEvent(shelteredPet.ShelterIdentity, ShelterEventKind.PetDiedInCare, timestamp);
                break;
            case OutcomeKind.Euthanized:
                AddPetEvent(shelteredPet.Pet.Id, PetEventKind.Euthanized, timestamp);
                AddShelterEvent(shelteredPet.ShelterIdentity, ShelterEventKind.PetEuthanized, timestamp);
                break;
            default:
                throw new InvalidOperationException($"Unsupported outcome kind: {outcomeKind}");
        }

        db.SaveChanges();
    }

    public void RemovePetHistory(PetIdentity petIdentity, ShelterIdentity shelterIdentity)
    {
        var petId = petIdentity.Id.ToString();
        db.ShelteredPetEvents.RemoveRange(db.ShelteredPetEvents.Where(p => p.PetId == petId));
        db.SaveChanges();
    }

    public int GetPetsListedCountByShelter(ShelterIdentity shelterIdentity)
    {
        return db.ShelterEvents.Count(e => e.ShelterId == shelterIdentity.Id.ToString() && e.EventKind == ShelterEventKind.PetListed.ToString());
    }

    public int GetPetsFosteredCountByShelter(ShelterIdentity shelterIdentity)
    {
        return db.ShelterEvents.Count(e => e.ShelterId == shelterIdentity.Id.ToString() && e.EventKind == ShelterEventKind.PetFostered.ToString());
    }

    public int GetPetsAdoptedCountByShelter(ShelterIdentity shelterIdentity)
    {
        return db.ShelterEvents.Count(e => e.ShelterId == shelterIdentity.Id.ToString() && e.EventKind == ShelterEventKind.PetAdopted.ToString());
    }

    public IEnumerable<ShelteredPetEvent> GetPetHistory(PetIdentity petIdentity)
    {
        return db.ShelteredPetEvents
            .AsNoTracking()
            .Where(e => e.PetId == petIdentity.Id.ToString())
            .Select(e => new ShelteredPetEvent(petIdentity, Enum.Parse<PetEventKind>(e.EventKind), e.Timestamp))
            .AsEnumerable()
            .OrderBy(e => e.Timestamp)
            .ToList();
    }

    public void NewShelterAddedEvent(ShelterIdentity shelterIdentity, DateTimeOffset timestamp)
    {
        AddShelterEvent(shelterIdentity, ShelterEventKind.ShelterListed, timestamp);
        db.SaveChanges();
    }

    public void RemoveShelterHistory(ShelterIdentity shelterIdentity)
    {
        var shelterId = shelterIdentity.Id.ToString();
        db.ShelterEvents.RemoveRange(db.ShelterEvents.Where(e => e.ShelterId == shelterId));
        db.SaveChanges();
    }

    public void RemoveAdopterHistory(PersonIdentity personIdentity)
    {
        var personId = personIdentity.Id.ToString();
        db.AdopterPersonEvents.RemoveRange(db.AdopterPersonEvents.Where(e => e.PersonId == personId));
        db.SaveChanges();
    }

    public void RemoveFosterHistory(PersonIdentity personIdentity)
    {
        var personId = personIdentity.Id.ToString();
        db.FosterPersonEvents.RemoveRange(db.FosterPersonEvents.Where(e => e.PersonId == personId));
        db.SaveChanges();
    }

    public void PersonOpenToFoster(PersonIdentity personIdentity, DateTimeOffset timestamp)
    {
        AddFosterPersonEvent(personIdentity, FosterPersonEventKind.FosterPersonJoin, timestamp);
        db.SaveChanges();
    }

    public ShelterEvent GetShelterDateListedByShelter(ShelterIdentity shelterIdentity)
    {
        var row = db.ShelterEvents
            .AsNoTracking()
            .FirstOrDefault(e => e.ShelterId == shelterIdentity.Id.ToString() && e.EventKind == ShelterEventKind.ShelterListed.ToString());

        if (row is null)
        {
            throw new InvalidOperationException("Shelter listing event not found");
        }

        return new ShelterEvent(shelterIdentity, ShelterEventKind.ShelterListed, row.Timestamp);
    }

    public IEnumerable<FosterPersonEvent> GetFosterPersonHistory(PersonIdentity personIdentity)
    {
        return db.FosterPersonEvents
            .AsNoTracking()
            .Where(e => e.PersonId == personIdentity.Id.ToString())
            .Select(e => new FosterPersonEvent(personIdentity, Enum.Parse<FosterPersonEventKind>(e.EventKind), e.Timestamp))
            .AsEnumerable()
            .OrderBy(e => e.Timestamp)
            .ToList();
    }

    public void PersonOpenToAdoption(PersonIdentity adopterPerson)
    {
        AddAdopterPersonEvent(adopterPerson, AdopterPersonEventKind.OpenToAdopt, timeProvider.GetUtcNow());
        db.SaveChanges();
    }

    private void AddPetEvent(PetIdentity petIdentity, PetEventKind eventKind, DateTimeOffset timestamp)
    {
        db.ShelteredPetEvents.Add(new ShelteredPetEventRow
        {
            Id = Guid.NewGuid(),
            PetId = petIdentity.Id.ToString(),
            EventKind = eventKind.ToString(),
            Timestamp = timestamp
        });
    }

    private void AddShelterEvent(ShelterIdentity shelterIdentity, ShelterEventKind eventKind, DateTimeOffset timestamp)
    {
        db.ShelterEvents.Add(new ShelterEventRow
        {
            Id = Guid.NewGuid(),
            ShelterId = shelterIdentity.Id.ToString(),
            EventKind = eventKind.ToString(),
            Timestamp = timestamp
        });
    }

    private void AddFosterPersonEvent(PersonIdentity personIdentity, FosterPersonEventKind eventKind, DateTimeOffset timestamp)
    {
        db.FosterPersonEvents.Add(new FosterPersonEventRow
        {
            Id = Guid.NewGuid(),
            PersonId = personIdentity.Id.ToString(),
            EventKind = eventKind.ToString(),
            Timestamp = timestamp
        });
    }

    private void AddAdopterPersonEvent(PersonIdentity personIdentity, AdopterPersonEventKind eventKind, DateTimeOffset timestamp)
    {
        db.AdopterPersonEvents.Add(new AdopterPersonEventRow
        {
            Id = Guid.NewGuid(),
            PersonId = personIdentity.Id.ToString(),
            EventKind = eventKind.ToString(),
            Timestamp = timestamp
        });
    }
}
