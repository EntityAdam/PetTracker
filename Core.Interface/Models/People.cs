namespace Core.Interface.Models;

// Value Objects
public record PersonIdentity(Ulid Id)
{
    public static PersonIdentity CreateNew() => new(Ulid.NewUlid());
}

public record FosterPersonDetails(string Name, int MaxPets)
{
    public static FosterPersonDetails CreateDefaultWithDetails(string personName, int maxPets) => new(personName, maxPets);
}

public record AdopterPersonDetails(string Name)
{
    public static AdopterPersonDetails CreateDefaultWithName(string personName) => new(personName);
}

// Domain Objects
public record AdopterPerson(PersonIdentity Id, AdopterPersonDetails adopterPersonDetails)
{
    public static AdopterPerson CreateNewWithName(string personName) => new(PersonIdentity.CreateNew(), AdopterPersonDetails.CreateDefaultWithName(personName));
}

public record FosterPerson(PersonIdentity Id, FosterPersonDetails fosterPersonDetails);


// 1:1 domain objects
public record FosteredPet(ShelteredPet Pet, PersonIdentity PersonIdentity);
public record AdoptedPet(ShelteredPet Pet, PersonIdentity PersonIdentity);


// Commands
public record FosterPetAssignment(ShelteredPet Pet, PersonIdentity PersonIdentity, DateTimeOffset TimeOfAssignment);
public record AdopterPetAssignment(ShelteredPet Pet, PersonIdentity PersonIdentity, DateTimeOffset TimeOfAssignment);
