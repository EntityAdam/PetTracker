using Core.Interface;
using Core.Interface.Models;

namespace Core.Access;

public class ShelterAccessAdapter : IShelterFacade, IShelterPetsFacade
{
    private readonly IUserRolesStore userRoles;
    private readonly IShelterFacade domainFacade;
    private readonly IShelterPetsFacade? shelterPetsFacade;
    private readonly User user;
    private readonly Role requiredRole = new(RoleKind.Shelter);

    public ShelterAccessAdapter(IUserRolesStore userRoleStore, IShelterFacade shelterFacade, User user)
    {
        this.userRoles = userRoleStore;
        this.domainFacade = shelterFacade;
        this.shelterPetsFacade = shelterFacade as IShelterPetsFacade;
        this.user = user;
    }

    public void ShelterCreate(Shelter shelter, DateTimeOffset timestamp)
    {
        EnsureAuthorized();
        domainFacade.ShelterCreate(shelter, timestamp);
    }

    public IEnumerable<Shelter> ListShelters()
    {
        EnsureAuthorized();
        return domainFacade.ListShelters();
    }

    public void ShelterRemoveAndObfuscateData(ShelterIdentity shelter)
    {
        EnsureAuthorized();
        domainFacade.ShelterRemoveAndObfuscateData(shelter);
    }

    public void DeleteShelter(ShelterIdentity shelter)
    {
        EnsureAuthorized();
        domainFacade.DeleteShelter(shelter);
    }

    public void ShelterAddPet(ShelteredPet shelteredPet, DateTimeOffset timestamp)
    {
        EnsureAuthorized();
        RequireShelterPetsFacade().ShelterAddPet(shelteredPet, timestamp);
    }

    public IEnumerable<ShelteredPet> GetShelteredPets(ShelterIdentity shelterIdentity)
    {
        EnsureAuthorized();
        return RequireShelterPetsFacade().GetShelteredPets(shelterIdentity);
    }

    public ShelteredPet? GetShelteredPetDetails(ShelterIdentity shelterIdentity, PetIdentity petIdentity)
    {
        EnsureAuthorized();
        return RequireShelterPetsFacade().GetShelteredPetDetails(shelterIdentity, petIdentity);
    }

    public void ShelterTransferPet(ShelteredPet shelteredPet, ShelterIdentity shelter)
    {
        EnsureAuthorized();
        RequireShelterPetsFacade().ShelterTransferPet(shelteredPet, shelter);
    }

    public void ShelterUnlistPet(ShelteredPet shelteredPet)
    {
        EnsureAuthorized();
        RequireShelterPetsFacade().ShelterUnlistPet(shelteredPet);
    }

    private void EnsureAuthorized()
    {
        if (!userRoles.IsUserInRole(user, requiredRole))
        {
            throw new UnauthorizedAccessException("User does not have permission");
        }
    }

    private IShelterPetsFacade RequireShelterPetsFacade()
    {
        if (shelterPetsFacade is null)
        {
            throw new InvalidOperationException("Shelter pet operations are not available from the configured facade.");
        }

        return shelterPetsFacade;
    }
}