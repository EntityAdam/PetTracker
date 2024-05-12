using Core.Interface;
using Core.Interface.Models;

public class ShelterApiViewModel(IDomainFacade facade, TimeProvider timeProvider)
{
    public async Task<Shelter> Create(ShelterModel shelter)
    {
        var create = NewEmptyShelter(shelter.Name);
        facade.ShelterCreate(create, timeProvider.GetUtcNow());
        return await Task.FromResult(create);
    }

    public async Task<IEnumerable<Shelter>> ListAll() => await Task.FromResult(facade.ListShelters());

    public async Task<bool> Delete(string shelterId)
    {
        if (!Ulid.TryParse(shelterId, out var shelterUlid))
        {
            return false;
        }
        try
        {
            ShelterIdentity shelterIdentity = new(shelterUlid);
            facade.DeleteShelter(shelterIdentity);
            return true;
        }
        catch (Exception)
        {
            return await Task.FromResult(false);
        }
    }

    public async Task<Shelter> GetById(string id)
    {
        var result = facade.ListShelters().FirstOrDefault(x => x.Id.Id.ToString() == id);
        return await Task.FromResult(result!);
    }

    private Shelter NewEmptyShelter(string name)
    {
        return new Shelter(new ShelterIdentity(Ulid.NewUlid()), new ShelterDetails(name, ""));
    }
}
