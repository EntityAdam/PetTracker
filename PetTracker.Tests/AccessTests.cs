using Core.Access;
using Microsoft.Extensions.Time.Testing;

namespace PetTracker.Tests;

public class AccessTests
{
    [Fact]
    public void First()
    {
        TestHelper testHelper = new(new FakeTimeProvider());
        User user = testHelper.NewUserInRole("adam@entityadam.com", RoleKind.Shelter);
        ShelterAccessAdapter adapter = new(testHelper.UserStore, testHelper.Facade, user);
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
