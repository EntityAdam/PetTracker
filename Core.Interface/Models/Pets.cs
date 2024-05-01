namespace Core.Interface.Models;


// value objects
public record PetIdentity(Ulid Id, PetMicrochip[]? PetMicrochips)
{
    public static PetIdentity CreateNew() => new(Ulid.NewUlid(), Array.Empty<PetMicrochip>());
}

public record PetMicrochip(string Id, MicrochipFrequencyKind MicrochipFrequencyKind);

public record PetDetails(string Name)
{
    public static PetDetails CreateDefaultWithName(string petName) => new(petName);
}

public enum MicrochipFrequencyKind
{
    ISO
}

// domain objects
public record Pet(PetIdentity Id, PetDetails Name)
{
    public static Pet CreateNewWithName(string petName)
    {
        return new Pet(PetIdentity.CreateNew(), PetDetails.CreateDefaultWithName(petName));
    }
}