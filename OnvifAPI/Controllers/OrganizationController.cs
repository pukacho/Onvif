using EFOnvifAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnvifAPI.Interfaces;

namespace OnvifAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private IOrganizationService _organizationService;
        private readonly ILogger<OrganizationController> _logger;
        public OrganizationController(IOrganizationService organizationService, ILogger<OrganizationController> logger)
        {
            _organizationService = organizationService;
            _logger = logger;
        }

        [HttpGet("")]
        public ActionResult GetAll()
        {
            try
            {
                return Ok(_organizationService.GetAll());
            }
            catch (Exception ex)
            {
                _logger.LogError(" Organization GetAll " + ex);
                throw;
            }
        }

        [HttpPost("")]
        public ActionResult Add(Organization newOrganization)
        {
            try
            {
                return Ok(_organizationService.Add(newOrganization));
            }
            catch (Exception ex)
            {
                _logger.LogError(" Organization Add " + ex);
                throw;
            }
        }


        [HttpPut("")]
        public ActionResult Update(Organization updateOrganization)
        {
            try
            {
                return Ok(_organizationService.Update(updateOrganization));
            }
            catch (Exception ex)
            {
                _logger.LogError(" Organization Update " + ex);
                throw;
            }
        }

        [HttpDelete("")]
        public ActionResult Delete(int organizationId)
        {
            try
            {
                if (_organizationService.Delete(organizationId))
                    return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(" Organization Delete " + ex);
                throw;
            }
        }


        [HttpGet("by_id")]
        public ActionResult GetById(int organizationId)
        {
            try
            {
                return Ok(_organizationService.GetById(organizationId));
            }
            catch (Exception ex)
            {
                _logger.LogError(" Organization GetById " + ex);
                throw;
            }
        }
    }
}
