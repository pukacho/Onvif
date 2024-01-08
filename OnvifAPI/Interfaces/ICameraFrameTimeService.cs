using EFOnvifAPI.Models;
using OnvifAPI.Model;

namespace OnvifAPI.Interfaces
{
    public interface ICameraFrameTimeService
    {
        IEnumerable<CameraFrameTime> GetAll();
        CameraFrameTime Add(CameraFrameTime newcameraFrameTime);
        IEnumerable<CameraFrameTime> Update(CameraFrameTimeStringList newcameraFrameTime);
        bool Delete(int cameraFrameTimeId);
        CameraFrameTime GetById(int id);
        IEnumerable<CameraFrameTime> AddList(CameraFrameTimeStringList newcameraFrameTime);

        IEnumerable<CameraFrameTime> GetByCameraId(int id);
        bool DeleteByCamerId(int id);
    }
}
