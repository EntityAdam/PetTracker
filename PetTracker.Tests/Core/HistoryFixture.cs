using Core.Interface.Models;
using Microsoft.Extensions.Time.Testing;
using System.Diagnostics.CodeAnalysis;

namespace PetTracker.Tests;

[ExcludeFromCodeCoverage]
public class HistoryFixture
{
    public FakeTimeProvider Date { get; }
    public Facade Facade { get; }
    
    public HistoryFixture()
    {
        this.Date = new FakeTimeProvider();
        this.Facade = new Facade(new DataFacadeInMemory(Date), new HistoryProviderInMemeory(Date));
    }

    private ShelterIdentity NewShelterIdentity() => new(Ulid.NewUlid());
    private ShelterDetails NewShelterDetails() => new("", "");
    private ShelterDetails NewShelterDetails(string name) => new(name, "");
    private Shelter NewShelter() => new(NewShelterIdentity(), NewShelterDetails());
    private Shelter NewShelter(string name) => new(NewShelterIdentity(), NewShelterDetails(name));

    private PetIdentity NewPetIdentity() => new(Ulid.NewUlid(), Array.Empty<PetMicrochip>());
    private PetDetails NewPetDetails() => new("");
    private PetDetails NewPetDetails(string name) => new(name);
    private Pet NewPet() => new(NewPetIdentity(), NewPetDetails());
    private Pet NewPet(string name) => new(NewPetIdentity(), NewPetDetails(name));
    
    public ShelteredPet NewShelteredPet() => new(NewPet(), NewShelterIdentity());
    public ShelteredPet NewShelteredPet(string petName) => new(NewPet(petName), NewShelterIdentity());
    public ShelteredPet NewShelteredPet(string petName, ShelterIdentity shelterIdentity) => new(NewPet(petName), shelterIdentity);
    public ShelteredPet NewShelteredPet(string petName, Shelter shelter) => new(NewPet(petName), shelter.Id);

    private FosterPerson NewFosterPerson(string personName, int maxPets) => new(PersonIdentity.CreateNew(), FosterPersonDetails.CreateDefaultWithDetails(personName, maxPets));
    private AdopterPerson NewAdopterPerson(string personName) => AdopterPerson.CreateNewWithName(personName);


    public Shelter Seed_OneShelter()
    {
        Date.SetUtcNow(new DateTime(2020, 1, 5));
        var shelter1 = NewShelter("ShelterA");
        Facade.ShelterCreate(shelter1, Date.GetUtcNow());
        return shelter1;
    }

    public ShelteredPet Seed_OneShelter_WithOnePet()
    {
        Date.SetUtcNow(new DateTime(2020, 1, 5));
        var shelter1 = NewShelter("ShelterA");
        var pet1 = NewShelteredPet("Doc", shelter1);

        Facade.ShelterCreate(shelter1, Date.GetUtcNow());
        Facade.ShelterAddPet(pet1, Date.GetUtcNow());

        return pet1;
    }

    public ShelteredPet GenerateScenario_FosterPet1()
    {
        Date.SetUtcNow(new DateTime(2020, 1, 5));
        var shelter1 = NewShelter("ShelterA");
        var shelteredPet = NewShelteredPet("Doc", shelter1);
        var fosterPerson = NewFosterPerson("Steve", 1);

        Facade.ShelterCreate(shelter1, Date.GetUtcNow());
        Facade.ShelterAddPet(shelteredPet, Date.GetUtcNow());
        Facade.FosterPersonCreate(fosterPerson, Date.GetUtcNow());
        Facade.ShelterAssignFosterPerson(new(shelteredPet, fosterPerson.Id, Date.GetUtcNow()));

        return shelteredPet;
    }

