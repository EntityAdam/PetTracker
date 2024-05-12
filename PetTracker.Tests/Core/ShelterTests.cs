using Core.Interface.Models;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;

namespace PetTracker.Tests;


public class ShelterTests
{
    private readonly HistoryFixture fixture;
    private readonly FakeTimeProvider date;
    private readonly IShelterFacade facade;

    public ShelterTests()
    {
        this.fixture = new HistoryFixture();
        this.date = new FakeTimeProvider();
        this.facade = fixture.Facade;
    }

    [Fact]
    public void CreateShelter_ListSheltersShouldReturnShelters()
    {
        var shelter1 = fixture.Seed_OneShelter();

        var shelterList = facade.ListShelters();

        shelterList.Should().Contain(shelter1);
    }

    [Fact]
    public void CreateShelter_ShouldThrow_IfShelterAlreadyExists()
    {
        var shelter1 = fixture.Seed_OneShelter();

        Action action = () => facade.ShelterCreate(shelter1, date.GetUtcNow());

        action.Should().Throw<InvalidOperationException>().WithMessage("Shelter already exists");
    }








    //[Fact]
    //public void CreateFosterPerson_ShouldSucceed()
    //{
    //    var x = historyFixture.GenerateScenario_FosterPet1();
    //    Action action = () => facade.PersonOpenToFoster(historyFixture.F1, timestamp);
    //    action.Should().NotThrow();
    //}





    [Fact]
    public void RemoveShelter_ShouldThrow_IfShelterDoesNotExist()
    {
        ShelterIdentity nonExistantShelter = new(Ulid.NewUlid());
        Action action = () => facade.ShelterRemoveAndObfuscateData(nonExistantShelter);
        action.Should().Throw<InvalidOperationException>().WithMessage("Shelter does not exist");
    }


    //[Fact]
    //public void FosterPetAssign_ShouldSucceed()
    //{
    //    var timestamp = date.Specify(new DateTime(2020, 1, 5));
    //    facade.ShelterCreate(historyFixture.S1, timestamp);
    //    facade.ShelterAddPet(historyFixture.P1, timestamp);
    //    facade.PersonOpenToFoster(historyFixture.F1, timestamp);
    //    Action action = () => facade.ShelterAssignFosterPerson(new(historyFixture.P1, historyFixture.F1, historyFixture.Date.GetCurrentDateTime()));
    //    action.Should().NotThrow();
    //}

    //[Fact]
    //public void FosterPetAssign_ShouldThrow_WhenPetIsAlreadyFostered()
    //{
    //    var timestamp = date.Specify(new DateTime(2020, 1, 5));
    //    facade.ShelterCreate(historyFixture.S1, timestamp);
    //    facade.ShelterAddPet(historyFixture.P1, timestamp);
    //    facade.PersonOpenToFoster(historyFixture.F1, timestamp);
    //    facade.ShelterAssignFosterPerson(new(historyFixture.P1, historyFixture.F1, historyFixture.Date.GetCurrentDateTime()));
    //    Action action = () => facade.ShelterAssignFosterPerson(new(historyFixture.P1, historyFixture.F1, historyFixture.Date.GetCurrentDateTime()));
    //    action.Should().Throw<InvalidOperationException>().WithMessage("Pet is already fostered");
    //}

    //[Fact]
    //public void FosterPetAssign_ShouldThrow_WhenPetIsAlreadyAdopted()
    //{
    //    var timestamp = date.Specify(new DateTime(2020, 1, 5));
    //    facade.ShelterCreate(historyFixture.S1, timestamp);
    //    facade.ShelterAddPet(historyFixture.P1, timestamp);
    //    facade.PersonOpenToAdoption(historyFixture.A1);
    //    facade.ShelterAssignAdopterPerson(new(historyFixture.P1, historyFixture.A1, historyFixture.Date.GetCurrentDateTime()));
    //    Action action = () => facade.ShelterAssignFosterPerson(new(historyFixture.P1, historyFixture.F1, historyFixture.Date.GetCurrentDateTime()));
    //    action.Should().Throw<InvalidOperationException>().WithMessage("Adopted pets can't be assigned to a foster");
    //}

    //[Fact]
    //public void RemoveAdopterPerson_ShouldSucceed()
    //{
    //    facade.PersonOpenToAdoption(historyFixture.A1);
    //    Action action = () => facade.AdopterPersonRemove(historyFixture.A1);
    //    action.Should().NotThrow();
    //}


    //[Fact]
    //public void RemoveFosterPerson_ShouldSucceed()
    //{
    //    var timestamp = date.Specify(new DateTime(2020, 1, 5));
    //    facade.PersonOpenToFoster(historyFixture.F1, timestamp);
    //    Action action = () => facade.FosterPersonRemove(historyFixture.F1);
    //    action.Should().NotThrow();
    //}

    //[Fact]
    //public void RemoveFosterPerson_ShouldThrowIfFosteringPets()
    //{
    //    var timestamp = date.Specify(new DateTime(2020, 1, 5));
    //    facade.ShelterCreate(historyFixture.S1, timestamp);
    //    facade.ShelterAddPet(historyFixture.P1, timestamp);
    //    facade.PersonOpenToFoster(historyFixture.F1, timestamp);
    //    facade.ShelterAssignFosterPerson(new(historyFixture.P1, historyFixture.F1, historyFixture.Date.GetCurrentDateTime()));
    //    Action action = () => facade.FosterPersonRemove(historyFixture.F1);
    //    action.Should().Throw<InvalidOperationException>().WithMessage("Foster person is in custody of pets that must be returned");
    //}

    //[Fact]
    //public void RemovePet_ShouldSucceed()
    //{
    //    var timestamp = date.Specify(new DateTime(2020, 1, 5));
    //    facade.ShelterCreate(historyFixture.S1, timestamp);
    //    facade.ShelterAddPet(historyFixture.P1, timestamp);
    //    facade.ShelterUnlistPet(historyFixture.P1);
    //}
}
