using Core.Interface.Events;
using Core.Interface.Models;

namespace Core.Interface
{
    public interface IDomainFacade
    {
        void PersonOpenToAdoption(AdopterPerson adopterPerson, DateTimeOffset timestamp);
        //void PersonClosedToAdoption(AdopterPerson adopterPerson, DateTimeOffset timestamp);
        void PersonOpenToFoster(FosterPerson fosterPerson, DateTimeOffset timestamp);
        //void PersonClosedToFoster(FosterPerson fosterPerson, DateTimeOffset timestamp);


        void AdopterPersonRemove(PersonIdentity personIdentity);

        
        void FosterPersonRemove(PersonIdentity personIdentity);
        IEnumerable<FosterPersonEvent> GetFosterPersonHistory(PersonIdentity personIdentity);
        IEnumerable<ShelteredPetEvent> GetPetHistory(PetIdentity petidentity);
        int GetPetsAdoptedCountByShelter(ShelterIdentity shelterIdentity);
        int GetPetsFosteredCountByShelter(ShelterIdentity shelterIdentity);
        int GetPetsListedCountByShelter(ShelterIdentity shelterIdentity);
 
        void ShelterAssignAdopterPerson(AdopterPetAssignment adopterPetAssignement);
        void ShelterAssignFosterPerson(FosterPetAssignment fosterPetAssignment);



        IEnumerable<ShelteredPetEvent> GetShelteredPetHistory(ShelterIdentity shelterIdentity, PetIdentity petIdentity);
    }
}