using EFOnvifAPI.Models;

namespace OnvifAPI.Interfaces
{
    public interface IOrganizationService
    {
        IEnumerable<Organization> GetAll();
        Organization Add(Organization newOrganization);
        Organization Update(Organization updateOrganization);
        bool Delete(int organizationId);
        Organization GetById(int organizationId);
    }
}
