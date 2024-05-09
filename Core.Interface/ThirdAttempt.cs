//using System.Diagnostics.CodeAnalysis;

//namespace Core.Interface;

//public record PetDetails(string Name);
//public class PetIdentityV1
//{
//    //ulid
//    //microchips
//    //
//}
//public record PetMicrochip(string Id, MicrochipFrequencyKind MicrochipFrequencyKind);
//public enum MicrochipFrequencyKind
//{
//    ISO
//}

//public class NewPet
//{
//    private const string shortCode = "PT";

//    public NewPet(Ulid id, PetDetails name)
//    {
//        this.Id = id;
//        this.Name = name;
//    }

//    public Ulid Id { get; } // primary key
//    public string ShortCode => $"{shortCode}-{Id}"; // public

//    public PetDetails Name { get; }
//    public PetMicrochip[] Microchips { get; } = Array.Empty<PetMicrochip>();
    
//    public static NewPet CreateNew(string name) => CreateNew(new PetDetails(name));
//    public static NewPet CreateNew(PetDetails name) => new(Ulid.NewUlid(), name);
//}

///// <summary>
///// Many shelters may share a name and differ by location
///// </summary>
///// <param name="Name"></param>
//public record ShelterIdentity(string Name);

///// <summary>
///// Shelters have a physical address
///// </summary>
///// <param name="Address"></param>
//public record ShelterDetails(string Address);

///// <summary>
///// Shelters internal identifier
///// I'm thinking of using ULID: https://github.com/Cysharp/Ulid
///// </summary>
///// <param name="Id"></param>
//public class ShelterPublicShortCode
//{
//    private const string shortCode = "SH";

//    public ShelterPublicShortCode(Ulid Id)
//    {
//        this.Id = Id;
//    }

//    public Ulid Id { get; }
//    public string ShortCode => $"{shortCode}-{Id}";
//    public static ShelterPublicShortCode CreateNew() => new(Ulid.NewUlid());
//}

//public record ShelterEvent(ShelterIdentity Shelter, ShelterEventKind ShelterEventKind, DateTime Timestamp);
//public record ShelteredPet(NewPet Pet, ShelterIdentity Shelter); //Aggregate
//public record ShelteredPetEvent(ShelteredPet Pet, PetEventKind PetEventKind, DateTime Timestamp);


//public record NewRescueGroup(string Name);
//public record RescueGroupPublicShortCode(string Id);
//public record RescueGroupEvent();
//public record RecuedPet(NewPet Pet, ShelterIdentity Shelter);
//public record RescuedPetEvent();


//public record PersonIdentity(string Name, int MaxPets);
//public record FosteredPet(ShelteredPet Pet, PersonIdentity Person);
//public record FosterPetAssignment(ShelteredPet Pet, PersonIdentity Person, DateTime TimeOfAssignment);
//public record FosterPersonEvent(PersonIdentity Person, FosterPersonEventKind FosterPersonEventKind, DateTime Timestamp);

//public class AdoptAggregate
//{
//    public AdoptAggregate(ShelteredPet pet, AdopterPerson person)
//    {
//    }

//    public void AssignPetToAdopterPerson() {  }
//}
//public record AdopterPerson(string Name);
//public record AdoptedPet(ShelteredPet Pet, AdopterPerson Person);
//public record AdopterPetAssignment(ShelteredPet Pet, AdopterPerson Person, DateTime TimeOfAssignment);
//public record AdopterPersonEvent(AdopterPerson Person, AdopterPersonEventKind AdopterPersonEventKind, DateTime Timestamp);


//public enum PetEventKind
//{
//    Adopted,
//    ReturnedFromAdopter,
//    Fostered,
//    ReturnedFromFoster,
//    ListedAtShelter,
//    TransferredToAnotherShelter,
//    TransferredFromAnotherShelter,
//    Sponsored
//}

//public enum FosterPersonEventKind
//{
//    Adopted,
//    ReturnedAdoptedPet,
//    Fostered,
//    ReturnedFosterPet,
//    SponsoredShelter,
//    SponsoredPet,
//    FosterPersonJoin
//}

//public enum AdopterPersonEventKind
//{
//    Adopted,
//    ReturnedAdoptedPet,
//    SponsoredShelter,
//    SponsoredPet,
//    AdopterPersonJoin
//}

//public enum ShelterEventKind
//{
//    PetAdopted,
//    PetFostered,
//    PetListed,
//    PetTransferredAway,
//    PetTransferredHere,
//    ShelterListed,
//    PetReturnedFromFoster,
//    PetReturnedFromAdopter,
//    Sponsored
//}

