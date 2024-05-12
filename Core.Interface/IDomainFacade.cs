using Core.Interface.Events;
using Core.Interface.Models;

namespace Core.Interface
{
    public interface IDomainFacade
    {
        void PersonOpenToAdoption(AdopterPerson adopterPerson, DateTimeOffset timestamp);
        void AdopterPersonRemove(PersonIdentity personIdentity);
        void DeleteShelter(ShelterIdentity id);
        void PersonOpenToFoster(FosterPerson fosterPerson, DateTimeOffset timestamp);
        void FosterPersonRemove(PersonIdentity personIdentity);
        IEnumerable<FosterPersonEvent> GetFosterPersonHistory(PersonIdentity personIdentity);
        IEnumerable<ShelteredPetEvent> GetPetHistory(PetIdentity petidentity);
        int GetPetsAdoptedCountByShelter(ShelterIdentity shelterIdentity);
        int GetPetsFosteredCountByShelter(ShelterIdentity shelterIdentity);
        int GetPetsListedCountByShelter(ShelterIdentity shelterIdentity);
        ShelterEvent GetShelterDateListedByShelter(ShelterIdentity shelterIdentity);
        IEnumerable<Shelter> ListShelters();
        void ShelterAddPet(ShelteredPet shelteredPet, DateTimeOffset timestamp);
        void ShelterAssignAdopterPerson(AdopterPetAssignment adopterPetAssignement);
        void ShelterAssignFosterPerson(FosterPetAssignment fosterPetAssignment);
        void ShelterCreate(Shelter shelter, DateTimeOffset timestamp);
        void ShelterRemoveAndObfuscateData(ShelterIdentity shelterIdentity);
        void ShelterTransferPet(ShelteredPet shelteredPet, ShelterIdentity shelterIdentity);
        void ShelterUnlistPet(ShelteredPet shelteredPet);
        ShelteredPet? GetShelteredPetDetails(ShelterIdentity shelterIdentity, PetIdentity petIdentity);
        IEnumerable<ShelteredPet> GetShelteredPets(ShelterIdentity shelterIdentity);
        IEnumerable<ShelteredPetEvent> GetShelteredPetHistory(ShelterIdentity shelterIdentity, PetIdentity petIdentity);
    }
}