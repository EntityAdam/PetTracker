using Core.Interface.Events;
using Core.Interface.Models;

namespace Core.Interface;

public interface IHistoryProvider
{
    public void ShelteredPetEventListed(ShelteredPet shelteredPet, DateTimeOffset timestamp);
    public void ShelteredPetEventFostered(ShelteredPet shelteredPet, PersonIdentity personIdentity, DateTimeOffset timestamp);
    public void ShelteredPetEventAdopted(ShelteredPet shelteredPet, PersonIdentity personIdentity, DateTimeOffset timestamp);
    public void ShelteredPetEventTransferred(PetIdentity petIdentity, ShelterIdentity shelterIdentity);


    public void RemovePetHistory(PetIdentity petIdentity, ShelterIdentity shelterIdentity);

    public int GetPetsListedCountByShelter(ShelterIdentity shelterIdentity);
    public int GetPetsFosteredCountByShelter(ShelterIdentity shelterIdentity);
    public int GetPetsAdoptedCountByShelter(ShelterIdentity shelterIdentity);

    public IEnumerable<ShelteredPetEvent> GetPetHistory(PetIdentity petIdentity);

    public void NewShelterAddedEvent(ShelterIdentity shelterIdentity, DateTimeOffset timestamp);
    public void RemoveShelterHistory(ShelterIdentity shelterIdentity);
    public void RemoveAdopterHistory(PersonIdentity personIdentity);
    public void RemoveFosterHistory(PersonIdentity personIdentity);
    public void PersonOpenToFoster(PersonIdentity personIdentity, DateTimeOffset timestamp);
    ShelterEvent GetShelterDateListedByShelter(ShelterIdentity personIdentity);
    IEnumerable<FosterPersonEvent> GetFosterPersonHistory(PersonIdentity personIdentity);
    public void PersonOpenToAdoption(PersonIdentity adopterPerson);
}
