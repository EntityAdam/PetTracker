using Core.Interface;
using Core.Interface.Models;

namespace Core.Access;

public class ShelterAccessAdapter : IShelterFacade, IShelterPetsFacade
{
    private readonly IUserRolesStore userRoles;
    private readonly IShelterFacade shelterFacade;
    private readonly IShelterPetsFacade shelterPetsFacade;
    private readonly User user;
    private readonly Role requiredRole = new(RoleKind.Shelter);

    public ShelterAccessAdapter(IUserRolesStore userRoleStore, IDomainFacade domainFacade, User user)
    {
        this.userRoles = userRoleStore;
        this.shelterFacade = domainFacade as IShelterFacade
            ?? throw new InvalidOperationException("IDomainFacade must implement IShelterFacade.");
        this.shelterPetsFacade = domainFacade as IShelterPetsFacade
            ?? throw new InvalidOperationException("IDomainFacade must implement IShelterPetsFacade.");
        this.user = user;
    }

    public void ShelterCreate(Shelter shelter, DateTimeOffset timestamp)
    {
        EnsureAuthorized();
        shelterFacade.ShelterCreate(shelter, timestamp);
    }

    public IEnumerable<Shelter> ListShelters()
    {
        return shelterFacade.ListShelters();
    }

    public void ShelterRemoveAndObfuscateData(ShelterIdentity shelter)
    {
        EnsureAuthorized();
        shelterFacade.ShelterRemoveAndObfuscateData(shelter);
    }

    public void DeleteShelter(ShelterIdentity shelter)
    {
        EnsureAuthorized();
        shelterFacade.DeleteShelter(shelter);
    }

    public void ShelterAddPet(ShelteredPet shelteredPet, DateTimeOffset timestamp)
    {
        EnsureAuthorized();
        shelterPetsFacade.ShelterAddPet(shelteredPet, timestamp);
    }

    public IEnumerable<ShelteredPet> GetShelteredPets(ShelterIdentity shelterIdentity)
    {
        return shelterPetsFacade.GetShelteredPets(shelterIdentity);
    }

    public ShelteredPet? GetShelteredPetDetails(ShelterIdentity shelterIdentity, PetIdentity petIdentity)
    {
        return shelterPetsFacade.GetShelteredPetDetails(shelterIdentity, petIdentity);
    }

    public void ShelterTransferPet(ShelteredPet shelteredPet, ShelterIdentity shelter)
    {
        EnsureAuthorized();
        shelterPetsFacade.ShelterTransferPet(shelteredPet, shelter);
    }

    public void ShelterUnlistPet(ShelteredPet shelteredPet)
    {
        EnsureAuthorized();
        shelterPetsFacade.ShelterUnlistPet(shelteredPet);
    }

    private void EnsureAuthorized()
    {
        if (!userRoles.IsUserInRole(user, requiredRole))
        {
            throw new UnauthorizedAccessException("User does not have permission");
        }
    }

}