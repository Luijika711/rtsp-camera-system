using System;
using System.Windows.Forms;
using Accord;
using Accord.Video;
using Accord.Video.FFMPEG;
using System.Drawing;
using System.Threading;
namespace cameraViewer
{
    public partial class Form1 : Form
    {
        public class VideoStreamWindow
        {
            
            public VideoFileReader reader = new VideoFileReader();
            public VideoFileWriter videoWriter = new VideoFileWriter();
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

        VideoStreamWindow vsw = new VideoStreamWindow();
        public Form1()
        {
            
            vsw.reader.Open("rtsp://192.168.100.95:8080/h264_ulaw.sdp");
            vsw.videoWriter.Open("thing.avi", 1920, 1080,30);
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
