using Core.Interface;
using Core.Interface.Events;

public class ShelterHistoryApiViewModel(IShelterHistoryFacade facade)
{
    public async Task<IEnumerable<ShelterEvent>?> GetHistoryById(string shelterId)
    {
        if (!Ulid.TryParse(shelterId, out var ulid))
        {
            return null;
        }

        var result = facade.GetShelterHistory(new(ulid));
        return await Task.FromResult(result);
    }

    public async Task<IEnumerable<ShelterEvent>?> GetHistoryEventTypeById(string shelterId, int eventKind)
    {
        if (!Ulid.TryParse(shelterId, out var ulid))
        {
            return null;
        }

        if (!Enum.IsDefined(typeof(ShelterEventKind), eventKind))
        {
            return null;
        }

        var result = facade.GetShelterHistoryByEventKind(new(ulid), (ShelterEventKind)eventKind);
        return await Task.FromResult(result);
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
