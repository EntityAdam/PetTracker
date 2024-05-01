//public record NewPet(string Name, string Age, string PetKind);
//public record NewPetRecordOfDeath(string PetId, string PersonId, DateTime TimeOfDeath);
//public record NewPerson(string Name, int MaxPets);
//public record NewPetOwnerAssignment(string PetId, string PersonId);

//public interface IDomainFacade
//{
//    // assign a pet to an existing person
//    public void AssignPetOwner(NewPetOwnerAssignment newPetOwnerAssignment);

//    // create a person
//    public void CreatePerson(NewPerson newPerson);

//    // create a pet
//    public void CreatePet(NewPet newPet);

//    // record the pets passing
//    public void RecordPetDeath(NewPetRecordOfDeath newPetRecordOfDeath);

//    // remove a persons identifying information
//    // annonymize a persons identifying information in pet records
//    public void RemovePerson(string personId);
//}

//public class Facade : IDomainFacade
//{
//    private readonly IDataFacade dataFacade;

//    public Facade(IDataFacade petStore)
//    {
//        this.dataFacade = petStore;
//    }

//    public void AssignPetOwner(NewPetOwnerAssignment newPetOwnerAssignment)
//    {
//        if (newPetOwnerAssignment is null) throw new ArgumentNullException(nameof(newPetOwnerAssignment));
//        if (CanPetBeAssignedToNewOwner(newPetOwnerAssignment)) throw new InvalidOperationException("Unable to assign this pet to a new owner");
//        dataFacade.AssignPetOwner(newPetOwnerAssignment);
//    }

//    public void CreatePerson(NewPerson newPerson)
//    {
//        if (newPerson is null) throw new ArgumentNullException(nameof(newPerson));
//        dataFacade.CreatePerson(newPerson);
//    }

//    public void CreatePet(NewPet newPet)
//    {
//        if (newPet is null) throw new ArgumentNullException(nameof(newPet));
//        dataFacade.CreatePet(newPet);
//    }

//    public void RecordPetDeath(NewPetRecordOfDeath newPetRecordOfDeath)
//    {
//        if (newPetRecordOfDeath is null) throw new ArgumentNullException(nameof(newPetRecordOfDeath));
//        dataFacade.RecordPetDeath(newPetRecordOfDeath);
//    }

//    public void RemovePerson(string personId)
//    {
//        if (personId is null) throw new ArgumentNullException(nameof(personId));
//        dataFacade.AnnonymizePersonData(personId);
//    }

//    private bool CanPetBeAssignedToNewOwner(NewPetOwnerAssignment newPetOwnerAssignment)
//    {
//        dataFacade.GetPetOwnershipStatus(newPetOwnerAssignment);
//        return true;
//    }
//}

//public interface IDataFacade
//{
//    void AssignPetOwner(NewPetOwnerAssignment newPetOwnerAssignment);
//    void CreatePerson(NewPerson newPerson);
//    void CreatePet(NewPet newPet);
//    void RecordPetDeath(NewPetRecordOfDeath newPetRecordOfDeath);
//    void AnnonymizePersonData(string personId);
//    void GetPetOwnershipStatus(NewPetOwnerAssignment newPetOwnerAssignment);
//}

//public class DataFacadeInMemory : IDataFacade
//{
//    private readonly IDateTimeProvider dateTimeProvider;
//    private List<Person> People { get; set; } = new List<Person>();
//    private List<Pet> Pets { get; set; } = new List<Pet>();
//    private List<OwnershipEvent> OwnershipEvents { get; set; } = new List<OwnershipEvent>();

//    public DataFacadeInMemory(IDateTimeProvider dateTimeProvider)
//    {
//        this.dateTimeProvider = dateTimeProvider;
//    }

//    record Person(string Id, string Name, int MaxPets);
//    record Pet(string Id, string Name);
//    record OwnershipEvent(string Id, string PersonId, string PetId, OwnershipEventKind OwnershipEventKind, DateTime Timestamp);
//    record OwnershipStatus(int PersonMaxPets, int PersonCurrentPets, bool PetIsAlive, bool PetHasCurrentOwner, string? PetCurrentOwner);

