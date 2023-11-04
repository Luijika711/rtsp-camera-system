using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.Toolkit.Uwp.Notifications;

namespace cameraViewer
{
    static class Program
    {
        public static string Email = "";
        public static string Password = "";
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void AlertUser(string str)
        {
            new ToastContentBuilder()
                .AddArgument("action", "viewConversation")
                .AddArgument("conversationId", 9813)
                .AddText("Alert")
                .AddText("Face detected by camera " + str + "!")
                .Show();
        }
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if(new FileInfo("email.txt").Length == 0)
            {
                Application.Run(new GetEmailForm());
            }
            else
            {
                Email = File.ReadAllLines("email.txt")[0];
                Password = File.ReadAllLines("email.txt")[1];
                Application.Run(new Form1());
            }
            
        }
    }
}
