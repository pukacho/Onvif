using EFOnvifAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnvifAPI.Interfaces;
using OnvifAPI.Service;

namespace OnvifAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private IProjectService _projectService;
        private readonly ILogger<OrganizationController> _logger;
        public ProjectController(IProjectService projectService, ILogger<OrganizationController> logger)
        {
            _projectService = projectService;
            _logger = logger;
        }

        [HttpGet("")]
        public ActionResult GetAll()
        {
            try
            {
                return Ok(_projectService.GetAll());
            }
            catch (Exception ex)
            {
                _logger.LogError(" Project GetAll " + ex);
                throw;
            }
           
        }

        [HttpPost("")]
        public ActionResult Add(Project newProject)
        {
            try
            {
                return Ok(_projectService.Add(newProject));
            }
            catch (Exception ex)
            {
                _logger.LogError(" Project Add " + ex);
                throw;
            }
            
        }


        [HttpPut("")]
        public ActionResult Update(Project updateProject)
        {
            try
            {
                return Ok(_projectService.Update(updateProject));
            }
            catch (Exception ex)
            {
                _logger.LogError(" Project Update " + ex);
                throw;
            }
        }

        [HttpDelete("")]
        public ActionResult Delete(int projectId)
        {
            try
            {
                if (_projectService.Delete(projectId))
                    return Ok();

                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(" Project Delete " + ex);
                throw;
            }
        }


        [HttpGet("by_id")]
        public ActionResult GetById(int projectId)
        {
            try
            {
                return Ok(_projectService.GetById(projectId));
            }
            catch (Exception ex)
            {
                _logger.LogError(" Project GetById " + ex);
                throw;
            }
        }

        [HttpGet("by_OrgId")]
        public ActionResult GetByPojectId(int orgIId)
        {
            try
            {
                return Ok(_projectService.GetByOrgId(orgIId));
            }
            catch (Exception ex)
            {
                _logger.LogError(" Project GetByPojectId " + ex);
                throw;
            }
        }
    }
}
