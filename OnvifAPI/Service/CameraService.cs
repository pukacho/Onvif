using EFOnvifAPI.Models;
using Microsoft.EntityFrameworkCore;
using OnvifAPI.Interfaces;

namespace OnvifAPI.Service
{
    public class CameraService: BaseService, ICameraService
    {

        private readonly IRepository<Camera> _cameraRepository;
        private readonly masterContext masterContext;

        public ICameraFrameTimeService CameraFrameTimeService { get; }

        public CameraService(IRepository<Camera> cameraRepository, ICameraFrameTimeService cameraFrameTimeService, masterContext masterContext)
        {
            _cameraRepository = cameraRepository;
            CameraFrameTimeService = cameraFrameTimeService;
            this.masterContext = masterContext;
        }

        public IEnumerable<Camera> GetAll()
        {
            return SetImages(masterContext.Cameras.Include(c=>c.Project).Include(x => x.Project.Organization));
        }

        public Camera Add(Camera newCamera)
        {
            var cam = _cameraRepository.Add(newCamera);
           
            return cam;
        }

        public Camera Update(Camera updateCamera)
        {
            var cam = _cameraRepository.Update(updateCamera);
            Camera camPO = masterContext.Cameras.Include(c => c.Project).Include(x => x.Project.Organization).FirstOrDefault(n=>n.Id== cam.Id);
            cam.Image = Getimage(camPO);
            return cam;
            //return Getimage(_cameraRepository.Update(updateCamera));
        }

        public bool Delete(int cameraId)
        {
            CameraFrameTimeService.DeleteByCamerId(cameraId);
            return _cameraRepository.Delete(cameraId);
        }

        public Camera GetById(int id)
        {
            var cam = _cameraRepository.GetById(id);
            Camera camPO = masterContext.Cameras.Include(c => c.Project).Include(x => x.Project.Organization).FirstOrDefault(n => n.Id == cam.Id);
            cam.Image = Getimage(camPO);
            return cam;
        }

        public IEnumerable<Camera> GetByPojectId(int id)
        {
            return SetImages(masterContext.Cameras.Include(c => c.Project).Include(x => x.Project.Organization).Where(c=>c.ProjectId== id));
        }


       
    }
}
