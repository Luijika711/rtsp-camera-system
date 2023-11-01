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
using Accord.Vision.Detection.Cascades;
using Accord.Vision.Detection;
namespace cameraViewer
{
    public partial class Form1 : Form
    {
        List<VideoStreamWindow> VideoStreams = new List<VideoStreamWindow>();
        List<Thread> VideoStreamThreads = new List<Thread>();
        int VideoStreamIndex = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int LineIndex = 0;
            foreach(var line in File.ReadAllLines("cameras.txt"))
            {
                var CurrentVideoStreamWindow = new VideoStreamWindow(line.ToString(), LineIndex);
                VideoStreams.Add(CurrentVideoStreamWindow);
                LineIndex++;
            }
            

            if (VideoStreams.Count > 0)
            {
                foreach (var VideoStream in VideoStreams)
                {
                    Thread VideoStreamThread = new Thread(() => VideoStream.set_frame(ref pictureBox1));
                    VideoStreamThread.IsBackground = true;
                    VideoStreamThread.Start();
                    VideoStreamThreads.Add(VideoStreamThread);
                }
               
                VideoStreams[VideoStreamIndex].IsActive = true;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*
            if(VideoStreams[VideoStreamIndex].IsActive == true)
            {
                VideoStreams[VideoStreamIndex].IsActive = false;
            }
            */
            VideoStreamIndex--;
            if(VideoStreamIndex < 0)
            {
                VideoStreamIndex = VideoStreams.Count - 1;
            }
            foreach (var v in VideoStreams)
                v.IsActive = false;
            VideoStreams[VideoStreamIndex].IsActive = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*
            if (VideoStreams[VideoStreamIndex].IsActive == true)
            {
                VideoStreams[VideoStreamIndex].IsActive = false;
            }
            */
            VideoStreamIndex++;
            VideoStreamIndex %= (VideoStreams.Count);
            foreach (var v in VideoStreams)
                v.IsActive = false;
            VideoStreams[VideoStreamIndex].IsActive = true;
        }
    }
}
