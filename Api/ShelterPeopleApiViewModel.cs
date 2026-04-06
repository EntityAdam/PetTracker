using Core.Interface;
using Core.Interface.Events;
using Core.Interface.Models;

public class ShelterPeopleApiViewModel(IDomainFacade domainFacade, IShelterPetsFacade shelterPetsFacade, TimeProvider timeProvider)
{
    public async Task<FosterPerson?> CreateFosterPerson(CreateFosterPersonModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Name) || model.MaxPets <= 0)
        {
            return null;
        }

        var fosterPerson = new FosterPerson(
            PersonIdentity.CreateNew(),
            FosterPersonDetails.CreateDefaultWithDetails(model.Name, model.MaxPets));

        try
        {
            domainFacade.PersonOpenToFoster(fosterPerson, timeProvider.GetUtcNow());
        }
        catch (InvalidOperationException)
        {
            return null;
        }

        return await Task.FromResult(fosterPerson);
    }

    public async Task<AdopterPerson?> CreateAdopterPerson(CreateAdopterPersonModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Name))
        {
            return null;
        }

        var adopterPerson = AdopterPerson.CreateNewWithName(model.Name);

        try
        {
            domainFacade.PersonOpenToAdoption(adopterPerson, timeProvider.GetUtcNow());
        }
        catch (InvalidOperationException)
        {
            return null;
        }

        return await Task.FromResult(adopterPerson);
    }

    public async Task<ShelteredPetEvent?> AssignToFosterPerson(string shelterId, string petId, string fosterPersonId)
    {
        if (!TryParseIds(shelterId, petId, out var shelterIdentity, out var petIdentity))
        {
            return null;
        }

        if (!Ulid.TryParse(fosterPersonId, out var fosterPersonUlid))
        {
            return null;
        }

        var shelteredPet = shelterPetsFacade.GetShelteredPetDetails(shelterIdentity, petIdentity);
        if (shelteredPet is null)
        {
            return null;
        }

        try
        {
            var assignment = new FosterPetAssignment(shelteredPet, new PersonIdentity(fosterPersonUlid), timeProvider.GetUtcNow());
            domainFacade.ShelterAssignFosterPerson(assignment);
        }
        catch (InvalidOperationException)
        {
            return null;
        }

        var result = domainFacade.GetPetHistory(petIdentity).LastOrDefault();
        return await Task.FromResult(result);
    }

    public async Task<ShelteredPetEvent?> AssignToAdopterPerson(string shelterId, string petId, string adopterPersonId)
    {
        if (!TryParseIds(shelterId, petId, out var shelterIdentity, out var petIdentity))
        {
            return null;
        }

        if (!Ulid.TryParse(adopterPersonId, out var adopterPersonUlid))
        {
            return null;
        }

        var shelteredPet = shelterPetsFacade.GetShelteredPetDetails(shelterIdentity, petIdentity);
        if (shelteredPet is null)
        {
            return null;
        }

        try
        {
            var assignment = new AdopterPetAssignment(shelteredPet, new PersonIdentity(adopterPersonUlid), timeProvider.GetUtcNow());
            domainFacade.ShelterAssignAdopterPerson(assignment);
        }
        catch (InvalidOperationException)
        {
            return null;
        }

        var result = domainFacade.GetPetHistory(petIdentity).LastOrDefault();
        return await Task.FromResult(result);
    }

    public async Task<ShelteredPetEvent?> RecordOutcome(string shelterId, string petId, OutcomeKind outcomeKind)
    {
        if (!TryParseIds(shelterId, petId, out var shelterIdentity, out var petIdentity))
        {
            return null;
        }

        var shelteredPet = shelterPetsFacade.GetShelteredPetDetails(shelterIdentity, petIdentity);
        if (shelteredPet is null)
        {
            return null;
        }

        try
        {
            domainFacade.ShelterRecordOutcome(shelteredPet, outcomeKind, timeProvider.GetUtcNow());
        }
        catch (InvalidOperationException)
        {
            return null;
        }

        var result = domainFacade.GetPetHistory(petIdentity).LastOrDefault();
        return await Task.FromResult(result);
    }

    public async Task<IEnumerable<FosterPersonEvent>?> GetFosterPersonHistory(string fosterPersonId)
    {
        if (!Ulid.TryParse(fosterPersonId, out var fosterPersonUlid))
        {
            return null;
        }

        var personIdentity = new PersonIdentity(fosterPersonUlid);
        var result = domainFacade.GetFosterPersonHistory(personIdentity);
        return await Task.FromResult(result);
    }

    public async Task<IEnumerable<FosterPersonEvent>?> GetFosterPersonHistoryByEventKind(string fosterPersonId, int eventKind)
    {
        if (!Ulid.TryParse(fosterPersonId, out var fosterPersonUlid))
        {
            return null;
        }

        if (!Enum.IsDefined(typeof(FosterPersonEventKind), eventKind))
        {
            return null;
        }

        var personIdentity = new PersonIdentity(fosterPersonUlid);
        var result = domainFacade
            .GetFosterPersonHistory(personIdentity)
            .Where(x => x.FosterPersonEventKind == (FosterPersonEventKind)eventKind)
            .ToList();

        return await Task.FromResult<IEnumerable<FosterPersonEvent>>(result);
    }

    private static bool TryParseIds(string shelterId, string petId, out ShelterIdentity shelterIdentity, out PetIdentity petIdentity)
    {
        shelterIdentity = new ShelterIdentity(Ulid.Empty);
        petIdentity = new PetIdentity(Ulid.Empty, null);

        if (!Ulid.TryParse(shelterId, out var shelterUlid) || !Ulid.TryParse(petId, out var petUlid))
        {
            return false;
        }

        shelterIdentity = new ShelterIdentity(shelterUlid);
        petIdentity = new PetIdentity(petUlid, null);
        return true;
    }
}
