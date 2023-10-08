using System;
using System.Windows.Forms;
using Accord;
using Accord.Video;
using Accord.Video.FFMPEG;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using System.IO;
namespace cameraViewer
{
    public partial class Form1 : Form
    {
        public class VideoStreamWindow
        {
            private VideoFileReader reader = new VideoFileReader();
            private VideoFileWriter videoWriter = new VideoFileWriter();
            public bool isActive = false;
            string fileName;
            public VideoStreamWindow(string ip,int idx)
            {
                reader.Open(ip);
                Bitmap bmp = reader.ReadVideoFrame();
                fileName = "camera_" + idx.ToString();
                Directory.CreateDirectory("videos/" + fileName);
                int numberOfVideos = System.IO.Directory.GetFiles("videos/" + fileName).Length;

                System.Diagnostics.Debug.Print(numberOfVideos.ToString());

                videoWriter.Open("videos/camera_" + idx.ToString() + "/" + numberOfVideos.ToString() + ".avi", bmp.Width, bmp.Height, 24, VideoCodec.Default, 25000);
            }
            public void set_frame(ref PictureBox pictureBox1)
            {
                while (true)
                {
                    try
                    {
                        Bitmap bmp = reader.ReadVideoFrame();
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            g.FillRectangle(new SolidBrush(color: Color.White), new Rectangle(0, 0, 310, 45));
                            g.DrawString(DateTime.Now.ToString(), new Font("Arial", 25), new SolidBrush(color: Color.Black), 0, 0);
                        }
                        videoWriter.WriteVideoFrame(bmp);
                        if(isActive == true)
                            pictureBox1.Image = bmp;
                    }
                    catch(Exception e)
                    {
                        continue;
                    }
                }
            }
        }

        VideoStreamWindow vsw = new VideoStreamWindow("rtsp://192.168.100.95:8080/h264_ulaw.sdp",0);
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            vsw.isActive = true;
            Thread videoStreamThread = new Thread(() => vsw.set_frame(ref pictureBox1));
            videoStreamThread.IsBackground = true;
            videoStreamThread.Start();
            
            /*
            ThreadStart childref = new ThreadStart(set_frame);
            Thread childThread = new Thread(childref);
            childThread.IsBackground = true;
            childThread.Start();
            */
        }
    }
}
