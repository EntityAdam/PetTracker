using FluentAssertions;
using Microsoft.Extensions.Time.Testing;

namespace PetTracker.Tests;

public class ShelterPetsTests
{
    private readonly HistoryFixture fixture;
    private readonly FakeTimeProvider date;
    private readonly IShelterPetsFacade facade;

    public ShelterPetsTests()
    {
        this.fixture = new HistoryFixture();
        this.date = new FakeTimeProvider();
        this.facade = fixture.Facade;
    }

    [Fact]
    public void CreatePet_ShouldThrow_IfPetAlreadyExists()
    {
        var pet1 = fixture.Seed_OneShelter_WithOnePet();

        Action action = () => facade.ShelterAddPet(pet1, date.GetUtcNow());

        action.Should().Throw<InvalidOperationException>().WithMessage("Pet already exists");
    }

    [Fact]
    public void CreatePet_ShouldThrow_IfShelterDoesNotExist()
    {
        //This is a developer guard because it is not immediately apparent that:
        //Pets are only created by shelters, so the shelter must exist
        var shelteredPet = fixture.NewShelteredPet();

        Action action = () => facade.ShelterAddPet(shelteredPet, date.GetUtcNow());

        action.Should().Throw<InvalidOperationException>().WithMessage("Developer Guard: The shelter does not exist");
    }

    [Fact]
    public void TransferPet_ShouldThrow_IfOriginIsTarget()
    {
        var pet1 = fixture.Seed_OneShelter_WithOnePet();
        var originShelter = pet1.ShelterIdentity;

        Action action = () => facade.ShelterTransferPet(pet1, originShelter);

        action.Should().Throw<InvalidOperationException>().WithMessage("Origin shelter is target shelter");
    }

    [Fact]
    public void TransferPet_ShouldThrow_IfPetIsCurrentlyFostered() //shouldnt need this
    {
        var (shelteredPet, fosterPerson, targetShelterIdentity) = fixture.Seed_TwoShelters_OnePet_OneFosterPerson();

        Action action = () => facade.ShelterTransferPet(shelteredPet, targetShelterIdentity);

        action.Should().Throw<InvalidOperationException>().WithMessage("Pet is already fostered");
    }

    [Fact]
    public void TransferPet_ShouldThrow_IfPetIsCurrentlyAdopted() //shouldnt need this
    {
        var (shelteredPet, adopterPerson, targetShelterIdentity) = fixture.Seed_TwoShelters_OnePet_OneAdopterPerson();

        Action action = () => facade.ShelterTransferPet(shelteredPet, targetShelterIdentity);

        action.Should().Throw<InvalidOperationException>().WithMessage("Pet is already adopted");
    }

    [Fact]
    public void TransferPet_ShouldSucceed()
    {
        var (shelteredPet, targetShelter) = fixture.Seed_TwoShelters_OnePet();

        Action action = () => facade.ShelterTransferPet(shelteredPet, targetShelter.Id);

        action.Should().NotThrow();
    }
}
