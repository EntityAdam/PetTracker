using Core.Interface.Events;
using Core.Interface.Models;

namespace Core.Interface;

public class Facade : IDomainFacade
{
    private readonly IDataFacade data;
    private readonly IHistoryProvider history;

    public Facade(IDataFacade data, IHistoryProvider history)
    {
        this.data = data;
        this.history = history;
    }

    public void ShelterCreate(Shelter shelter, DateTimeOffset timestamp)
    {
        data.CreateShelter(shelter);
        history.NewShelterAddedEvent(shelter.Id, timestamp);
    }

    public IEnumerable<Shelter> ListShelters()
    {
        return data.ListShelters();
    }

    public void ShelterRemoveAndObfuscateData(ShelterIdentity shelterIdentity)
    {
        data.RemoveShelter(shelterIdentity);
        history.RemoveShelterHistory(shelterIdentity);
    }

    public void ShelterTransferPet(ShelteredPet shelteredPet, ShelterIdentity shelterIdentity)
    {
        data.TransferPet(shelteredPet, shelterIdentity);
        history.ShelteredPetEventTransferred(shelteredPet.Pet.Id, shelterIdentity);
    }

    public void ShelterAssignFosterPerson(FosterPetAssignment fosterPetAssignment)
    {
        var (pet, person, timestamp) = fosterPetAssignment;
        data.FosterPetAssign(fosterPetAssignment);
        history.ShelteredPetEventFostered(pet, person, timestamp);
    }

    public void PersonOpenToFoster(FosterPerson fosterPerson, DateTimeOffset timestamp)
    {
        data.CreateFosterPerson(fosterPerson);
        history.PersonOpenToFoster(fosterPerson.Id, timestamp);
    }

    public void ShelterAddPet(ShelteredPet shelteredPet, DateTimeOffset timestamp)
    {
        data.ListShelteredPet(shelteredPet);
        history.ShelteredPetEventListed(shelteredPet, timestamp);
    }

    public void FosterPersonRemove(PersonIdentity personIdentity)
    {
        data.RemoveFosterPerson(personIdentity);
        history.RemoveFosterHistory(personIdentity);
    }

    public void PersonOpenToAdoption(AdopterPerson adopterPerson, DateTimeOffset timestamp)
    {
        data.CreateAdopterPerson(adopterPerson);
        history.PersonOpenToAdoption(adopterPerson.Id);
    }

    public void ShelterAssignAdopterPerson(AdopterPetAssignment adopterPetAssignemtn)
    {
        var (pet, person, timestamp) = adopterPetAssignemtn;
        data.AdopterPetAssign(adopterPetAssignemtn);
        history.ShelteredPetEventAdopted(pet, person, timestamp);
    }

    public void AdopterPersonRemove(PersonIdentity personIdentity)
    {
        data.RemoveAdopterPerson(personIdentity);
        history.RemoveAdopterHistory(personIdentity);
    }

    public void ShelterUnlistPet(ShelteredPet shelteredPet)
    {
        var (pet, shelterId) = shelteredPet;
        data.UnlistShelteredPet(shelteredPet);
        history.RemovePetHistory(pet.Id, shelterId);
    }

    public IEnumerable<ShelteredPetEvent> GetPetHistory(PetIdentity petIdentity)
    {
        return history.GetPetHistory(petIdentity);
    }

    public IEnumerable<ShelterEvent> GetShelterDateListedByShelter(ShelterIdentity shelterIdentity)
    {
        return history.GetShelterDateListedByShelter(shelterIdentity);
    }

    public int GetPetsFosteredCount(ShelterIdentity shelterIdentity)
    {
        return history.GetPetsFosteredCountByShelter(shelterIdentity);
    }

    public IEnumerable<FosterPersonEvent> GetFosterPersonHistory(PersonIdentity personIdentity)
    {
        return history.GetFosterPersonHistory(personIdentity);
    }

    public int GetPetsListedCountByShelter(ShelterIdentity shelterIdentity)
    {
        return history.GetPetsListedCountByShelter(shelterIdentity);
    }

    public int GetPetsAdoptedCountByShelter(ShelterIdentity shelterIdentity)
    {
        return history.GetPetsAdoptedCountByShelter(shelterIdentity);
    }

    public void DeleteShelter(ShelterIdentity id)
    {
        data.RemoveShelter(id);
        history.RemoveShelterHistory(id);
    }
}
