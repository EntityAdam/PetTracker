namespace Core.Interface.Models;

// domain objects
public record Shelter(ShelterIdentity Id, ShelterDetails ShelterDetails);

// 1:1 domain objects
public record ShelteredPet(Pet Pet, ShelterIdentity ShelterIdentity);

// value objects
public record ShelterIdentity(Ulid Id);
public record ShelterDetails(string DisplayName, string Address);