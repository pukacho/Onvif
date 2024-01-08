using EFOnvifAPI.Models;

namespace OnvifAPI.Model
{
    public class CameraAndTime
    {
        public Camera Camera { get; set; }
        public CameraFrameTimeStringList CameraFrameTime { get; set; }
    }
}
