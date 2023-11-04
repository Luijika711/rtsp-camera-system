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
using Microsoft.Toolkit.Uwp.Notifications;


namespace cameraViewer
{
    public partial class Form1 : Form
    {
        public List<VideoStreamWindow> VideoStreams = new List<VideoStreamWindow>();
        public List<Thread> VideoStreamThreads = new List<Thread>();
        int VideoStreamIndex = -1;
        public Form1()
        {
            InitializeComponent();
            
        }

        private void LoadCameras()
        {
            VideoStreams.Clear();
            VideoStreamThreads.Clear();
            int LineIndex = 0;
            foreach (var line in File.ReadAllLines("cameras.txt"))
            {
                var CurrentVideoStreamWindow = new VideoStreamWindow(line.ToString(), LineIndex);
                VideoStreams.Add(CurrentVideoStreamWindow);
                LineIndex++;
            }


            if (VideoStreams.Count > 0)
            {
                VideoStreamIndex = 0;
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
        private void Form1_Load(object sender, EventArgs e)
        {
            VideoStreamIndex = 0;
            LoadCameras();
        }



        private void button1_Click(object sender, EventArgs e)
        {
            VideoStreamIndex--;
            if(VideoStreamIndex < 0)
            {
                VideoStreamIndex = VideoStreams.Count - 1;
            }
            if (VideoStreamIndex > 0)
            {
                foreach (var v in VideoStreams)
                    v.IsActive = false;
                VideoStreams[VideoStreamIndex].IsActive = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (VideoStreams.Count > 0)
            {
                VideoStreamIndex++;
                VideoStreamIndex %= (VideoStreams.Count);
                foreach (var v in VideoStreams)
                    v.IsActive = false;
                VideoStreams[VideoStreamIndex].IsActive = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            File.AppendAllText("cameras.txt", "\n" + textBox1.Text);
            foreach (var v in VideoStreams)
                v.IsActive = false;
            LoadCameras();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(VideoStreams.Count > 0)
            {
                String IpToRemove = VideoStreams[VideoStreamIndex].VideoIp;
                var AllCameras = File.ReadAllLines("cameras.txt");
                File.WriteAllText("cameras.txt", string.Empty);
                foreach(var v in AllCameras)
                {
                    if(IpToRemove != v)
                    {
                        File.WriteAllText("cameras.txt", v + "\n");
                    }
                }
                LoadCameras();
            }
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Process.Start(@"videos");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (var v in VideoStreams)
                v.closeStreams();
            
            Application.Exit();
        }
    }
}
