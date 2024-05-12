using Core.Interface.Models;

namespace Core.Interface
{
    public interface IShelterFacade
    {
        void ShelterCreate(Shelter shelter, DateTimeOffset timestamp);
        IEnumerable<Shelter> ListShelters();
        void DeleteShelter(ShelterIdentity id);
        void ShelterRemoveAndObfuscateData(ShelterIdentity shelterIdentity);
    }
}