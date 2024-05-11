using Core.Interface;
using Core.Interface.Events;
using Core.Interface.Models;

public interface IShelterApiHandler
{
    Task<Shelter> Delete(string id);
    Task<Shelter> GetById(string id);
    Task<IEnumerable<ShelterEvent>> GetHistoryById(string shelterId);
    Task<IEnumerable<ShelterEvent>> GetHistoryEventTypeById(string shelterId, int eventKind);
    Task<IEnumerable<ShelterEvent>> GetListedDate(string shelterId);
    Task<ShelteredPet> ListPet(string shelterId, ListPetModel listPetModel);
    Task<Shelter> ShelterCreate(ShelterModel shelter);
    Task<IEnumerable<Shelter>> ShelterList();
}

public class ShelterApiHandler(IDomainFacade facade, TimeProvider timeProvider) : IShelterApiHandler
{
    public async Task<Shelter> Delete(string id)
    {
        if (!Ulid.TryParse(id, out var ulid))
        {
            return null!;
        }


        var result = await GetById(id);
        facade.DeleteShelter(result.Id);
        return result;
    }

    public async Task<Shelter> GetById(string id)
    {
        var result = facade.ListShelters().FirstOrDefault(x => x.Id.Id.ToString() == id);
        return await Task.FromResult(result!);
    }

    public Task<IEnumerable<ShelterEvent>> GetHistoryById(string shelterId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ShelterEvent>> GetHistoryEventTypeById(string shelterId, int eventKind)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ShelterEvent>> GetListedDate(string shelterId)
    {
        if (!Ulid.TryParse(shelterId, out var ulid))
        {
            return null!;
        }

        var result = facade.GetShelterDateListedByShelter(new(ulid));
        return await Task.FromResult(result);

    }

    public Task<ListPetModel> ListPet(string shelterId, ListPetModel listPetModel)
    {
        throw new NotImplementedException();
    }

    //public async Task<IEnumerable<ShelterEvent>> GetHistoryById(string shelterId)
    //{
    //    IList<ShelterEvent> events = Array.Empty<ShelterEvent>();
    //    var ulid = Ulid.Parse(shelterId);
    //    var listed = facade.GetShelterDateListedByShelter(new ShelterIdentity(ulid));
    //    return await Task.FromResult(listed);
    //}

    //public async Task<IEnumerable<ShelterEvent>> GetHistoryEventTypeById(string shelterId, int eventKind)
    //{
    //    var ulid = Ulid.Parse(shelterId);
    //    ShelterEventKind shelterEventKind = (ShelterEventKind)eventKind;
    //    var result = facade.GetShelterDateListedByShelter(new(ulid));  //todo wrong impl
    //    return await Task.FromResult(result);
    //}

    public async Task<Shelter> ShelterCreate(ShelterModel shelter)
    {
        var create = NewEmptyShelter(shelter.Name);
        facade.ShelterCreate(create, timeProvider.GetUtcNow());
        return await Task.FromResult(create);
    }

    public async Task<IEnumerable<Shelter>> ShelterList()
    {
        return await Task.FromResult(facade.ListShelters());
    }

    async Task<ShelteredPet> IShelterApiHandler.ListPet(string shelterId, ListPetModel listPetModel)
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

    private Shelter NewEmptyShelter(string name)
    {
        return new Shelter(new ShelterIdentity(Ulid.NewUlid()), new ShelterDetails(name, ""));
    }
}