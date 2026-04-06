using Core.Access;
using Microsoft.Extensions.Time.Testing;

namespace PetTracker.Tests;

public class AccessTests
{
    [Fact]
    public void ShelterCreate_WithShelterRole_ShouldSucceed()
    {
        TestHelper testHelper = new(new FakeTimeProvider());
        User user = testHelper.NewUserInRole("adam@entityadam.com", RoleKind.Shelter);
        ShelterAccessAdapter adapter = new(testHelper.UserStore, testHelper.Facade, user);

        Shelter shelter = new(new(Ulid.NewUlid()), new("Shelter A", string.Empty));

        adapter.ShelterCreate(shelter, testHelper.TimeProvider.GetUtcNow());

        testHelper.Facade.ListShelters().Should().ContainSingle(x => x.Id == shelter.Id);
    }

    [Fact]
    public void ShelterCreate_WithoutShelterRole_ShouldThrow()
    {
        TestHelper testHelper = new(new FakeTimeProvider());
        User user = TestHelper.NewUser("adam@entityadam.com");
        ShelterAccessAdapter adapter = new(testHelper.UserStore, testHelper.Facade, user);

        Shelter shelter = new(new(Ulid.NewUlid()), new("Shelter A", string.Empty));

        var act = () => adapter.ShelterCreate(shelter, testHelper.TimeProvider.GetUtcNow());

        act.Should().Throw<UnauthorizedAccessException>();
    }

    [Fact]
    public void ShelterAddPet_WithoutShelterRole_ShouldThrow()
    {
        TestHelper testHelper = new(new FakeTimeProvider());
        User authorizedUser = testHelper.NewUserInRole("owner@entityadam.com", RoleKind.Shelter);
        ShelterAccessAdapter authorizedAdapter = new(testHelper.UserStore, testHelper.Facade, authorizedUser);
        Shelter shelter = new(new(Ulid.NewUlid()), new("Shelter A", string.Empty));
        authorizedAdapter.ShelterCreate(shelter, testHelper.TimeProvider.GetUtcNow());

        User unauthorizedUser = TestHelper.NewUser("anonymous@entityadam.com");
        ShelterAccessAdapter unauthorizedAdapter = new(testHelper.UserStore, testHelper.Facade, unauthorizedUser);
        Pet pet = Pet.CreateNewWithName("Sandy");
        ShelteredPet shelteredPet = new(pet, shelter.Id);

        var act = () => unauthorizedAdapter.ShelterAddPet(shelteredPet, testHelper.TimeProvider.GetUtcNow());

        act.Should().Throw<UnauthorizedAccessException>();
    }

    [Fact]
    public void ListShelters_WithoutShelterRole_ShouldReturnResults()
    {
        TestHelper testHelper = new(new FakeTimeProvider());
        User authorizedUser = testHelper.NewUserInRole("owner@entityadam.com", RoleKind.Shelter);
        ShelterAccessAdapter authorizedAdapter = new(testHelper.UserStore, testHelper.Facade, authorizedUser);
        Shelter shelter = new(new(Ulid.NewUlid()), new("Shelter A", string.Empty));
        authorizedAdapter.ShelterCreate(shelter, testHelper.TimeProvider.GetUtcNow());

        User anonymousUser = TestHelper.NewUser("anonymous@entityadam.com");
        ShelterAccessAdapter adapter = new(testHelper.UserStore, testHelper.Facade, anonymousUser);

        var result = adapter.ListShelters();
        result.Should().ContainSingle(s => s.Id == shelter.Id);
    }

    [Fact]
    public void GetShelteredPetDetails_WithShelterRole_ShouldReturnPet()
    {
        TestHelper testHelper = new(new FakeTimeProvider());
        User user = testHelper.NewUserInRole("owner@entityadam.com", RoleKind.Shelter);
        ShelterAccessAdapter adapter = new(testHelper.UserStore, testHelper.Facade, user);

        Shelter shelter = new(new(Ulid.NewUlid()), new("Shelter A", string.Empty));
        adapter.ShelterCreate(shelter, testHelper.TimeProvider.GetUtcNow());

        Pet pet = Pet.CreateNewWithName("Sandy");
        ShelteredPet shelteredPet = new(pet, shelter.Id);
        adapter.ShelterAddPet(shelteredPet, testHelper.TimeProvider.GetUtcNow());

        var result = adapter.GetShelteredPetDetails(shelter.Id, pet.Id);

        result.Should().NotBeNull();
        result!.Pet.Id.Should().Be(pet.Id);
    }
}

public class TestHelper
{
    private readonly HistoryProviderInMemeory historyProvider;
    private readonly DataFacadeInMemory dataProvider;
    private readonly Facade facade;
    private readonly UserRoleStoreInMemory userStore;
    private readonly AccessRoleManager accessRoleManager;
    private readonly TimeProvider timeProvider;

    public Facade Facade => facade;
    public UserRoleStoreInMemory UserStore => userStore;
    public TimeProvider TimeProvider => timeProvider;

    public TestHelper(TimeProvider timeProvider)
    {
        this.historyProvider = new HistoryProviderInMemeory(timeProvider);
        this.dataProvider = new DataFacadeInMemory(timeProvider);
        this.facade = new Facade(dataProvider, historyProvider);
        this.userStore = new UserRoleStoreInMemory();
        this.accessRoleManager = new AccessRoleManager(userStore);
        this.timeProvider = timeProvider;
    }

    public User NewUserInRole(string email, RoleKind roleKind)
    {
        accessRoleManager.AssignRole(NewRoleAssignment(email, roleKind));
        return NewUser(email);
    }

    public static AccessRoleAssignment NewRoleAssignment(string email, RoleKind roleKind) => new(NewUser(email), NewRole(roleKind));
    public static User NewUser(string email) => new User(email);
    public static Role NewRole(RoleKind roleKind) => new Role(roleKind);
}
