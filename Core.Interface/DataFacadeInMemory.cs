using Core.Interface.Models;

namespace Core.Interface;

public class DataFacadeInMemory : IDataFacade
{
    private readonly TimeProvider timeProvider;

    private List<AdopterPerson> AdopterPeople { get; set; } = new List<AdopterPerson>();
    private List<FosterPerson> FosterPeople { get; set; } = new List<FosterPerson>();
    private List<Shelter> Shelters { get; set; } = new List<Shelter>();
    private List<ShelteredPet> ShelteredPets { get; set; } = new List<ShelteredPet>();
    private List<FosteredPet> FosteredPets { get; set; } = new List<FosteredPet>();
    private List<AdoptedPet> AdoptedPets { get; set; } = new List<AdoptedPet>();

    public DataFacadeInMemory(TimeProvider timeProvider)
    {
        this.timeProvider = timeProvider;
    }

    public void CreateFosterPerson(FosterPerson fosterPerson)
    {
        if (FosterPeople.Exists(p => p.Id == fosterPerson.Id)) throw new InvalidOperationException("Person already exists");
        FosterPeople.Add(fosterPerson);
    }

    public void CreateShelter(Shelter shelter)
    {
        if (Shelters.Exists(p => p == shelter)) throw new InvalidOperationException("Shelter already exists");
        Shelters.Add(shelter);
    }

    public IEnumerable<Shelter> ListShelters()
    {
        return Shelters;
    }

    public void ListShelteredPet(ShelteredPet shelteredPet)
    {
        var (_, shelterId) = shelteredPet;
        if (ShelteredPets.Exists(p => p == shelteredPet)) throw new InvalidOperationException("Pet already exists");
        if (!Shelters.Exists(s => s.Id == shelterId)) throw new InvalidOperationException("Developer Guard: The shelter does not exist");
        ShelteredPets.Add(shelteredPet);
    }

    public void UnlistShelteredPet(ShelteredPet shelteredPet)
    {
        if (!ShelteredPets.Exists(p => p == shelteredPet)) throw new InvalidOperationException("Pet does not exist");
        ShelteredPets.Remove(shelteredPet);
    }

    public void FosterPetAssign(FosterPetAssignment assignment)
    {
        var (pet, person, timestamp) = assignment;
        if (AdoptedPets.Exists(p => p.Pet == pet)) throw new InvalidOperationException("Adopted pets can't be assigned to a foster"); //shouldnt need this, wouldnt be listed as available pets
        if (FosteredPets.Exists(p => p.Pet == pet)) throw new InvalidOperationException("Pet is already fostered"); //shouldnt need this, wouldnt be listed as available pets
        ShelteredPets.Remove(pet);
        FosteredPets.Add(new(pet, person));
    }

    public void RemoveShelter(ShelterIdentity shelterId)
    {
        if (!Shelters.Any(s => s.Id == shelterId)) throw new InvalidOperationException("Shelter does not exist");
        if (ShelteredPets.Any(p => p.ShelterIdentity == shelterId)) throw new InvalidOperationException("Shelter is not empty");
        var shelter = Shelters.Single(s => s.Id == shelterId);
        Shelters.Remove(shelter);
    }

    public void TransferPet(ShelteredPet shelteredPet, ShelterIdentity shelterIdentity)
    {
        ShelterIdentity originalShelter = shelteredPet.ShelterIdentity;
        if (originalShelter == shelterIdentity) throw new InvalidOperationException("Origin shelter is target shelter");
        if (FosteredPets.Exists(p => p.Pet == shelteredPet)) throw new InvalidOperationException("Pet is already fostered");
        if (AdoptedPets.Exists(p => p.Pet == shelteredPet)) throw new InvalidOperationException("Pet is already adopted");
        ShelteredPet targetPet = shelteredPet with { ShelterIdentity = shelterIdentity };
        ShelteredPets.Remove(shelteredPet);
        ShelteredPets.Add(targetPet);
    }

    public void CreateAdopterPerson(AdopterPerson person)
    {
        AdopterPeople.Add(person);
    }

    public void AdopterPetAssign(AdopterPetAssignment assignment)
    {
        var (pet, person, timestamp) = assignment;
        if (AdoptedPets.Exists(p => p.Pet == pet)) throw new InvalidOperationException("Pet is already adopted"); //shouldnt need this, wouldnt be listed as available pets
        if (FosteredPets.Exists(p => p.Pet == pet)) throw new InvalidOperationException("Pet is currently fostered"); //shouldnt need this, wouldnt be listed as available pets
        ShelteredPets.Remove(pet);
        AdoptedPets.Add(new(pet, person));
    }

    public void RemoveAdopterPerson(PersonIdentity person)
    {
        AdoptedPets.RemoveAll(p => p.PersonIdentity == person);
        AdopterPeople.RemoveAll(p => p.Id == person);
    }

    public void RemoveFosterPerson(PersonIdentity person)
    {
        if (FosteredPets.Any(p => p.PersonIdentity == person)) throw new InvalidOperationException("Foster fosterPerson is in custody of pets that must be returned");
        FosterPeople.RemoveAll(p=>p.Id == person);
        
    }
}
