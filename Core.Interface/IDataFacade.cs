using Core.Interface.Models;

namespace Core.Interface;

public interface IDataFacade
{
    void CreateShelter(Shelter shelter);
    IEnumerable<Shelter> ListShelters();
    void RemoveShelter(ShelterIdentity shelterIdentity);


    void ListShelteredPet(ShelteredPet shelteredPet);
    void TransferPet(ShelteredPet shelteredPet, ShelterIdentity shelterIdentity);
    void UnlistShelteredPet(ShelteredPet shelteredPet);


    void CreateFosterPerson(FosterPerson fosterPerson);
    void FosterPetAssign(FosterPetAssignment fosterPetAssignment);
    void RemoveFosterPerson(PersonIdentity personIdentity);

    void CreateAdopterPerson(AdopterPerson adopterPerson);
    void AdopterPetAssign(AdopterPetAssignment adopterPetAssignment);
    void RemoveAdopterPerson(PersonIdentity personIdentity);
}
