using EFOnvifAPI.Models;
using Microsoft.EntityFrameworkCore;
using OnvifAPI.Interfaces;
using OnvifAPI.Model;

namespace OnvifAPI.Service
{
    public class CameraFrameTimeService : BaseService, ICameraFrameTimeService
    {

        private readonly IRepository<CameraFrameTime> _cameraFrameTimeRepository;
        private readonly masterContext masterContext;

        public CameraFrameTimeService(IRepository<CameraFrameTime> cameraFrameTimeRepository, masterContext masterContext)
        {
            _cameraFrameTimeRepository = cameraFrameTimeRepository;
            this.masterContext = masterContext;
        }

        public IEnumerable<CameraFrameTime> GetAll()
        {
            return _cameraFrameTimeRepository.GetAll();
        }

        public CameraFrameTime Add(CameraFrameTime newcameraFrameTime)
        {
            return _cameraFrameTimeRepository.Add(newcameraFrameTime);
        }

        public IEnumerable< CameraFrameTime> AddList(CameraFrameTimeStringList newcameraFrameTime)
        {
            var list = new List<CameraFrameTime>();
            foreach (var item in newcameraFrameTime.FrameTime)
            {
                list.Add(new CameraFrameTime() { CameraId = newcameraFrameTime.Cameraid, FrameTime = item, CreateDate= DateTime.Now });
            }

            return _cameraFrameTimeRepository.AddList(list);
        }

        public IEnumerable<CameraFrameTime> Update(CameraFrameTimeStringList newcameraFrameTime)
        {
            DeleteByCamerId(newcameraFrameTime.Cameraid);
           
            if(newcameraFrameTime.FrameTime.Count==0) return new List<CameraFrameTime>();

            return AddList(newcameraFrameTime);
        }

        public bool Delete(int cameraFrameTimeId)
        {
            return _cameraFrameTimeRepository.Delete(cameraFrameTimeId);
        }

        public CameraFrameTime GetById(int id)
        {
            return _cameraFrameTimeRepository.GetById(id);
        }

        public IEnumerable<CameraFrameTime> GetByCameraId(int id)
        {
           return  masterContext.CameraFrameTimes.Where(p => p.CameraId == id);
        }


        public bool DeleteByCamerId(int id)
        {
            try
            {
                var allold = GetByCameraId(id);
                foreach (var item in allold)
                {
                    Delete(item.Id);
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
            
        }
    }
}
