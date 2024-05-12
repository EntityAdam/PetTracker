using Core.Interface;
using Core.Interface.Events;

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
        return await Task.FromResult(facade.GetShelteredPetHistory(new(shelterUlid), new(petUlid, null)));
    }
}