    public (ShelteredPet, FosterPerson) GenerateScenario_FosterPet2()
    {
        Date.SetUtcNow(new DateTime(2020, 1, 5));
        var shelter1 = NewShelter("ShelterA");
        var shelteredPet = NewShelteredPet("Doc", shelter1);
        var fosterPerson = NewFosterPerson("Steve", 1);

        Facade.ShelterCreate(shelter1, Date.GetUtcNow());
        Facade.ShelterAddPet(shelteredPet, Date.GetUtcNow());
        Facade.FosterPersonCreate(fosterPerson, Date.GetUtcNow());
        Facade.ShelterAssignFosterPerson(new(shelteredPet, fosterPerson.Id, Date.GetUtcNow()));

        return (shelteredPet, fosterPerson);
    }


    public (ShelteredPet, Shelter) Seed_TwoShelters_OnePet()
    {
        Date.SetUtcNow(new DateTime(2020, 1, 5));
        var originShelter = NewShelter("ShelterA");
        var targetShelter = NewShelter("ShelterB");
        var shelteredPet = NewShelteredPet("Doc", originShelter);

        Facade.ShelterCreate(originShelter, Date.GetUtcNow());
        Facade.ShelterCreate(targetShelter, Date.GetUtcNow());
        Facade.ShelterAddPet(shelteredPet, Date.GetUtcNow());

        return (shelteredPet, targetShelter);
    }


    public (ShelteredPet, FosterPerson, ShelterIdentity) Seed_TwoShelters_OnePet_OneFosterPerson()
    {
        Date.SetUtcNow(new DateTime(2020, 1, 5));
        var originShelter = NewShelter("ShelterA");
        var targetShelter = NewShelter("ShelterB");
        var shelteredPet = NewShelteredPet("Doc", originShelter);
        var fosterPerson = NewFosterPerson("Steve", 1);

        Facade.ShelterCreate(originShelter, Date.GetUtcNow());
        Facade.ShelterCreate(targetShelter, Date.GetUtcNow());
        Facade.ShelterAddPet(shelteredPet, Date.GetUtcNow());
        Facade.FosterPersonCreate(fosterPerson, Date.GetUtcNow());
        Facade.ShelterAssignFosterPerson(new(shelteredPet, fosterPerson.Id, Date.GetUtcNow()));

        return (shelteredPet, fosterPerson, targetShelter.Id);
    }


    public ShelteredPet GenerateScenario_AdoptPet()
    {
        Date.SetUtcNow(new DateTime(2020, 1, 5));
        var shelter1 = NewShelter("ShelterA");
        var shelteredPet = NewShelteredPet("Doc", shelter1);
        var adpopterPerson = NewAdopterPerson("Steve");

        Facade.ShelterCreate(shelter1, Date.GetUtcNow());
        Facade.ShelterAddPet(shelteredPet, Date.GetUtcNow());
        Facade.AdopterPersonCreate(adpopterPerson, Date.GetUtcNow());
        Facade.ShelterAssignAdopterPerson(new(shelteredPet, adpopterPerson.Id, Date.GetUtcNow()));

        return shelteredPet;
    }

    public (ShelteredPet, AdopterPerson, ShelterIdentity) Seed_TwoShelters_OnePet_OneAdopterPerson()
    {
        Date.SetUtcNow(new DateTime(2020, 1, 5));
        var originShelter = NewShelter("ShelterA");
        var targetShelter = NewShelter("ShelterB");
        var shelteredPet = NewShelteredPet("Doc", originShelter);
        var adopterPerson = NewAdopterPerson("Steve");

        Facade.ShelterCreate(originShelter, Date.GetUtcNow());
        Facade.ShelterCreate(targetShelter, Date.GetUtcNow());
        Facade.ShelterAddPet(shelteredPet, Date.GetUtcNow());
        Facade.AdopterPersonCreate(adopterPerson, Date.GetUtcNow());
        Facade.ShelterAssignAdopterPerson(new(shelteredPet, adopterPerson.Id, Date.GetUtcNow()));

        return (shelteredPet, adopterPerson, targetShelter.Id);
    }
}