using Core.Interface.Events;
using Core.Interface.Models;

namespace Core.Interface
{
    public interface IShelterHistoryFacade
    {
        ShelterEvent GetShelterDateListedByShelter(ShelterIdentity shelterIdentity);
    }
}