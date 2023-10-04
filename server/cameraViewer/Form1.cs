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
        VideoFileReader reader = new VideoFileReader();
        void set_frame()
        {
            while (true)
            {
                Bitmap bmp = reader.ReadVideoFrame();
                pictureBox1.Image = bmp;
            }
        }
        public Form1()
        {
            reader.Open("rtsp://192.168.100.95:8080/h264_ulaw.sdp");
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ThreadStart childref = new ThreadStart(set_frame);
            Thread childThread = new Thread(childref);
            childThread.IsBackground = true;
            childThread.Start();
        }
    }
}
