using FluentAssertions;

namespace PetTracker.Tests;

public class HistoryTests
{
    private readonly HistoryFixture historyFixture; // This historyFixture is not shared. New instance per [Fact].
    private readonly Facade facade; //Shortcut.

    public HistoryTests()
    {
        this.historyFixture = new HistoryFixture();
        this.facade = historyFixture.Facade;
    }

    [Fact]
    public void GetPetHistory_WithScenarioListPetAtShelter()
    {
        var shelteredPet = historyFixture.Seed_OneShelter_WithOnePet();

        var p1events = facade.GetPetHistory(shelteredPet.Pet.Id);
        var listedEvent = p1events.Single();

        p1events.Should().HaveCount(1);
        listedEvent.PetIdentity.Should().Be(shelteredPet.Pet.Id);
    }

    [Fact]
    public void GetPetHistory_PetEvents_WithScenarioFosterPet()
    {
        var shelteredPet = historyFixture.GenerateScenario_FosterPet1();

        var petEvents = facade.GetPetHistory(shelteredPet.Pet.Id);
        var petFosteredEvent = petEvents.First(e => e.PetEventKind == PetEventKind.Fostered);

        petEvents.Should().HaveCount(2);
        petFosteredEvent.PetIdentity.Should().Be(shelteredPet.Pet.Id);
        petFosteredEvent.Timestamp.Should().Be(new DateTime(2020, 1, 5));

        var petsListedCount = facade.GetPetsListedCountByShelter(shelteredPet.ShelterIdentity);
        petsListedCount.Should().Be(1);
        var petsFosteredCount = facade.GetPetsFosteredCountByShelter(shelteredPet.ShelterIdentity);
        petsFosteredCount.Should().Be(1);
    }

    [Fact]
    public void GetPetsFosteredCount_ShelterEvents_WithScenarioFosterPet()
    {
        var pet = historyFixture.GenerateScenario_FosterPet1();

        var fosteredCount = facade.GetPetsFosteredCountByShelter(pet.ShelterIdentity);
        
        fosteredCount.Should().Be(1);
    }

    [Fact]
    public void GetShelterDateListedByShelter_ShelterEvents_WithScenarioFosterPet()
    {
        var pet = historyFixture.GenerateScenario_FosterPet1();

        var shelterEvents = facade.GetShelterDateListedByShelter(pet.ShelterIdentity);
        var shelterListedEvent = shelterEvents.First(e => e.ShelterEventKind == ShelterEventKind.ShelterListed);

        shelterEvents.Should().HaveCount(1);
        shelterListedEvent.Timestamp.Should().Be(new DateTime(2020, 1, 5));
    }

    [Fact]
    public void GetPetHistory_FosterPersonEvent_FosterPersonJoined_WithScenarioFosterPet()
    {
        var (_, fosterPerson) = historyFixture.GenerateScenario_FosterPet2();

        var personEvents = facade.GetFosterPersonHistory(fosterPerson.Id);
        var fosterPersonJoined = personEvents.First(e => e.FosterPersonEventKind == FosterPersonEventKind.FosterPersonJoin);

        personEvents.Should().HaveCount(2);
        fosterPersonJoined.PersonIdentity.Should().Be(fosterPerson.Id);
        fosterPersonJoined.Timestamp.Should().Be(new DateTime(2020, 1, 5));
    }

    [Fact]
    public void GetPetHistory_FosterPersonEvents_FosteredEvent_WithScenarioFosterPet()
    {
        var (_, fosterPerson) = historyFixture.GenerateScenario_FosterPet2();

        var personEvents = facade.GetFosterPersonHistory(fosterPerson.Id);
        var petFosteredEvent = personEvents.First(e => e.FosterPersonEventKind == FosterPersonEventKind.Fostered);

        personEvents.Should().HaveCount(2);
        petFosteredEvent.PersonIdentity.Should().Be(fosterPerson.Id);
        petFosteredEvent.Timestamp.Should().Be(new DateTime(2020, 1, 5));
    }


    [Fact]
    public void GetPetsListedCountByShelter_ShelterEvents_WithScenarioFosterPet()
    {
        var pet = historyFixture.Seed_OneShelter_WithOnePet();

        var count = facade.GetPetsListedCountByShelter(pet.ShelterIdentity);

        count.Should().Be(1);
    }

    [Fact]
    public void GetPetsAdoptedCountByShelter_ShelterEvents_WithScenarioAdoptedPet()
    {
        var pet = historyFixture.GenerateScenario_AdoptPet();

        int count = facade.GetPetsAdoptedCountByShelter(pet.ShelterIdentity);
        
        count.Should().Be(1);
    }
}
