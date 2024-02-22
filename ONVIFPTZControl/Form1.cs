using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Threading;
using NLog;

namespace ONVIFPTZControl
{

    public partial class Form1 : Form
    {
        System.Timers.Timer timerImage;

        System.Timers.Timer timerToSend;

        private masterEntities1 masterEntitiesDB;
        
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public double TimerInterval { get; set; } = 300000;
        public double TimerIntervalToSend { get; set; } = 1800000;

        private SynchronizationContext syncContext;
        public Form1()
        {
            InitializeComponent();
            syncContext = SynchronizationContext.Current;
            masterEntitiesDB = new masterEntities1();
            timerImage = new System.Timers.Timer(TimerInterval);
            timerImage.Elapsed += Timer_Elapsed;
            TimerToSend_Elapsed(null, null);
            timerToSend = new System.Timers.Timer(TimerIntervalToSend);
            timerToSend.Elapsed += TimerToSend_Elapsed;
            timerToSend.Start();
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

                foreach (var ptz in allPtz)
                {
                    try
                    {
                        using (Sender send = new Sender(ptz, Logger))
                        {
                            send.StrtSending(textBox2.Text);
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
                }
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

                foreach (var ptz in allPtz)
                {
                    Task.Run(() =>
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
                                    var presets = ptz.Presets
                                                   .Split(';')
                                                   .Where(x => int.TryParse(x, out _))
                                                   .Select(int.Parse)
                                                   .ToList();

                                    foreach (var item in presets)
                                    {
                                        camera.GoToImagePreset(item);
                                        Thread.Sleep(5000);
                                        try
                                        {
                                            if (!camera.SaveImage(textBox1.Text, textBox3.Text))
                                            {
                                                using (Sender send = new Sender(ptz, Logger))
                                                {
                                                    send.SendAlert(textBox2.Text, camera.FullPath);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.Error("Save Image no image :" + ex);
                                            using (Sender send = new Sender(ptz, Logger))
                                            {
                                                send.SendAlertNoSave(textBox2.Text, ex.ToString());
                                            }
                                        }
                                    }

                                    camera.GoToSavedPreset();
                                    ptz.NextFrameDate = SetNextFrameDate(ptz.NextFrameDate, frames);
                                    masterEntitiesDB.SaveChanges();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Parallel fiemes :" + ex);
                        }
                    });
                }
                
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
        private static DateTime CreateDateToSend(Camera ptz)
        {
            return new DateTime(ptz.NextSendDate.Value.Year, ptz.NextSendDate.Value.Month, ptz.NextSendDate.Value.Day, 23, 59, 00);
        }

    }
}