//    enum OwnershipEventKind
//    {
//        AssignPetOwner,
//        RemovePetOwner,
//        PetDeath
//    }

//    public void CreatePerson(NewPerson newPerson)
//    {
//        People.Add(new Person(Guid.NewGuid().ToString(), newPerson.Name, newPerson.MaxPets));
//    }

//    public void CreatePet(NewPet newPet)
//    {
//        Pets.Add(new Pet(Guid.NewGuid().ToString(), newPet.Name));
//    }

//    public void AssignPetOwner(NewPetOwnerAssignment newPetOwnerAssignment)
//    {
//        CreateEvent(new OwnershipEvent(Guid.NewGuid().ToString(), newPetOwnerAssignment.PersonId, newPetOwnerAssignment.PetId, OwnershipEventKind.AssignPetOwner, dateTimeProvider.Now()));
//    }

//    public void RecordPetDeath(NewPetRecordOfDeath newPetRecordOfDeath)
//    {
//        // fetch personId if not provided? what if pet dies and doesn't have an owner?
//        // CreateEvent(new OwnershipEvent(Guid.NewGuid().ToString(), null, newPetRecordOfDeath.PetId, OwnershipEventKind.PetDeath, newPetRecordOfDeath.TimeOfDeath));
//        CreateEvent(new OwnershipEvent(Guid.NewGuid().ToString(), newPetRecordOfDeath.PersonId, newPetRecordOfDeath.PetId, OwnershipEventKind.PetDeath, newPetRecordOfDeath.TimeOfDeath));
//    }

//    public void AnnonymizePersonData(string personId)
//    {
//        var personToRedact = People.Single(p => p.Id == personId);
//        var personToAdd = personToRedact with { Name = "REDACTED", MaxPets = 0 };
//        People.Remove(personToRedact);
//        People.Add(personToAdd);
//        //TODO : Should any events be modified to further annonymize the person?
//    }

//    public void GetPetOwnershipStatus(NewPetOwnerAssignment newPetOwnerAssignment)
//    {
//        var person = People.Single(p => p.Id == newPetOwnerAssignment.PersonId);
//        var relatedEvents = OwnershipEvents.Where(e => e.PersonId == newPetOwnerAssignment.PersonId);
//        var isAlive = !relatedEvents.Any(e => e.OwnershipEventKind == OwnershipEventKind.PetDeath);
//        var allPersonAssignedPets = relatedEvents.Where(e => e.OwnershipEventKind == OwnershipEventKind.AssignPetOwner);
//        var allPersonRemovedPets = relatedEvents.Where(e => e.OwnershipEventKind == OwnershipEventKind.RemovePetOwner);
//        var allCurrentPets = allPersonAssignedPets.ExceptBy(allPersonRemovedPets.Select(x => x.PetId), x => x.PetId);
//        var lastOwnerEvent = relatedEvents.LastOrDefault(e => e.OwnershipEventKind == OwnershipEventKind.AssignPetOwner);
//        var lastUnassignedEvent = relatedEvents.LastOrDefault(e => e.OwnershipEventKind == OwnershipEventKind.RemovePetOwner);
//        var hasCurrentOwner = lastUnassignedEvent.Timestamp > lastOwnerEvent.Timestamp;
//        string? lastOwnerId = null;
//        if (hasCurrentOwner)
//        {
//            lastOwnerId = lastOwnerEvent.PersonId;
//        }
//        var x = new OwnershipStatus(person.MaxPets, allCurrentPets.Count(), isAlive, hasCurrentOwner, lastOwnerId);
//    }

//    private void CreateEvent(OwnershipEvent ownershipEvent)
//    {
//        OwnershipEvents.Add(ownershipEvent);
//    }
//}

//public interface IDateTimeProvider
//{
//    DateTime Now();
//}
//public class DateTimeProvider : IDateTimeProvider
//{
//    public DateTime Now() => DateTime.UtcNow;
//}