////public interface IDomainFacade
////{
////    // Shelter
////    public void ShelterCreate(NewShelter shelter, DateTime timestamp);
////    public void ShelterAddPet(ShelteredPet pet, DateTime timestamp);
////    public void ShelterTransferPet(ShelteredPet pet, NewShelter shelter);
////    public void ShelterRemoveAndObfuscateData(NewShelter shelter);
////    public void ShelterUnlistPet(ShelteredPet pet);

////    // Adopt
////    public void PersonOpenToAdoption(AdopterPerson person);
////    public void AdopterPetAssign(AdopterPetAssignment assignment);
////    public void RemoveAdopterPerson(AdopterPerson person);

////    // Foster
////    public void CreateFosterPerson(PersonIdentity person, DateTime timestamp);
////    public void FosterPetAssign(FosterPetAssignment assignment);
////    public void RemoveFosterPerson(PersonIdentity person);

////    // Anonymous
////    public int GetPetsListedCountByShelter(NewShelter shelter);
////}

//public class Facade : IDomainFacade, IAnonymousFacade, IShelterFacade, IAdopterPersonFacade, IFosterPersonFacade
//{
//    private readonly IDataFacade data;
//    private readonly IHistoryProvider history;

//    public Facade(IDataFacade data, IHistoryProvider history)
//    {
//        this.data = data;
//        this.history = history;
//    }

//    public void ShelterCreate(ShelterIdentity shelter, DateTime timestamp)
//    {
//        data.CreateShelter(shelter);
//        history.NewShelterAddedEvent(shelter, timestamp);
//    }

//    public void ShelterRemoveAndObfuscateData(ShelterIdentity shelter)
//    {
//        data.RemoveShelter(shelter);
//        history.RemoveShelterHistory(shelter);
//    }

//    public void ShelterTransferPet(ShelteredPet pet, ShelterIdentity shelter)
//    {
//        data.TransferPet(pet, shelter);
//        history.ShelteredPetEventTransferred(pet, shelter);
//    }

//    public void ShelterAssignFosterPerson(FosterPetAssignment assignment)
//    {
//        var (pet, person, timestamp) = assignment;
//        data.FosterPetAssign(assignment);
//        history.ShelteredPetEventFostered(pet, person, timestamp);
//    }

//    public void PersonOpenToFoster(PersonIdentity person, DateTime timestamp)
//    {
//        data.CreateFosterPerson(person);
//        history.PersonOpenToFoster(person, timestamp);
//    }

//    public void ShelterAddPet(ShelteredPet pet, DateTime timestamp)
//    {
//        data.ListShelteredPet(pet);
//        history.ShelteredPetEventListed(pet, timestamp);
//    }

//    public void FosterPersonRemove(PersonIdentity person)
//    {
//        data.RemoveFosterPerson(person);
//        history.RemoveFosterHistory(person);
//    }

//    public void PersonOpenToAdoption(AdopterPerson person)
//    {
//        data.PersonOpenToAdoption(person);
//    }

//    public void ShelterAssignAdopterPerson(AdopterPetAssignment assignment)
//    {
//        var (pet, person, timestamp) = assignment;
//        data.AdopterPetAssign(assignment);
//        history.ShelteredPetEventAdopted(pet, person, timestamp);
//    }

//    public void AdopterPersonRemove(AdopterPerson person)
//    {
//        data.RemoveAdopterPerson(person);
//        history.RemoveAdopterHistory(person);
//    }

//    public void ShelterUnlistPet(ShelteredPet pet)
//    {
//        data.UnlistShelteredPet(pet);
//        history.RemovePetHistory(pet, pet.Shelter);
//    }

//    public IEnumerable<ShelteredPetEvent> GetPetHistory(ShelteredPet pet)
//    {
//        return history.GetPetHistory(pet);
//    }

//    public IEnumerable<ShelterEvent> GetShelterDateListedByShelter(ShelterIdentity shelter)
//    {
//        return history.GetShelterDateListedByShelter(shelter);
//    }

//    public int GetPetsFosteredCountByShelter(ShelterIdentity shelter)
//    {
//        return history.GetPetsFosteredCountByShelter(shelter);
//    }

//    public IEnumerable<FosterPersonEvent> GetFosterPersonHistory(PersonIdentity person)
//    {
//        return history.GetFosterPersonHistory(person);
//    }

