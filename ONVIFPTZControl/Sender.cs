using Accord.Video.FFMPEG;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ONVIFPTZControl
{
    public class Sender : IDisposable
    {
        private Camera Camera { get; set; }
        private string TargetDir { get; set; }

        private SmtpClient _smtpClient;
        private readonly string _appPath = "";
        private readonly string _zipPath="";
        private readonly string _nameOrgProjectCamera;
        private readonly string _dateTime = "";
        public Sender()
        {
            _smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("Arie.cam11@gmail.com", "leclntyetldmgxwr"),
                EnableSsl = true
            };
        }
        public Sender(Camera camera) :this()
        {
            Camera = camera;
            _dateTime= $"{DateTime.Now.ToString("dd-MM-yyy-hh-mm")}";
            _appPath = Path.GetDirectoryName(Application.ExecutablePath) + $@"\{Camera.Project.Organization.Name}\{Camera.Project.Name}\{Camera.Name}\";
            _zipPath = _appPath + $@"\{Camera.Project.Organization.Name}-{Camera.Project.Name}-{Camera.Name}-{_dateTime}.zip";
            _nameOrgProjectCamera = $"Organization: {Camera.Project.Organization.Name} Project: {Camera.Project.Name} Camera: {Camera.Name}";
        }

        public void StrtSending()
        {
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
                        foreach (var file in Directory.GetFiles(_appPath))
                        {
                            File.Move(file, Path.Combine(TargetDir, Path.GetFileName(file)));
                        }

                    }
                }

                foreach (var item in Camera.Project.EmailAndWhatsAppSenders)
                {
                    if (Directory.GetFiles(_appPath).Any())
                    {
                        CreateVideo();
                        ZipFile.CreateFromDirectory(TargetDir, _zipPath);
                        _smtpClient.Send(GetMailWithImg(item));
                    }
                   
                }
            }
            catch (Exception)
            {

                
            }
          
        }

        private void DeleteOldAndNotNeedFiles()
        {
            try
            {
                Directory.GetFiles(_appPath)
                   .Select(f => new FileInfo(f))
                   .Where(f => f.LastAccessTime < DateTime.Now.AddMonths(-1))
                   .ToList()
                   .ForEach(f => f.Delete());
            }
            catch (Exception)
            {

            }
            try
            {
                Directory.GetFiles(_appPath, "*.avi")
                  .Select(f => new FileInfo(f))
                  .ToList()
                  .ForEach(f => f.Delete());
            }
            catch (Exception)
            {

            }

            try
            {
                Directory.GetFiles(_appPath, "*.zip")
                 .Select(f => new FileInfo(f))
                 .ToList()
                 .ForEach(f => f.Delete());
            }
            catch (Exception)
            {

            }
            try
            {
                var oldFile = Directory.GetFiles(TargetDir, "*.jpg").FirstOrDefault();
                File.Copy(oldFile, Path.Combine(_appPath, Path.GetFileName(oldFile)));
            }
            catch (Exception)
            {

            }

            
        }

        private void CreateVideo()
        {
            using (VideoFileWriter writer = new VideoFileWriter())
            {
                writer.Open($@"{_appPath}myfile.avi", 1920, 1080, 25 , VideoCodec.MPEG4);
                var seconds = 0;
                foreach (var file in Directory.GetFiles($@"{TargetDir}\", "*.jpg"))
                {
                    writer.WriteVideoFrame(Bitmap.FromFile(file) as Bitmap, TimeSpan.FromSeconds(seconds));
                    seconds += Camera.FrameTimeSec;
                    writer.WriteVideoFrame(Bitmap.FromFile(file) as Bitmap, TimeSpan.FromSeconds(seconds));
                }
                writer.Close();
            }
        }
        public void SendAlert(string to,string path)
        {
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = true;
            AddFile(mail, path);
            mail.From = new MailAddress("Arie.cam11@gmail.com");
            mail.To.Add(to);
            mail.Subject = $"images bad quality  {_nameOrgProjectCamera}";
            _smtpClient.Send(mail);
        }
        private MailMessage GetMailWithImg(EmailAndWhatsAppSender emailAndWhatsAppSender)
        {
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = true;
            AttachFiles(mail, "*.zip");
            AttachFiles(mail, "*.avi");
            mail.From = new MailAddress("Arie.cam11@gmail.com");
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

        //private AlternateView GetEmbeddedImage(String filePath)
        //{
        //    LinkedResource res = new LinkedResource(filePath);
        //    res.ContentId = Guid.NewGuid().ToString();
        //    string htmlBody = @"<img src='cid:" + res.ContentId + @"'/>";
        //    AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
        //    alternateView.LinkedResources.Add(res);
        //    return alternateView;
        //}

        public void Dispose()
        {
            DeleteOldAndNotNeedFiles();
            _smtpClient.Dispose();
            _smtpClient = null;
        }
    }
}
