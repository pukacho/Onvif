using EFOnvifAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OnvifAPI.Interfaces;
using OnvifAPI.Model;

namespace OnvifAPI.Controllers
{
    
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    [EnableCors("OpenCorsPolicy")]
    [Produces("application/json")]
    public class CameraController : ControllerBase
    {
        private ICameraFrameTimeService _cameraFrameTimeService;
        private ICameraService _cameraService;
        private readonly ILogger<CameraController> _logger;

        public CameraController(ICameraFrameTimeService cameraFrameTimeService, ICameraService cameraService, ILogger<CameraController> logger) 
        {
            _cameraService = cameraService;
            _cameraFrameTimeService = cameraFrameTimeService;
            _logger = logger;
        }


        [HttpGet("")]
        public ActionResult GetAll()
        {
            try
            {
                return Ok(_cameraService.GetAll());

            }
            catch (Exception ex)
            {
                _logger.LogError("Camera GetAll " + ex);
                throw;
            }
        }

        [HttpPost("")]
        public ActionResult Add(Camera camera)
        {
            try
            {
                var cam = _cameraService.Add(camera);
                return Ok(cam);

            }
            catch (Exception ex)
            {
                _logger.LogError("Camera Add " + ex);
                throw;
            }
            
        }


        [HttpPost("incloud_camera_frametime")]
        public ActionResult Addincloudframetime(CameraAndTime cameraandTime)
        {
            try
            {
                var cam = _cameraService.Add(cameraandTime.Camera);
                if (cam.Id!=0 && cameraandTime.CameraFrameTime.FrameTime.Count!=0)
                {
                    cameraandTime.CameraFrameTime.Cameraid = cam.Id;
                    _cameraFrameTimeService.AddList(cameraandTime.CameraFrameTime);
                }
                return Ok(cam);

            }
            catch (Exception ex)
            {
                _logger.LogError("Camera Add " + ex);
                throw;
            }
        }

        [HttpPut("")]
        public ActionResult Update(Camera camera)
        {
            try
            {
                return Ok(_cameraService.Update(camera));

            }
            catch (Exception ex)
            {
                _logger.LogError("Camera Update " + ex);
                throw;
            }

        }

        [HttpDelete("")]
        public ActionResult Delete(int cameraId)
        {
            try
            {
               
                if (_cameraService.Delete(cameraId))
                    return Ok();

                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError("Camera Delete " + ex);
                throw;
            }

        }


        [HttpGet("by_id")]
        public ActionResult GetById(int  cameraId)
        {
            try
            {
                return Ok(_cameraService.GetById(cameraId));

            }
            catch (Exception ex)
            {
                _logger.LogError("Camera GetById " + ex);
                throw;
            }

        }


        [HttpGet("by_PojectId")]
        public ActionResult GetByPojectId(int pojectId)
        {
            try
            {
                return Ok(_cameraService.GetByPojectId(pojectId));

            }
            catch (Exception ex)
            {
                _logger.LogError("Camera GetByPojectId " + ex);
                throw;
            }

        }
    }
}
