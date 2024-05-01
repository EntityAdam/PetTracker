//public record NewPet(string Name, string Age, string PetKind);
//public record NewPetRecordOfDeath(string PetId, DateTime DateTime);
//public record NewPerson(string Name, string Age);
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
//    private readonly IPetStore petStore;
//    private readonly IPeopleStore peopleStore;
//    private readonly IPetOwnerStore petOwnerStore;

//    public Facade(IPetStore petStore, IPeopleStore peopleStore, IPetOwnerStore petOwnerStore)
//    {
//        this.petStore = petStore;
//        this.peopleStore = peopleStore;
//        this.petOwnerStore = petOwnerStore;
//    }
//    public void AssignPetOwner(NewPetOwnerAssignment newPetOwnerAssignment)
//    {
//        if (newPetOwnerAssignment is null) throw new ArgumentNullException(nameof(newPetOwnerAssignment));
//        if (!peopleStore.CanAssignNewPet(newPetOwnerAssignment.PersonId)) throw new InvalidOperationException("Unable to assign any pet to this person");
//        if (!petStore.CanBeAssignedToNewOwner(newPetOwnerAssignment.PetId)) throw new InvalidOperationException("Unable to assign this pet to a new owner");
//        petOwnerStore.AssignPetOwner(newPetOwnerAssignment);
//    }

//    public void CreatePerson(NewPerson newPerson)
//    {
//        if (newPerson is null) throw new ArgumentNullException(nameof(newPerson));
//        peopleStore.CreatePerson(newPerson);
//    }

//    public void CreatePet(NewPet newPet)
//    {
//        if (newPet is null) throw new ArgumentNullException(nameof(newPet));
//        petStore.CreatePet(newPet);
//    }

//    public void RecordPetDeath(NewPetRecordOfDeath newPetRecordOfDeath)
//    {
//        if (newPetRecordOfDeath is null) throw new ArgumentNullException(nameof(newPetRecordOfDeath));
//        petStore.RecordPetDeath(newPetRecordOfDeath);
//    }

//    public void RemovePerson(string personId)
//    {
//        if (personId is null) throw new ArgumentNullException(nameof(personId));
//        petOwnerStore.AnnonymizeOwner(personId);
//        peopleStore.RemovePerson(personId);
//    }
//}


//public interface IPeopleStore
//{
//    bool CanAssignNewPet(string personId);
//    void CreatePerson(NewPerson newPerson);
//    void RemovePerson(string personId);
//}

//public interface IPetStore
//{
//    bool CanBeAssignedToNewOwner(string petId);
//    void CreatePet(NewPet newPet);
//    void RecordPetDeath(NewPetRecordOfDeath newPetRecordOfDeath);
//}

//public interface IPetOwnerStore
//{
//    void AnnonymizeOwner(string personId);
//    void AssignPetOwner(NewPetOwnerAssignment newPetOwnerAssignment);
//}
