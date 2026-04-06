using Core.Interface.Events;
using Core.Interface.Models;

namespace Core.Interface
{
    public interface IShelterHistoryFacade
    {
        IEnumerable<ShelterEvent> GetShelterHistory(ShelterIdentity shelterIdentity);
        IEnumerable<ShelterEvent> GetShelterHistoryByEventKind(ShelterIdentity shelterIdentity, ShelterEventKind eventKind);
        ShelterEvent GetShelterDateListedByShelter(ShelterIdentity shelterIdentity);
    }
}