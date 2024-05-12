using Core.Interface.Events;
using Core.Interface.Models;

namespace Core.Interface;

public class HistoryProviderInMemeory : IHistoryProvider
{
    private readonly TimeProvider timeProvider;

    private List<AdopterPersonEvent> AdopterPersonEvents { get; set; } = new List<AdopterPersonEvent>();
    private List<FosterPersonEvent> FosterPersonEvents { get; set; } = new List<FosterPersonEvent>();
    private List<ShelterEvent> ShelterEvents { get; set; } = new List<ShelterEvent>();
    private List<ShelteredPetEvent> PetEvents { get; set; } = new List<ShelteredPetEvent>();

    public HistoryProviderInMemeory(TimeProvider timeProvider)
    {
        this.timeProvider = timeProvider;
    }

    public void ShelteredPetEventAdopted(ShelteredPet shelteredPet, PersonIdentity personIdentity, DateTimeOffset timestamp)
    {
        var (pet, shelterId) = shelteredPet;
        PetEvents.Add(new(pet.Id, PetEventKind.Adopted, timestamp));
        ShelterEvents.Add(new(shelterId, ShelterEventKind.PetAdopted, timestamp));
        AdopterPersonEvents.Add(new(personIdentity, AdopterPersonEventKind.Adopted, timestamp));
    }

    public void NewShelterAddedEvent(ShelterIdentity shelter, DateTimeOffset timestamp)
    {
        ShelterEvents.Add(new(shelter, ShelterEventKind.ShelterListed, timestamp));
    }

    public void ShelteredPetEventFostered(ShelteredPet shelteredPet, PersonIdentity person, DateTimeOffset timestamp)
    {
        var (pet, shelterId) = shelteredPet;
        FosterPersonEvents.Add(new(person, FosterPersonEventKind.Fostered, timestamp));
        PetEvents.Add(new(pet.Id, PetEventKind.Fostered, timestamp));
        ShelterEvents.Add(new(shelterId, ShelterEventKind.PetFostered, timestamp));
    }

    public IEnumerable<ShelteredPetEvent> GetPetHistory(PetIdentity petIdentity)
    {
        return PetEvents.Where(e => e.PetIdentity == petIdentity);
    }

    public void ShelteredPetEventListed(ShelteredPet shelteredPet, DateTimeOffset timestamp)
    {
        var (pet, shelterId) = shelteredPet;
        PetEvents.Add(new(pet.Id, PetEventKind.ListedAtShelter, timestamp));
        ShelterEvents.Add(new(shelterId, ShelterEventKind.PetListed, timestamp));
    }

    public void RemoveAdopterHistory(PersonIdentity personIdentity)
    {
        AdopterPersonEvents.RemoveAll(e => e.PersonIdentity == personIdentity);
    }

    public void RemoveFosterHistory(PersonIdentity personIdentity)
    {
        FosterPersonEvents.RemoveAll(e => e.PersonIdentity == personIdentity);
    }

    public void RemoveShelterHistory(ShelterIdentity shelterIdentity)
    {
        ShelterEvents.RemoveAll(e => e.ShelterIdentity == shelterIdentity);
    }

    public void ShelteredPetEventTransferred(ShelteredPet shelteredPet, ShelterIdentity targetShelter)
    {
        var (pet, originShelter) = shelteredPet;
        PetEvents.Add(new(pet.Id, PetEventKind.TransferredToAnotherShelter, timeProvider.GetUtcNow()));
        ShelterEvents.Add(new(originShelter, ShelterEventKind.PetTransferredAway, timeProvider.GetUtcNow()));
        ShelterEvents.Add(new(targetShelter, ShelterEventKind.PetTransferredHere, timeProvider.GetUtcNow()));
    }

    public void PersonOpenToFoster(PersonIdentity person, DateTimeOffset timestamp)
    {
        FosterPersonEvents.Add(new(person, FosterPersonEventKind.FosterPersonJoin, timestamp));
    }

    public void RemovePetHistory(ShelteredPet shelteredPet, ShelterIdentity shelter)
    {
        var (pet, _) = shelteredPet;
        PetEvents.RemoveAll(p => p.PetIdentity == pet.Id);
    }

    public int GetPetsFosteredCountByShelter(ShelterIdentity shelterIdentity)
    {
        return ShelterEvents.Count(e => e.ShelterEventKind == ShelterEventKind.PetFostered);
    }

    public ShelterEvent GetShelterDateListedByShelter(ShelterIdentity person)
    {
        return ShelterEvents.First(e => e.ShelterEventKind == ShelterEventKind.ShelterListed);
    }

    public IEnumerable<FosterPersonEvent> GetFosterPersonHistory(PersonIdentity person)
    {
        return FosterPersonEvents.Where(p => p.PersonIdentity == person);
    }

    public int GetPetsListedCountByShelter(ShelterIdentity shelter)
    {
        return ShelterEvents.Count(e => e.ShelterEventKind == ShelterEventKind.PetListed);
    }

    public int GetPetsAdoptedCountByShelter(ShelterIdentity shelter)
    {
        return ShelterEvents.Count(e => e.ShelterEventKind == ShelterEventKind.PetAdopted);
    }

    public void ShelteredPetEventTransferred(PetIdentity petIdentity, ShelterIdentity shelterIdentity)
    {
        PetEvents.Add(new(petIdentity, PetEventKind.TransferredToAnotherShelter, timeProvider.GetUtcNow()));
        ShelterEvents.Add(new(shelterIdentity, ShelterEventKind.PetTransferredHere, timeProvider.GetUtcNow()));
    }

    public void RemovePetHistory(PetIdentity petIdentity, ShelterIdentity shelterIdentity)
    {
        throw new NotImplementedException();
    }

    public void PersonOpenToAdoption(PersonIdentity adopterPerson)
    {
        AdopterPersonEvents.Add(new(adopterPerson, AdopterPersonEventKind.OpenToAdopt, timeProvider.GetUtcNow()));
    }
}
