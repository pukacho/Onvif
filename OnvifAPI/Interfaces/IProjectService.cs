using EFOnvifAPI.Models;

namespace OnvifAPI.Interfaces
{
    public interface IProjectService
    {
        IEnumerable<Project> GetAll();
        Project Add(Project newProject);
        Project Update(Project updateProject);
        bool Delete(int projectId);
        Project GetById(int projectId);
        IEnumerable<Project> GetByOrgId(int orgIId);
    }
}
