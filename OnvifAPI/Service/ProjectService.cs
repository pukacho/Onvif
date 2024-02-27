using EFOnvifAPI.Models;
using Microsoft.EntityFrameworkCore;
using OnvifAPI.Interfaces;
using System.Diagnostics.Eventing.Reader;

namespace OnvifAPI.Service
{
    public class ProjectService: BaseService, IProjectService
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<EmailAndWhatsAppSender> emailAndWhatsAppSenderRepository;
        private readonly masterContext masterContext;

        public ProjectService(IRepository<Project> projectRepository, IRepository<EmailAndWhatsAppSender> emailAndWhatsAppSenderRepository, masterContext masterContext)
        {
            _projectRepository = projectRepository;
            this.emailAndWhatsAppSenderRepository = emailAndWhatsAppSenderRepository;
            this.masterContext = masterContext;
        }

       

        public IEnumerable<Project> GetAll()
        {

            var proj= masterContext.Projects.Include(x => x.Cameras).Include(x => x.Organization).ToList();
            foreach (var project in proj) 
            {
                if(project.Cameras.Count>0)
                    SetImages(project.Cameras);

                project.Image = GetProjectImage(project.OrganizationId, project.Id);
            }
            return proj;
        }


        public Project Add(Project newProject)
        {
            if (newProject.Image != null && newProject.Image.Any())
            {
                SaveProjectImage(newProject.Image, newProject.OrganizationId, newProject.Id);
            }
            return _projectRepository.Add(newProject);
        }

        public Project Update(Project updateProject)
        {
            if (updateProject.Image != null && updateProject.Image.Any())
            {
                SaveProjectImage(updateProject.Image, updateProject.OrganizationId, updateProject.Id);
            }
            else
            {
                DeleteProjectImage(updateProject.OrganizationId, updateProject.Id);
            }    
            return _projectRepository.Update(updateProject);
        }

      

        public bool Delete(int projectId)
        {
            var proj = masterContext.Projects.Include(x => x.EmailAndWhatsAppSenders).Where(n=>n.Id== projectId).FirstOrDefault();
            if (proj!= null && proj.EmailAndWhatsAppSenders!= null && proj.EmailAndWhatsAppSenders.Any())
            {
                foreach (var ems in proj.EmailAndWhatsAppSenders)
                {
                    emailAndWhatsAppSenderRepository.Delete(ems.Id);
                }
            }
           
            return _projectRepository.Delete(projectId);
        }

        public Project GetById(int projectId)
        {
            var proj = _projectRepository.GetById(projectId);
            proj.Image = GetProjectImage(proj.OrganizationId, proj.Id);
            return proj;
        }

        public IEnumerable<Project> GetByOrgId(int orgIId)
        {

            var proj = masterContext.Projects.Include(x => x.Cameras).Include(x => x.Organization).Where(p => p.OrganizationId == orgIId);
            foreach (var project in proj)
            {
                if (project.Cameras.Count > 0)
                    SetImages(project.Cameras);

                project.Image = GetProjectImage(project.OrganizationId, project.Id);
            }

            return proj;
        }
    }
}
