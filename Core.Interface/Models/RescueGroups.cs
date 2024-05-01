namespace Core.Interface.Models;

// domain objects

public record NewRescueGroup(string Name);
public record RescuedPet(Pet Pet, ShelterIdentity Shelter);

// value objects