//    public int GetPetsListedCountByShelter(ShelterIdentity shelter)
//    {
//        return history.GetPetsListedCountByShelter(shelter);
//    }

//    public int GetPetsAdoptedCountByShelter(ShelterIdentity shelter)
//    {
//        return history.GetPetsAdoptedCountByShelter(shelter);
//    }
//}

//public interface IDataFacade
//{
//    void CreateShelter(ShelterIdentity shelter);
//    void RemoveShelter(ShelterIdentity shelter);
//    void ListShelteredPet(ShelteredPet pet);
//    void TransferPet(ShelteredPet pet, ShelterIdentity shelter);
//    void UnlistShelteredPet(ShelteredPet pet);


//    void CreateFosterPerson(PersonIdentity person);
//    void FosterPetAssign(FosterPetAssignment assignment);
//    void RemoveFosterPerson(PersonIdentity person);

//    void PersonOpenToAdoption(AdopterPerson person);
//    void AdopterPetAssign(AdopterPetAssignment assignment);
//    void RemoveAdopterPerson(AdopterPerson person);
//}

//public interface IHistoryProvider
//{
//    public void ShelteredPetEventListed(ShelteredPet pet, DateTime timestamp);
//    public void ShelteredPetEventFostered(ShelteredPet pet, PersonIdentity person, DateTime timestamp);
//    public void ShelteredPetEventAdopted(ShelteredPet pet, AdopterPerson person, DateTime timestamp);
//    public void ShelteredPetEventTransferred(ShelteredPet pet, ShelterIdentity shelter);


//    public void RemovePetHistory(ShelteredPet pet, ShelterIdentity shelter);

//    public int GetPetsListedCountByShelter(ShelterIdentity shelter);
//    public int GetPetsFosteredCountByShelter(ShelterIdentity shelter);
//    public int GetPetsAdoptedCountByShelter(ShelterIdentity shelter);

//    public IEnumerable<ShelteredPetEvent> GetPetHistory(ShelteredPet pet);

//    public void NewShelterAddedEvent(ShelterIdentity shelter, DateTime timestamp);
//    public void RemoveShelterHistory(ShelterIdentity shelter);
//    public void RemoveAdopterHistory(AdopterPerson person);
//    public void RemoveFosterHistory(PersonIdentity person);
//    public void PersonOpenToFoster(PersonIdentity person, DateTime timestamp);
//    IEnumerable<ShelterEvent> GetShelterDateListedByShelter(ShelterIdentity person);
//    IEnumerable<FosterPersonEvent> GetFosterPersonHistory(PersonIdentity person);
//}

//public class HistoryProviderInMemeory : IHistoryProvider
//{
//    private readonly IDateTimeProvider dateTimeProvider;

//    private List<AdopterPersonEvent> AdopterPersonEvents { get; set; } = new List<AdopterPersonEvent>();
//    private List<FosterPersonEvent> FosterPersonEvents { get; set; } = new List<FosterPersonEvent>();
//    private List<ShelterEvent> ShelterEvents { get; set; } = new List<ShelterEvent>();
//    private List<ShelteredPetEvent> PetEvents { get; set; } = new List<ShelteredPetEvent>();

//    public HistoryProviderInMemeory(IDateTimeProvider dateTimeProvider)
//    {
//        this.dateTimeProvider = dateTimeProvider;
//    }

//    public void ShelteredPetEventAdopted(ShelteredPet pet, AdopterPerson person, DateTime timestamp)
//    {
//        PetEvents.Add(new(pet, PetEventKind.Adopted, timestamp));
//        ShelterEvents.Add(new(pet.Shelter, ShelterEventKind.PetAdopted, timestamp));
//        AdopterPersonEvents.Add(new(person, AdopterPersonEventKind.Adopted, timestamp));
//    }

//    public void NewShelterAddedEvent(ShelterIdentity shelter, DateTime timestamp)
//    {
//        ShelterEvents.Add(new(shelter, ShelterEventKind.ShelterListed, timestamp));
//    }

//    public void ShelteredPetEventFostered(ShelteredPet pet, PersonIdentity person, DateTime timestamp)
//    {
//        FosterPersonEvents.Add(new(person, FosterPersonEventKind.Fostered, timestamp));
//        PetEvents.Add(new(pet, PetEventKind.Fostered, timestamp));
//        ShelterEvents.Add(new(pet.Shelter, ShelterEventKind.PetFostered, timestamp));
//    }

