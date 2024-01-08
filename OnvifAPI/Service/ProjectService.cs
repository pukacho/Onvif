using EFOnvifAPI.Models;
using Microsoft.EntityFrameworkCore;
using OnvifAPI.Interfaces;

namespace OnvifAPI.Service
{
    public class ProjectService: BaseService, IProjectService
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly masterContext masterContext;

        public ProjectService(IRepository<Project> projectRepository, masterContext masterContext)
        {
            _projectRepository = projectRepository;
            this.masterContext = masterContext;
        }

        public IEnumerable<Project> GetAll()
        {

            var proj= masterContext.Projects.Include(x => x.Cameras).Include(x => x.Organization).ToList();
            foreach (var project in proj) 
            {
                if(project.Cameras.Count>0)
                    SetImages(project.Cameras);
            }
            return proj;
        }


        public Project Add(Project newProject)
        {
            return _projectRepository.Add(newProject);
        }

        public Project Update(Project updateProject)
        {
            return _projectRepository.Update(updateProject);
        }

        public bool Delete(int projectId)
        {
            return _projectRepository.Delete(projectId);
        }

        public Project GetById(int projectId)
        {
            return _projectRepository.GetById(projectId);
        }

        public IEnumerable<Project> GetByOrgId(int orgIId)
        {

            var proj = masterContext.Projects.Include(x => x.Cameras).Include(x => x.Organization).Where(p => p.OrganizationId == orgIId);
            foreach (var project in proj)
            {
                if (project.Cameras.Count > 0)
                    SetImages(project.Cameras);
            }
            return proj;
        }
    }
}
