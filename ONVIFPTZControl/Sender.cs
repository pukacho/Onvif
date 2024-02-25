using Accord.Video.FFMPEG;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace ONVIFPTZControl
{
    public class Sender : IDisposable
    {
        private Camera Camera { get; set; }
        private string TargetDir { get; set; }

        private const string mp4 = "*.mp4";
        private const string zip = "*.zip";
        private const string image = "*.jpg";
        private SmtpClient _smtpClient;
        private readonly string _appPath = "";
        private readonly string _orgImage;
        private readonly string _zipPath="";
        private readonly string _nameOrgProjectCamera;
        private readonly string _dateTime = "";
        private Logger Logger;
        /// <summary>
        /// Credentials = new NetworkCredential("Arie.cam11@gmail.com", "leclntyetldmgxwr"),
        /// </summary>
        public Sender()
        {
            _smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("arie.systems.ltd@gmail.com", "pgyg amwx bnqy ydye"),
                EnableSsl = true
            };
            TwilioClient.Init("ACd880ece640bede139af25e8bd531af6d", "21383155ba3e8e700c170a386f7aee33");
        }
        public Sender(Camera camera , Logger logger) :this()
        {
            Camera = camera;
            Logger = logger;
            _dateTime = $"{DateTime.Now.ToString("dd-MM-yyy-hh-mm")}";
            _appPath = ConfigurationManager.AppSettings["imagesPath"] + $@"\{Camera.Project.Organization.Id}\{Camera.Project.Id}\{Camera.Id}\";
            _orgImage = string.Format(@"{0}{1}", ConfigurationManager.AppSettings["imagesPath"], $@"\{Camera.Project.Organization.Id}\{Camera.Project.Id}\orgImage.png");
            _zipPath = _appPath + $@"{Camera.Project.Organization.Name}-{Camera.Project.Name}-{Camera.Name}-{_dateTime}.zip";
            _nameOrgProjectCamera = $"Organization: {Camera.Project.Organization.Name} Project: {Camera.Project.Name} Camera: {Camera.Name}";
        }

        public void StrtSending(string sendTo)
        {
            DeletoOldZipandVideo();
            try
            {
                if (Camera.Project.EmailAndWhatsAppSenders.Any())
                {
                    TargetDir = _appPath + $"old-{_dateTime}";
                    if (!Directory.Exists(TargetDir))
                    {
                        Directory.CreateDirectory(TargetDir);
                    }
                    if (Directory.GetFiles(_appPath).Any())
                    {
                        foreach (var file in Directory.GetFiles(_appPath, image))
                        {
                            File.Move(file, Path.Combine(TargetDir, Path.GetFileName(file)));
                        }
                    }
                    CreateVideo();
                    ZipFile.CreateFromDirectory(TargetDir, _zipPath);
                }
                foreach (var item in Camera.Project.EmailAndWhatsAppSenders)
                {
                    if (Directory.GetFiles(_appPath).Any())
                    {
                        _smtpClient.Send(GetMailWithImg(item));
                        Logger.Info($"Send to {item} all file from {TargetDir}");
                    }
                }
            }
            catch (Exception)
            {
                SendAlertNoEmail(sendTo, TargetDir );
            }
        }

        private void sendImageWhatsApp(EmailAndWhatsAppSender item)
        {
            if (!string.IsNullOrEmpty(item.PhoneNumber))
            {
                var url = Directory.EnumerateFiles(_appPath, mp4).FirstOrDefault();
                var mediaUrl = new[] 
                {
                    new Uri(url)
                }.ToList();

                MessageResource.Create(
                    mediaUrl: mediaUrl,
                    from: new Twilio.Types.PhoneNumber($"whatsapp:+972506800201"),
                    to: new Twilio.Types.PhoneNumber($"whatsapp:+972{item.PhoneNumber}")
                );
            }
           
        }

        private void DeleteOldAndNotNeedFiles()
        {
            DeletoOldZipandVideo();

            try
            {
                Directory.GetFiles(_appPath)
                   .Select(f => new FileInfo(f))
                   .Where(f => f.LastAccessTime < DateTime.Now.AddMonths(-1))
                   .ToList()
                   .ForEach(f => f.Delete());
            }
            catch (Exception ex)
            {
                Logger.Info($"Delete 1 Months EX {ex}");
            }
            try
            {
                var oldFile = Directory.GetFiles(TargetDir, image).FirstOrDefault();
                System.IO.FileInfo fi = new System.IO.FileInfo(oldFile);
                fi.MoveTo(_appPath+ "Old.jpg");
            }
            catch (Exception ex)
            {
                Logger.Info($"MoveTo EX  {ex}");
            }

          

        }
        void DeletoOldZipandVideo()
        {
            
            try
            {
                Directory.GetFiles(_appPath, mp4)
                  .Select(f => new FileInfo(f))
                  .ToList()
                  .ForEach(f => f.Delete());
            }
            catch (Exception ex)
            {
                Logger.Info($"Delete 1 mp4 EX {ex}");
            }

            try
            {
                Directory.GetFiles(_appPath, zip)
                 .Select(f => new FileInfo(f))
                 .ToList()
                 .ForEach(f => f.Delete());
            }
            catch (Exception ex)
            {
                Logger.Info($"Delete 1 zip EX {ex}");
            }
        }
        private void CreateVideo()
        {
            DeleteOldImage();

            using (VideoFileWriter writer = new VideoFileWriter())
            {
                writer.Open($@"{_appPath}myfile.mp4", 1920, 1080, 25, VideoCodec.MPEG4);
                var seconds = 0;

                if (File.Exists(_orgImage))
                {
                    writer.WriteVideoFrame(Bitmap.FromFile(_orgImage) as Bitmap, TimeSpan.FromSeconds(seconds));
                    seconds += Camera.FrameTimeSec;
                }
                
                foreach (var file in Directory.GetFiles($@"{TargetDir}\", image))
                {
                    writer.WriteVideoFrame(Bitmap.FromFile(file) as Bitmap, TimeSpan.FromSeconds(seconds));
                    seconds += Camera.FrameTimeSec;
                }
                writer.WriteVideoFrame(new Bitmap(1920, 1080), TimeSpan.FromSeconds(seconds));
                writer.Close();
            }
        }

        private void DeleteOldImage()
        {
            try
            {
                Directory.GetFiles(_appPath, "Old.jpg")
                  .Select(f => new FileInfo(f))
                  .ToList()
                  .ForEach(f => f.Delete());
            }
            catch (Exception ex)
            {
                Logger.Info($"Delete 1 Old.jpg EX {ex}");
            }
        }

        public void SendAlert(string to,string path)
        {
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = true;
            AddFile(mail, path);
            mail.From = new MailAddress("arie.systems.ltd@gmail.com");
            mail.To.Add(to);
            mail.Subject = $"images bad quality  {_nameOrgProjectCamera}";
            _smtpClient.Send(mail);
        }

        public void SendAlertNoSave(string to, string text)
        {
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = true;
          
            mail.From = new MailAddress("arie.systems.ltd@gmail.com");
            mail.To.Add(to);
            mail.Subject = $"Image Not Save {_nameOrgProjectCamera}";
            mail.Body = text;
            _smtpClient.Send(mail);
        }

        public void SendAlertNoEmail(string to, string text)
        {
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = true;

            mail.From = new MailAddress("arie.systems.ltd@gmail.com");
            mail.To.Add(to);
            mail.Subject = $"Email not send {_nameOrgProjectCamera}";
            mail.Body = text;
            _smtpClient.Send(mail);
        }

        private MailMessage GetMailWithImg(EmailAndWhatsAppSender emailAndWhatsAppSender)
        {
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = true;
            AttachFiles(mail, zip);
            if (!Camera.VideoDisabled)
            {
                AttachFiles(mail, mp4);
            }
            mail.From = new MailAddress("arie.systems.ltd@gmail.com");
            mail.To.Add(emailAndWhatsAppSender.Email);
            mail.Subject = $"Images from Project {Camera.Project.Name}";
            return mail;
        }

        private void AttachFiles(MailMessage mail,string path)
        {
            foreach (var item in Directory.EnumerateFiles(_appPath, path ))
            {
                AddFile(mail, item);
            }
        }

        private void AddFile(MailMessage mail, string item)
        {
            Attachment attachment = new Attachment(item, MediaTypeNames.Application.Octet);
            ContentDisposition disposition = attachment.ContentDisposition;
            disposition.CreationDate = File.GetCreationTime(item);
            disposition.ModificationDate = File.GetLastWriteTime(item);
            disposition.ReadDate = File.GetLastAccessTime(item);
            mail.Attachments.Add(attachment);
        }

        public void Dispose()
        {
            Logger= null;
            DeleteOldAndNotNeedFiles();
            _smtpClient.Dispose();
            _smtpClient = null;
        }
    }
}
