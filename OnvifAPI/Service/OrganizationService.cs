using EFOnvifAPI.Models;
using OnvifAPI.Interfaces;

namespace OnvifAPI.Service
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IRepository<Project> projectRepository;
        private readonly IRepository<Organization> _organizationRepository;

        public OrganizationService(IRepository<Project> projectRepository, IRepository<Organization> OrganizationRepository)
        {
            this.projectRepository = projectRepository;
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
            var org= _organizationRepository.GetById(organizationId);
            if (org.Projects!= null && org.Projects.Any())
            {
                foreach (var item in org.Projects)
                {
                    projectRepository.Delete(item.Id);
                }
            }
            return _organizationRepository.Delete(organizationId);
        }

        public Organization GetById(int organizationId)
        {
            return _organizationRepository.GetById(organizationId);
        }
    }
}
