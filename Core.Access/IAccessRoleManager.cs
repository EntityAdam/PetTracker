namespace Core.Access;

public record Role(RoleKind RoleKind);
public record User(string PrincipalId);
public record AccessRoleAssignment(User User, Role Role);

public enum RoleKind
{
    Anonymous,
    FosterPerson,
    AdopterPerson,
    Shelter
}

public interface IAccessRoleManager
{
    public void AssignRole(AccessRoleAssignment assignment);
}

public class AccessRoleManager : IAccessRoleManager
{
    private readonly List<Role> roles;
    private readonly IUserRolesStore store;

    public AccessRoleManager(IUserRolesStore store)
    {
        this.roles = new()
        {
            new(RoleKind.Anonymous),
            new(RoleKind.FosterPerson),
            new(RoleKind.AdopterPerson),
            new(RoleKind.Shelter)
        };
        this.store = store;
    }
    public void AssignRole(AccessRoleAssignment assignment)
    {
        var (user, role) = assignment;
        if (!roles.Contains(role)) throw new InvalidOperationException("Role does not exist");
        store.AssignRole(user, role);
    }
}

