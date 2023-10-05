using System;
using System.Windows.Forms;
using Accord;
using Accord.Video;
using Accord.Video.FFMPEG;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;

namespace cameraViewer
{
    public partial class Form1 : Form
    {
        public class VideoStreamWindow
        {
            private VideoFileReader reader = new VideoFileReader();
            private VideoFileWriter videoWriter = new VideoFileWriter();

            public VideoStreamWindow(string ip,int idx)
            {
                //reader.Open(ip);
                //Bitmap bmp = reader.ReadVideoFrame();
                //reader.close();
                reader.Open("camera_0.mp4");
                long frameCount = reader.FrameCount;
                List<Bitmap> bitmapList = new List<Bitmap>();
                while (frameCount > 0)
                {
                    var img = reader.ReadVideoFrame();
                    bitmapList.Add(img);
                    frameCount--;
                }
                reader.Close();
                reader.Open(ip);

                videoWriter.Open("camera_" + idx.ToString() + ".mp4", 1920,1080, 30, VideoCodec.MPEG4, 25000);

                foreach (Bitmap f in bitmapList)
                {
                    videoWriter.WriteVideoFrame(f);
                }
                
                
            }
            public void set_frame(ref PictureBox pictureBox1)
            {
                while (true)
                {
                    Bitmap bmp = reader.ReadVideoFrame();
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.DrawString(DateTime.Now.ToString(), new Font("Arial", 25), new SolidBrush(color: Color.Black), 0, 0);
                    }
                    videoWriter.WriteVideoFrame(bmp);
                    pictureBox1.Image = bmp;
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
