using EFOnvifAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OnvifAPI.Interfaces;
using OnvifAPI.Model;
using OnvifAPI.Service;

namespace OnvifAPI.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CameraFrameTimeController : ControllerBase
    {
        private ICameraFrameTimeService _cameraFrameTimeService;
        private readonly ILogger<CameraFrameTimeController> _logger;

        public CameraFrameTimeController(ICameraFrameTimeService cameraService, ILogger<CameraFrameTimeController> logger) 
        {
            _cameraFrameTimeService = cameraService;
            _logger = logger;
        }


        [HttpGet("")]
        public ActionResult GetAll()
        {
            try
            {
                return Ok(_cameraFrameTimeService.GetAll());

            }
            catch (Exception ex)
            {
                _logger.LogError("Camera GetAll " + ex);
                throw;
            }
        }

        [HttpPost("")]
        public ActionResult Add(CameraFrameTime cameraFrameTime)
        {
            try
            {
                return Ok(_cameraFrameTimeService.Add(cameraFrameTime));

            }
            catch (Exception ex)
            {
                _logger.LogError("Camera Add " + ex);
                throw;
            }
            
        }


        [HttpPost("List")]
        public ActionResult AddList(CameraFrameTimeStringList cameraFrameTime)
        {
            try
            {
                return Ok(_cameraFrameTimeService.AddList(cameraFrameTime));

            }
            catch (Exception ex)
            {
                _logger.LogError("Camera Add " + ex);
                throw;
            }

        }


        [HttpPut("")]
        public ActionResult Update(CameraFrameTimeStringList cameraFrameTime)
        {
            try
            {
                return Ok(_cameraFrameTimeService.Update(cameraFrameTime));

            }
            catch (Exception ex)
            {
                _logger.LogError("Camera Update " + ex);
                throw;
            }

        }

        [HttpDelete("")]
        public ActionResult Delete(int cameraFrameTimeId)
        {
            try
            {
                if (_cameraFrameTimeService.Delete(cameraFrameTimeId))
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
        public ActionResult GetById(int cameraFrameTimeId)
        {
            try
            {
                return Ok(_cameraFrameTimeService.GetById(cameraFrameTimeId));

            }
            catch (Exception ex)
            {
                _logger.LogError("Camera GetById " + ex);
                throw;
            }

        }
        [HttpGet("by_cameraId")]
        public ActionResult GetByCameraId(int cameraId)
        {
            try
            {
                return Ok(_cameraFrameTimeService.GetByCameraId(cameraId));

            }
            catch (Exception ex)
            {
                _logger.LogError("Camera GetById " + ex);
                throw;
            }

        }
    }
}
