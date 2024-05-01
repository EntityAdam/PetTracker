namespace Core.Access;

public interface IUserRolesStore
{
    void AssignRole(User user, Role role);
    bool IsUserInRole(User user, Role role);
}

public class UserRoleStoreInMemory : IUserRolesStore
{
    private readonly List<(User, Role)> UserRoles = new();

    public void AssignRole(User user, Role role)
    {
        UserRoles.Add((user, role));
    }

    public bool IsUserInRole(User user, Role role)
    {
        return UserRoles.Contains((user, role));
    }
}

