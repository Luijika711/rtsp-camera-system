using Accord.Video.FFMPEG;
using Accord.Vision.Detection.Cascades;
using Accord.Vision.Detection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using Windows.Phone.UI.Input;

namespace cameraViewer
{
    public class VideoStreamWindow
    {
        public VideoFileReader Reader = new VideoFileReader();
        public VideoFileWriter VideoWriter = new VideoFileWriter();
        public bool IsActive = false;
        string FileName;
        private HaarObjectDetector faceDetector = new HaarObjectDetector(new FaceHaarCascade());
        private DateTime lastNotificationTime = new DateTime();
        int CameraNumber;
        public string VideoIp;
        public VideoStreamWindow(string ip, int idx)
        {
            Reader.Open(ip);
            VideoIp = ip;
            CameraNumber = idx;
            Bitmap bmp = Reader.ReadVideoFrame();
            FileName = "camera_" + idx.ToString();
            Directory.CreateDirectory("videos/" + FileName);
            int NumberOfVideos = System.IO.Directory.GetFiles("videos/" + FileName).Length;

            //System.Diagnostics.Debug.Print(numberOfVideos.ToString());

            VideoWriter.Open("videos/camera_" + idx.ToString() + "/" + NumberOfVideos.ToString() + ".avi", bmp.Width, bmp.Height, 30, VideoCodec.Default, 25000);
        }
        public static void SendEmail(string email, string password)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.mail.yahoo.com")
                {
                    Port = 465,
                    Credentials = new NetworkCredential(email, password),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(email),
                    Subject = "Subiectul emailului",
                    Body = "Acesta este conținutul emailului.",
                };

                mailMessage.To.Add(email);

                smtpClient.Send(mailMessage);
                Console.WriteLine("Emailul a fost trimis cu succes.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare la trimiterea emailului: {ex.Message}");
            }
        }
        public void closeStreams()
        {
            VideoWriter.Close();
            Reader.Close();
        }
        public void set_frame(ref PictureBox pictureBox1)
        {
            int frameIndex = 0;
            while (true)
            {
                try
                {

                    Bitmap bmp = Reader.ReadVideoFrame();

                    bool frameHasFace = false;
                    if (frameIndex % 120 == 0)
                    {
                        var faceRectangles = faceDetector.ProcessFrame(bmp);
                        if (faceRectangles.Length > 0)
                        {
                            frameHasFace = true;
                        }
                    }

                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.FillRectangle(new SolidBrush(color: Color.White), new Rectangle(0, 0, 400, 30));
                        g.DrawString(DateTime.Now.ToString() + ", camera " + CameraNumber.ToString(), new Font("Arial", 20), new SolidBrush(color: Color.Black), 0, 0);
                    }

                    if (frameHasFace == true)
                    {
                        Program.AlertUser(this.CameraNumber.ToString());
                    }

                    VideoWriter.WriteVideoFrame(bmp);
                    if (IsActive == true)
                        pictureBox1.Image = bmp;
                    frameIndex++;
                }
                catch (Exception e)
                {
                    Debug.Print(e.Message);
                    continue;
                }
            }
        }
    }
}