//    public IEnumerable<ShelteredPetEvent> GetPetHistory(ShelteredPet pet)
//    {
//        return PetEvents.Where(e => e.Pet == pet);
//    }

//    public void ShelteredPetEventListed(ShelteredPet pet, DateTime timestamp)
//    {
//        PetEvents.Add(new(pet, PetEventKind.ListedAtShelter, timestamp));
//        ShelterEvents.Add(new(pet.Shelter, ShelterEventKind.PetListed, timestamp));
//    }

//    public void RemoveAdopterHistory(AdopterPerson person)
//    {
//        AdopterPersonEvents.RemoveAll(e => e.Person == person);
//    }

//    public void RemoveFosterHistory(PersonIdentity person)
//    {
//        FosterPersonEvents.RemoveAll(e => e.Person == person);
//    }

//    public void RemoveShelterHistory(ShelterIdentity shelter)
//    {
//        ShelterEvents.RemoveAll(e => e.Shelter == shelter);
//    }

//    public void ShelteredPetEventTransferred(ShelteredPet pet, ShelterIdentity shelter)
//    {
//        PetEvents.Add(new(pet, PetEventKind.TransferredToAnotherShelter, dateTimeProvider.GetCurrentDateTime()));
//        ShelterEvents.Add(new(shelter, ShelterEventKind.PetTransferredAway, dateTimeProvider.GetCurrentDateTime()));
//    }

//    public void PersonOpenToFoster(PersonIdentity person, DateTime timestamp)
//    {
//        FosterPersonEvents.Add(new(person, FosterPersonEventKind.FosterPersonJoin, timestamp));
//    }

//    public void RemovePetHistory(ShelteredPet pet, ShelterIdentity shelter)
//    {
//        PetEvents.RemoveAll(p => p.Pet == pet);
//        ShelterEvents.RemoveAll(s => s.Shelter == shelter);
//    }

//    public int GetPetsFosteredCountByShelter(ShelterIdentity s1)
//    {
//        return ShelterEvents.Count(e => e.ShelterEventKind == ShelterEventKind.PetFostered);
//    }

//    public IEnumerable<ShelterEvent> GetShelterDateListedByShelter(ShelterIdentity person)
//    {
//        return ShelterEvents.Where(e => e.ShelterEventKind == ShelterEventKind.ShelterListed);
//    }

//    public IEnumerable<FosterPersonEvent> GetFosterPersonHistory(PersonIdentity person)
//    {
//        return FosterPersonEvents.Where(p => p.Person == person);
//    }

//    public int GetPetsListedCountByShelter(ShelterIdentity shelter)
//    {
//        return ShelterEvents.Count(e => e.ShelterEventKind == ShelterEventKind.PetListed);
//    }

//    public int GetPetsAdoptedCountByShelter(ShelterIdentity shelter)
//    {
//        return ShelterEvents.Count(e => e.ShelterEventKind == ShelterEventKind.PetAdopted);
//    }
//}

//public class DataFacadeInMemory : IDataFacade
//{
//    private readonly IDateTimeProvider dateTimeProvider;

//    private List<AdopterPerson> AdopterPeople { get; set; } = new List<AdopterPerson>();
//    private List<PersonIdentity> FosterPeople { get; set; } = new List<PersonIdentity>();
//    private List<ShelterIdentity> Shelters { get; set; } = new List<ShelterIdentity>();
//    private List<ShelteredPet> ShelteredPets { get; set; } = new List<ShelteredPet>();
//    private List<FosteredPet> FosteredPets { get; set; } = new List<FosteredPet>();
//    private List<AdoptedPet> AdoptedPets { get; set; } = new List<AdoptedPet>();

//    public DataFacadeInMemory(IDateTimeProvider dateTimeProvider)
//    {
//        this.dateTimeProvider = dateTimeProvider;
//    }

//    public void CreateFosterPerson(PersonIdentity person)
//    {
//        if (FosterPeople.Exists(p => p == person)) throw new InvalidOperationException("Person already exists");
//        FosterPeople.Add(person);
//    }

//    public void CreateShelter(ShelterIdentity shelter)
//    {
//        if (Shelters.Exists(p => p == shelter)) throw new InvalidOperationException("Shelter already exists");
//        Shelters.Add(shelter);
//    }

//    public void ListShelteredPet(ShelteredPet pet)
//    {
//        if (ShelteredPets.Exists(p => p == pet)) throw new InvalidOperationException("Pet already exists");
//        if (!Shelters.Exists(s => s == pet.Shelter)) throw new InvalidOperationException("Developer Guard: The shelter does not exist");
//        ShelteredPets.Add(pet);
//    }

