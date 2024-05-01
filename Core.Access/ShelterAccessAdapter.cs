using Core.Interface;
using Core.Interface.Models;

namespace Core.Access;

public class ShelterAccessAdapter
{
    private readonly IUserRolesStore userRoles;
    private readonly IDomainFacade domainFacade;
    private readonly User user;
    private readonly Role requiredRole = new(RoleKind.Shelter);

    public ShelterAccessAdapter(IUserRolesStore userRoleStore, IDomainFacade shelterFacade, User user)
    {
        this.userRoles = userRoleStore;
        this.domainFacade = shelterFacade;
        this.user = user;
    }

    public void ShelterAssignAdopterPerson(AdopterPetAssignment assignment)
    { 
        if (!userRoles.IsUserInRole(user, requiredRole)) throw new UnauthorizedAccessException("User does not have permission");
        domainFacade.ShelterAssignAdopterPerson(assignment);
    }

    public void ShelterAssignFosterPerson(FosterPetAssignment assignment)
    {
        if (!userRoles.IsUserInRole(user, requiredRole)) throw new UnauthorizedAccessException("User does not have permission");
        domainFacade.ShelterAssignFosterPerson(assignment);
    }

    public void ShelterCreate(Shelter shelter, DateTime timestamp)
    {
        if (!userRoles.IsUserInRole(user, requiredRole)) throw new UnauthorizedAccessException("User does not have permission");
        domainFacade.ShelterCreate(shelter, timestamp);
    }

    //public void ShelterListPet(ShelteredPet pet, DateTime timestamp)
    //{
    //    if (!userRoles.IsUserInRole(user, requiredRole)) throw new UnauthorizedAccessException("User does not have permission");
    //    domainFacade.ShelterListPet(pet, timestamp);
    //}

    public void ShelterRemoveAndObfuscateData(ShelterIdentity shelter)
    {
        if (!userRoles.IsUserInRole(user, requiredRole)) throw new UnauthorizedAccessException("User does not have permission");
        domainFacade.ShelterRemoveAndObfuscateData(shelter);
    }

    public void ShelterTransferPet(ShelteredPet pet, ShelterIdentity shelter)
    {
        if (!userRoles.IsUserInRole(user, requiredRole)) throw new UnauthorizedAccessException("User does not have permission");
        domainFacade.ShelterTransferPet(pet, shelter);
    }

    public void ShelterUnlistPet(ShelteredPet pet)
    {
        if (!userRoles.IsUserInRole(user, requiredRole)) throw new UnauthorizedAccessException("User does not have permission");
        domainFacade.ShelterUnlistPet(pet);
    }
}