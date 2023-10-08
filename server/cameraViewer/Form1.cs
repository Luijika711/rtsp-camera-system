using System;
using System.Windows.Forms;
using Accord;
using Accord.Video;
using Accord.Video.FFMPEG;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
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

                //System.Diagnostics.Debug.Print(numberOfVideos.ToString());

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
                        Debug.Print(e.Message);
                        continue;
                    }
                }
            }
        }
        List<VideoStreamWindow> videoStreams = new List<VideoStreamWindow>();
        List<Thread> videoStreamThreads = new List<Thread>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int lineIndex = 0;
            foreach(var line in File.ReadAllLines("cameras.txt"))
            {
                var tmpVideoStreamWindow = new VideoStreamWindow(line.ToString(), lineIndex);
                videoStreams.Add(tmpVideoStreamWindow);
                lineIndex++;
            }
            Debug.Print(videoStreams.Count.ToString());

            

            
            foreach(var videoStream in videoStreams)
            {
                Thread videoStreamThread = new Thread(() => videoStream.set_frame(ref pictureBox1));
                videoStreamThread.IsBackground = true;
                videoStreamThread.Start();
                videoStreamThreads.Add(videoStreamThread);
            }

            videoStreams[0].isActive = true;
           
        }
    }
}
