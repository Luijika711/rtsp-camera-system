using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.Toolkit.Uwp.Notifications;

namespace cameraViewer
{
    public partial class GetEmailForm : Form
    {
        public GetEmailForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.Email = textBox1.Text;
            Program.Password = textBox2.Text;
            File.AppendAllText("email.txt",Program.Email + "\n" + Program.Password);
            Form1 f = new Form1();
            f.Show();
            this.Hide();
        }
    }
}
