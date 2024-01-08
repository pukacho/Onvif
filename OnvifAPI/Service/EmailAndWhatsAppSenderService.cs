using EFOnvifAPI.Models;
using OnvifAPI.Interfaces;

namespace OnvifAPI.Service
{
    public class EmailAndWhatsAppSenderService : IEmailAndWhatsSenderService
    {
        private readonly IRepository<EmailAndWhatsAppSender> _emailSenderRepository;

        public EmailAndWhatsAppSenderService(IRepository<EmailAndWhatsAppSender> emailSenderRepository)
        {
            _emailSenderRepository = emailSenderRepository;
        }

        public IEnumerable<EmailAndWhatsAppSender> GetAll()
        {
            return _emailSenderRepository.GetAll();
        }


        public EmailAndWhatsAppSender Add(EmailAndWhatsAppSender newEmailSender)
        {
            return _emailSenderRepository.Add(newEmailSender);
        }

        public EmailAndWhatsAppSender Update(EmailAndWhatsAppSender updateEmailSender)
        {
            return _emailSenderRepository.Update(updateEmailSender);
        }

        public bool Delete(int emailSenderId)
        {
            return _emailSenderRepository.Delete(emailSenderId);
        }

        public EmailAndWhatsAppSender GetById(int emailSenderId)
        {
            return _emailSenderRepository.GetById(emailSenderId);
        }

        public IEnumerable<EmailAndWhatsAppSender> GetProjectId(int projectId)
        {
            return _emailSenderRepository.GetAll().Where(p => p.ProjectId == projectId);
        }

        public IEnumerable<EmailAndWhatsAppSender> AddList(List<EmailAndWhatsAppSender> emailSender)
        {
            return _emailSenderRepository.AddList(emailSender);
        }
    }
}
