using EFOnvifAPI.Models;

namespace OnvifAPI.Interfaces
{
    public interface ICameraService
    {
        IEnumerable<Camera> GetAll();
        Camera Add(Camera newCamera);
        Camera Update(Camera updateCamera);
        bool Delete(int cameraId);
        Camera GetById(int id);
        IEnumerable<Camera> GetByPojectId(int id);
    }
}
