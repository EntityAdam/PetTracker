using Core.Interface;
using Core.Interface.Events;
using Core.Interface.Models;

public class ShelterPetsApiViewModel(IShelterPetsFacade facade, TimeProvider timeProvider)
{
    public async Task<ShelteredPet> AddPet(string shelterId, ListPetModel listPetModel)
    {
        if (!Ulid.TryParse(shelterId, out var ulid))
        {
            return null!;
        }
        var shelterUlid = ulid;
        var petDetails = new PetDetails(listPetModel.Name);
        var petIdentity = new PetIdentity(Ulid.NewUlid(), null);
        var pet = new Pet(petIdentity, petDetails);
        var shelteredPet = new ShelteredPet(pet, new ShelterIdentity(shelterUlid));
        facade.ShelterAddPet(shelteredPet, timeProvider.GetUtcNow());
        return await Task.FromResult<ShelteredPet>(shelteredPet);
    }

    public async Task<IEnumerable<ShelteredPet>> ListAllPets(string shelterId)
    {
        if (!Ulid.TryParse(shelterId, out var shelterUlid))
        {
            return null!;
        }
        var result = facade.GetShelteredPets(new ShelterIdentity(shelterUlid));
        return await Task.FromResult(result);
    }

    public async Task<ShelteredPet?> GetPetById(string shelterId, string petId)
    {
        if (!Ulid.TryParse(shelterId, out var shelterUlid))
        {
            return null!;
        }
        if (!Ulid.TryParse(petId, out var petUlid))
        {
            return null!;
        }
        var shelterIdentity = new ShelterIdentity(shelterUlid);
        var petIdentity = new PetIdentity(petUlid, null);

        ShelteredPet? result = facade.GetShelteredPetDetails(shelterIdentity, petIdentity);
        return await Task.FromResult(result);
    }

    public async Task<ShelteredPetEvent> TransferPet(string shelterId, string petId, string shelterIdTarget)
    {
        if (!Ulid.TryParse(shelterId, out var shelterUlid))
        {
            return null!;
        }
        if (!Ulid.TryParse(petId, out var petUlid))
        {
            return null!;
        }
        if (!Ulid.TryParse(petId, out var shelterUlidTarget))
        {
            return null!;
        }

        var petIdentity = new PetIdentity(petUlid, null);
        var shelterIdentity = new ShelterIdentity(shelterUlid);
        var shelteredPet = facade.GetShelteredPetDetails(shelterIdentity, petIdentity);
        var shelterIdentityTarget = new ShelterIdentity(shelterUlidTarget);
        facade.ShelterTransferPet(shelteredPet, shelterIdentityTarget);
        
        //TODO FIX
        //var result = facade.GetPetHistory(petIdentity).Last();
        var result = new ShelteredPetEvent(petIdentity, PetEventKind.TransferredToAnotherShelter, timeProvider.GetUtcNow());
        
        return await Task.FromResult(result);
    }
}
