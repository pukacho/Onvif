using EFOnvifAPI.Models;

namespace OnvifAPI.Interfaces
{
    public interface IEmailAndWhatsSenderService
    {
        IEnumerable<EmailAndWhatsAppSender> GetAll();
        EmailAndWhatsAppSender Add(EmailAndWhatsAppSender newEmailSender);
        EmailAndWhatsAppSender Update(EmailAndWhatsAppSender updateEmailSender);
        bool Delete(int emailSenderId);
        EmailAndWhatsAppSender GetById(int emailSenderId);
        IEnumerable<EmailAndWhatsAppSender> GetProjectId(int projectId);
        IEnumerable<EmailAndWhatsAppSender> AddList(List<EmailAndWhatsAppSender> emailSender);
    }
}
