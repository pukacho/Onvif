using ONVIFPTZControl.OnvifMedia10;
using ONVIFPTZControl.OnvifPTZService;
using System;
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Emgu.CV;
using System.Configuration;

namespace ONVIFPTZControl
{
    public class PtzCamera : IDisposable
    {
        MediaClient _mediaClient;
        PTZClient _ptzClient;
        Profile _profile;
        OnvifPTZService.PTZSpeed _velocity;
        PTZConfigurationOptions _options;
        private PTZPreset[] _all;
        private Camera _camera;
        public string FullPath { get;  set; }

        public string ErrorMessage { get; private set; }

        public bool Initialise(Camera camera)
        {
            if(camera==null) return false;

            bool result = false;

            try
            {
                _camera = camera;
                var cameraAddress = $"{camera.Url}:{camera.Port}";
                var userName = camera.Usermane;
                var password = camera.Password;
                var messageElement = new TextMessageEncodingBindingElement()
                {
                    MessageVersion = MessageVersion.CreateVersion(
                      EnvelopeVersion.Soap12, AddressingVersion.None)
                };
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement()
                {
                    AuthenticationScheme = AuthenticationSchemes.Digest
                };

                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                _mediaClient = new MediaClient(bind,
                  new EndpointAddress($"http://{cameraAddress}/onvif/Media"));
                _mediaClient.ClientCredentials.HttpDigest.AllowedImpersonationLevel =
                  System.Security.Principal.TokenImpersonationLevel.Impersonation;
                _mediaClient.ClientCredentials.HttpDigest.ClientCredential.UserName = userName;
                _mediaClient.ClientCredentials.HttpDigest.ClientCredential.Password = password;
                _ptzClient = new PTZClient(bind,
                  new EndpointAddress($"http://{cameraAddress}/onvif/PTZ"));
                _ptzClient.ClientCredentials.HttpDigest.AllowedImpersonationLevel =
                  System.Security.Principal.TokenImpersonationLevel.Impersonation;
                _ptzClient.ClientCredentials.HttpDigest.ClientCredential.UserName = userName;
                _ptzClient.ClientCredentials.HttpDigest.ClientCredential.Password = password;
                var profs = _mediaClient.GetProfiles();
                _profile = _mediaClient.GetProfile(profs[0].token);

                var configs = _ptzClient.GetConfigurations();

                _options = _ptzClient.GetConfigurationOptions(configs[0].token);
                _velocity = new OnvifPTZService.PTZSpeed()
                {
                    PanTilt = new OnvifPTZService.Vector2D()
                    {
                        x = 0,
                        y = 0,
                        space = _options.Spaces.ContinuousPanTiltVelocitySpace[0].URI,
                    },
                    Zoom = new OnvifPTZService.Vector1D()
                    {
                        x = 0,
                        space = _options.Spaces.ContinuousZoomVelocitySpace[0].URI,
                    }
                };


                ErrorMessage = "";
                result = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return result;
        }

        public void SetCurentPreset()
        {
            try
            {
                _all = _ptzClient.GetPresets(_profile.token);
                string PresetToken = _all[18].token;
                _ptzClient.SetPreset(_profile.token, _all[18].Name, ref PresetToken);
            }
            catch (Exception)
            {
            }
           
        }

        public void GoToImagePreset()
        {
            try
            {
                string PresetTokenNxet = _all[19].token;
                _ptzClient.GotoPreset(_profile.token, PresetTokenNxet, _velocity);
            }
            catch (Exception)
            {
            }

        }

        public void GoToSavedPreset()
        {
            try
            {
                string PresetToken = _all[18].token;
                _ptzClient.GotoPreset(_profile.token, PresetToken, _velocity);
            }
            catch (Exception)
            {
            }
        }


        public bool SaveImage(TextBox textBox1)
        {
            try
            {
                var rtsp= _camera.RtspPort==null? $"rtsp://{_camera.Usermane}:{_camera.Password}@{_camera.Url}" :$"rtsp://{_camera.Usermane}:{_camera.Password}@{_camera.Url}:{_camera.RtspPort}";
                using (VideoCapture cameraCapture = new VideoCapture(rtsp))
                {

                    var imagepath= ConfigurationManager.AppSettings["imagesPath"];
                    string fileName = $@"{DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss")}.jpg";
                    string appPath = imagepath + $@"\{_camera.Project.Organization.Name}\{_camera.Project.Name}\{_camera.Name}\";
                    if (!Directory.Exists(appPath))
                    {
                        Directory.CreateDirectory(appPath);
                    }
                    Mat frame = cameraCapture.QueryFrame();
                    var bit = frame.ToBitmap();
                    FullPath = appPath + fileName;
                    bit.Save(FullPath, ImageFormat.Jpeg);
                    FileInfo fInfo = new FileInfo(FullPath);
                    long sLen = 0;
                    if (fInfo.Length >= (1 << 10))
                        sLen =fInfo.Length >> 10;
                    if (sLen < int.Parse(textBox1.Text))
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;

        }

        public void Dispose()
        {
            _mediaClient=null;
            _ptzClient = null;
            _profile=null;
            _velocity = null;
            _options = null;
            _all=null;
        }
    }


}
