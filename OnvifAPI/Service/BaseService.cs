using EFOnvifAPI.Models;
using System.IO;

namespace OnvifAPI.Service
{
    public class BaseService
    {
        private readonly IConfigurationRoot _config;

        public BaseService() 
        {
            _config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
           .Build();
        }

        internal IEnumerable<Camera> SetImages(IEnumerable<Camera> enumerable)
        {

            // $@"\{Camera.Project.OrganizationId}\{Camera.ProjectId}\{Camera.Id}\
            foreach (var camera in enumerable)
            {
                camera.Image= Getimage(camera);
            }
            return enumerable;
        }

        internal byte[] Getimage(Camera camera)
        {
            try
            {
                string file = string.Format(@"{0}{1}", _config["ImagesPath"],$@"\{camera.Project.Organization.Id}\{camera.Project.Id}\{camera.Id}\");
                if (Directory.Exists(file))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(file);
                    var fileinfo = directoryInfo.GetFiles("*.jpg").OrderBy(d => d.CreationTime).FirstOrDefault();
                    return File.ReadAllBytes(fileinfo.FullName);
                }
                return new byte[0];
            }
            catch (Exception ex)
            {

                return new byte[0];
            }

        }

        internal bool SaveOrganizationImage(byte[] image, int organizationId)
        {
            try
            {
                string file = string.Format(@"{0}{1}", _config["ImagesPath"], $@"\{organizationId}\orgImage.png");
                File.WriteAllBytes(file, image);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        internal bool DeleteOrganizationFolders( int organizationId)
        {
            try
            {
                string file = string.Format(@"{0}{1}", _config["ImagesPath"], $@"\{organizationId}");
                File.Delete(file);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        internal byte[] GetOrganizationImage(int organizationId)
        {
            try
            {
                string file = string.Format(@"{0}{1}", _config["ImagesPath"], $@"\{organizationId}\");
                if (Directory.Exists(file))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(file);
                    var fileinfo = directoryInfo.GetFiles("orgImage.png").FirstOrDefault();
                    return File.ReadAllBytes(fileinfo.FullName);
                }
                
            }
            catch (Exception)
            {
                return new byte[0];
            }
            return new byte[0];
        }
    }
}
