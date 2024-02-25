using EFOnvifAPI.Models;
using OnvifAPI.Interfaces;

namespace OnvifAPI.Service
{
    public class OrganizationService : BaseService, IOrganizationService
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
            var org = _organizationRepository.Add(newOrganization);
           
            return org;
        }

        public Organization Update(Organization updateOrganization)
        {
            var org = _organizationRepository.Update(updateOrganization);
           
            return org;
        }

        public bool Delete(int organizationId)
        {
            try
            {
                var org = _organizationRepository.GetById(organizationId);
                if (org.Projects != null && org.Projects.Any())
                {
                    foreach (var item in org.Projects)
                    {
                        projectRepository.Delete(item.Id);
                    }
                }
                if (_organizationRepository.Delete(organizationId))
                {
                    DeleteOrganizationFolders(organizationId);
                }

                return true;
            }
            catch (Exception)
            {

               return false;
            }
            
        }

        public Organization GetById(int organizationId)
        {
            var org = _organizationRepository.GetById(organizationId);
            
            return _organizationRepository.GetById(organizationId);
        }
    }
}
