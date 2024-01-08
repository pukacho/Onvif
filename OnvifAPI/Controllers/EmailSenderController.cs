using EFOnvifAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OnvifAPI.Interfaces;
using OnvifAPI.Service;


namespace OnvifAPI.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmailAndWhatsSenderController : ControllerBase
    {
        private IEmailAndWhatsSenderService _emailSenderService;
        private readonly ILogger<EmailAndWhatsSenderController> _logger;
        public EmailAndWhatsSenderController(IEmailAndWhatsSenderService emailSenderService, ILogger<EmailAndWhatsSenderController> logger) 
        {
            _emailSenderService = emailSenderService;
            _logger = logger;
        }

        [HttpGet("")]
        public ActionResult GetAll()
        {
            try
            {
                return Ok(_emailSenderService.GetAll());

            }
            catch (Exception ex)
            {
                _logger.LogError("EmailAndWhatsSender GetAll " + ex);
                throw;
            }
            
        }

        [HttpPost("")]
        public ActionResult Add(EmailAndWhatsAppSender emailSender)
        {
            try
            {
                return Ok(_emailSenderService.Add(emailSender));

            }
            catch (Exception ex)
            {
                _logger.LogError("EmailAndWhatsSender Add " + ex);
                throw;
            }
            
        }
        [HttpPost("list")]
        public ActionResult AddList(List<EmailAndWhatsAppSender> emailSender)
        {
            try
            {
                return Ok(_emailSenderService.AddList(emailSender));

            }
            catch (Exception ex)
            {
                _logger.LogError("EmailAndWhatsSender Add " + ex);
                throw;
            }

        }

        [HttpPut("")]
        public ActionResult Update(EmailAndWhatsAppSender emailSender)
        {
            try
            {
                return Ok(_emailSenderService.Update(emailSender));

            }
            catch (Exception ex)
            {
                _logger.LogError("EmailAndWhatsSender Update " + ex);
                throw;
            }
            
        }

        [HttpDelete("")]
        public ActionResult Delete(int emailSenderId)
        {
            try
            {
                if (_emailSenderService.Delete(emailSenderId))
                    return Ok();
                return BadRequest();

            }
            catch (Exception ex)
            {
                _logger.LogError("EmailAndWhatsSender Delete " + ex);
                throw;
            }
            
        }


        [HttpGet("by_id")]
        public ActionResult GetById(int emailSenderId)
        {
            try
            {
                return Ok(_emailSenderService.GetById(emailSenderId));

            }
            catch (Exception ex)
            {
                _logger.LogError("EmailAndWhatsSender GetById " + ex);
                throw;
            }
            
        }

        [HttpGet("by_projectId")]
        public ActionResult GetByPojectId(int projectId)
        {
            try
            {
                return Ok(_emailSenderService.GetProjectId(projectId));

            }
            catch (Exception ex)
            {
                _logger.LogError("EmailAndWhatsSender GetProjectId " + ex);
                throw;
            }
            
        }
    }
}
