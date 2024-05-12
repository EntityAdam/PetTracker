using Core.Interface.Models;

namespace Core.Interface
{
    public interface IShelterPetsFacade
    {
        void ShelterAddPet(ShelteredPet shelteredPet, DateTimeOffset timestamp);
        IEnumerable<ShelteredPet> GetShelteredPets(ShelterIdentity shelterIdentity);
        ShelteredPet? GetShelteredPetDetails(ShelterIdentity shelterIdentity, PetIdentity petIdentity);
        void ShelterTransferPet(ShelteredPet shelteredPet, ShelterIdentity shelterIdentity);
        void ShelterUnlistPet(ShelteredPet shelteredPet);

    }
}