//    public void UnlistShelteredPet(ShelteredPet pet)
//    {
//        if (!ShelteredPets.Exists(p => p == pet)) throw new InvalidOperationException("Pet does not exist");
//        ShelteredPets.Remove(pet);
//    }

//    public void FosterPetAssign(FosterPetAssignment assignment)
//    {
//        var (pet, person, timestamp) = assignment;
//        if (AdoptedPets.Exists(p => p.Pet == pet)) throw new InvalidOperationException("Adopted pets can't be assigned to a foster"); //shouldnt need this, wouldnt be listed as available pets
//        if (FosteredPets.Exists(p => p.Pet == pet)) throw new InvalidOperationException("Pet is already fostered"); //shouldnt need this, wouldnt be listed as available pets
//        ShelteredPets.Remove(pet);
//        FosteredPets.Add(new(pet, person));
//    }

//    public void RemoveShelter(ShelterIdentity shelter)
//    {
//        if (!Shelters.Any(s => s == shelter)) throw new InvalidOperationException("Shelter does not exist");
//        if (ShelteredPets.Any(p => p.Shelter == shelter)) throw new InvalidOperationException("Shelter is not empty");
//        Shelters.Remove(shelter);
//    }

//    public void TransferPet(ShelteredPet pet, ShelterIdentity shelter)
//    {
//        ShelterIdentity originalShelter = pet.Shelter;
//        if (originalShelter == shelter) throw new InvalidOperationException("Origin shelter is target shelter");
//        if (FosteredPets.Exists(p => p.Pet == pet)) throw new InvalidOperationException("Pet is already fostered");
//        if (AdoptedPets.Exists(p => p.Pet == pet)) throw new InvalidOperationException("Pet is already adopted");
//        ShelteredPet targetPet = pet with { Shelter = shelter };
//        ShelteredPets.Remove(pet);
//        ShelteredPets.Add(targetPet);
//    }

//    public void PersonOpenToAdoption(AdopterPerson person)
//    {
//        AdopterPeople.Add(person);
//    }

//    public void AdopterPetAssign(AdopterPetAssignment assignment)
//    {
//        var (pet, person, timestamp) = assignment;
//        if (AdoptedPets.Exists(p => p.Pet == pet)) throw new InvalidOperationException("Pet is already adopted"); //shouldnt need this, wouldnt be listed as available pets
//        if (FosteredPets.Exists(p => p.Pet == pet)) throw new InvalidOperationException("Pet is currently fostered"); //shouldnt need this, wouldnt be listed as available pets
//        ShelteredPets.Remove(pet);
//        AdoptedPets.Add(new(pet, person));
//    }

//    public void RemoveAdopterPerson(AdopterPerson person)
//    {
//        AdoptedPets.RemoveAll(p => p.Person == person);
//        AdopterPeople.Remove(person);
//    }

//    public void RemoveFosterPerson(PersonIdentity person)
//    {
//        if (FosteredPets.Any(p => p.Person == person)) throw new InvalidOperationException("Foster person is in custody of pets that must be returned");
//        FosterPeople.Remove(person);
//    }
//}

//public interface IDateTimeProvider
//{
//    DateTime GetCurrentDateTime();
//}

//public class DateTimeProvider : IDateTimeProvider
//{
//    public DateTime GetCurrentDateTime() => DateTime.UtcNow;
//}

//[ExcludeFromCodeCoverage]
//public class DateTimeProviderMock : IDateTimeProvider
//{
//    internal DateTime CurrentDateTime { get; set; }

//    public DateTime GetCurrentDateTime() => CurrentDateTime;
//}

//[ExcludeFromCodeCoverage]
//public record ValidatorProblem(bool IsValid, string Reason);

//[ExcludeFromCodeCoverage]
//public abstract class Validator<T>
//{
//    public abstract bool IsValid(T type);
//    public abstract IEnumerable<ValidatorProblem> Validate(T type);
//}

//[ExcludeFromCodeCoverage]
//public class ValidateCreatePet<NewPet> : Validator<NewPet>
//{
//    public override bool IsValid(NewPet type)
//    {
//        return !Validate(type).Any();
//    }

//    public override IEnumerable<ValidatorProblem> Validate(NewPet type)
//    {
//        return Enumerable.Empty<ValidatorProblem>();
//    }
//}