using Core.Interface;
using Core.Interface.Events;

public class ShelterHistoryApiViewModel(IShelterHistoryFacade facade)
{
    public Task<IEnumerable<ShelterEvent>> GetHistoryById(string shelterId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ShelterEvent>> GetHistoryEventTypeById(string shelterId, int eventKind)
    {
        throw new NotImplementedException();
    }

    public async Task<ShelterEvent?> GetListedDate(string shelterId)
    {
        if (!Ulid.TryParse(shelterId, out var ulid))
        {
            return null;
        }

        try
        {
            var result = facade.GetShelterDateListedByShelter(new(ulid));
            return await Task.FromResult(result);
        }
        catch (InvalidOperationException)
        {
            return null;
        }

    }
}
