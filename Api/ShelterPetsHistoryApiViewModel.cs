using Core.Interface;
using Core.Interface.Events;
using Core.Interface.Models;

public class ShelterPetsHistoryApiViewModel(IDomainFacade facade, TimeProvider timeProvider)
{
    public async Task<IEnumerable<ShelteredPetEvent>> GetShelteredPetHistory(string shelterId, string petId)
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

        IEnumerable<ShelteredPetEvent> result = facade.GetShelteredPetHistory(new(shelterUlid), new(petUlid, null));
        return await Task.FromResult(result);
    }
}
