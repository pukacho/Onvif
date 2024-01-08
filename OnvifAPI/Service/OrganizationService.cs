using EFOnvifAPI.Models;
using OnvifAPI.Interfaces;

namespace OnvifAPI.Service
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IRepository<Organization> _organizationRepository;

        public OrganizationService(IRepository<Organization> OrganizationRepository)
        {
            _organizationRepository = OrganizationRepository;
        }

        public IEnumerable<Organization> GetAll()
        {
            return _organizationRepository.GetAll();
        }


        public Organization Add(Organization newOrganization)
        {
            return _organizationRepository.Add(newOrganization);
        }

        public Organization Update(Organization updateOrganization)
        {
            return _organizationRepository.Update(updateOrganization);
        }

        public bool Delete(int organizationId)
        {
            return _organizationRepository.Delete(organizationId);
        }

        public Organization GetById(int organizationId)
        {
            return _organizationRepository.GetById(organizationId);
        }
    }
}
