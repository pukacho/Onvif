using ONVIFPTZControl.OnvifMedia10;
using ONVIFPTZControl.OnvifPTZService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Threading;
using System.Net.Mail;
using System.Net.Mime;
using System.Data.Entity;
using System.Reflection;
using NLog;
using NLog.Targets;

namespace ONVIFPTZControl
{
  
    public partial class Form1 : Form
    {
        System.Timers.Timer timerImage;

        System.Timers.Timer timerToSend;

        private masterEntities1 masterEntitiesDB;
        
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public double TimerInterval { get; set; } = 1800000;

        public double TimerIntervalToSend { get; set; } = 1800000;

        
        private SynchronizationContext syncContext;
        ////string url = "http://cam:user12345@http://62.0.142.29:1091";
        public Form1()
        {
            InitializeComponent();
            syncContext = SynchronizationContext.Current;
            masterEntitiesDB = new masterEntities1();
            timerImage = new System.Timers.Timer(TimerInterval);
            timerImage.Elapsed += Timer_Elapsed;
            Timer_Elapsed(null, null);
            timerToSend = new System.Timers.Timer(TimerIntervalToSend);
            timerToSend.Elapsed += TimerToSend_Elapsed;
            TimerToSend_Elapsed(null, null);
            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "file.txt" };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            NLog.LogManager.Configuration = config;
            
            
        }

        private void TimerToSend_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                timerToSend.Stop();
                List<Camera> allPtz= new List<Camera>();
                syncContext.Send(state =>
                {
                    allPtz = masterEntitiesDB.Cameras.Include("Project.Organization").Include("Project.EmailAndWhatsAppSenders").Where(x => x.NextSendDate == null || x.NextSendDate <= DateTime.Now).ToList();

                }, null);

                Parallel.ForEach(allPtz, ptz =>
                {
                    try
                    {
                        using (Sender send = new Sender(ptz))
                        {
                            send.StrtSending();
                            if (ptz.NextSendDate == null)
                                ptz.NextSendDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 00);

                            switch (ptz.NextSendDWM.ToLower())
                            {
                                case "day":
                                    ptz.NextSendDate = CreateDateToSend(ptz).AddDays(1);
                                    break;
                                case "week":
                                    ptz.NextSendDate = CreateDateToSend(ptz).AddDays(7);
                                    break;
                                case "month":
                                    ptz.NextSendDate = CreateDateToSend(ptz).AddMonths(1);
                                    break;
                                default:
                                    break;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Parallel fiemes :" + ex);

                    }

                   
                });
                masterEntitiesDB.SaveChanges();

            }
            catch (Exception ex)
            {
                Logger.Error("ToSend :"+ex);
            }
            finally
            {
                timerToSend.Start();
            }
        }

        private static DateTime CreateDateToSend(Camera ptz)
        {
            return new DateTime(ptz.NextSendDate.Value.Year, ptz.NextSendDate.Value.Month, ptz.NextSendDate.Value.Day, 23, 59, 00);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {

                timerImage.Stop();
                List<Camera> allPtz = new List<Camera>();
                List<CameraFrameTime> fiemes = new List<CameraFrameTime>();
                syncContext.Send(state =>
                {
                    allPtz = masterEntitiesDB.Cameras.Include("Project.Organization").Where(x => x.NextFrameDate == null || x.NextFrameDate <= DateTime.Now).ToList();
                    fiemes= masterEntitiesDB.CameraFrameTimes.ToList();
                   
                }, null);

                Parallel.ForEach(allPtz, ptz =>
                {
                    try
                    {
                        var frames = fiemes.Where(n => n.CameraId == ptz.Id).ToList();
                        if (!frames.Any()) return;

                        using (PtzCamera camera = new PtzCamera())
                        {
                            if (camera.Initialise(ptz))
                            {
                                camera.SetCurentPreset();
                                camera.GoToImagePreset();
                                Thread.Sleep(5000);
                                try
                                {
                                    if (!camera.SaveImage(textBox1))
                                    {
                                        using (Sender send = new Sender(ptz))
                                        {
                                            send.SendAlert(textBox2.Text, camera.FullPath);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                    Logger.Error("Save Image no image :" + ex);
                                    using (Sender send = new Sender(ptz))
                                    {
                                        send.SendAlertNoSave(textBox2.Text, ex.ToString());
                                        send.SendAlertNoSave("anatolipak@gmail.com", ex.ToString());
                                    }
                                }
                               
                                camera.GoToSavedPreset();
                                ptz.NextFrameDate = SetNextFrameDate(ptz.NextFrameDate, frames);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Parallel fiemes :" + ex);

                    }
                    
                });
                masterEntitiesDB.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Error("SaveImage :"+ex);
            }
            finally
            {
                timerImage.Start();
            }
            
        }

        private DateTime? SetNextFrameDate(DateTime? nextFrameDate, List<CameraFrameTime> frames)
        {
            var date=DateTime.Now;
            if (nextFrameDate== null)
            {
                nextFrameDate = GetFierstNextTimefromNow(frames, date);
            }
            else
            {
                var time= nextFrameDate.Value.Hour + ":" + nextFrameDate.Value.Minute;
                var element = frames.IndexOf(frames.FirstOrDefault(n => n.FrameTime == time));
                if (element==-1)
                {
                    nextFrameDate = GetFierstNextTimefromNow(frames, date);
                }
                else
                {
                    CameraFrameTime toset = frames.FirstOrDefault();
                    if (element +1 < frames.Count)
                    {
                        toset = frames[element+1];
                    }
                    nextFrameDate = setDate(date, toset.FrameTime.Split(':').Select(n => int.Parse(n)).ToArray());
                }
            }

            return nextFrameDate;
        }

        private static DateTime? GetFierstNextTimefromNow( List<CameraFrameTime> frames, DateTime date)
        {
            foreach (var item in frames)
            {
                var nextTime = setDate(date, item.FrameTime.Split(':').Select(n => int.Parse(n)).ToArray());
                if (nextTime> date)
                {
                   return nextTime;
                }
            }

            return setDate(date.AddDays(1), frames.First().FrameTime.Split(':').Select(n => int.Parse(n)).ToArray());
        }

        private static DateTime setDate(DateTime date, int[] time)
        {
            if (time[0]==0)
            {
                return new DateTime(date.Year, date.Month, date.Day+1, time[0], time[1], 00);
            }
            return new DateTime(date.Year, date.Month, date.Day, time[0], time[1], 00);
        }

      

       
    }
}