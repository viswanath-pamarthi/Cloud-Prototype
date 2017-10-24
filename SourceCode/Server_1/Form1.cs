using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            txtBoxPort.Text = "3000";
            //listener = new TcpListener(IPAddress.Any, m_port);
        }

        private void openTcpPort()
        {
            tcpServer.Close();
            tcpServer.Port=Convert.ToInt32(txtBoxPort.Text);
            tcpServer.PortSync = Convert.ToInt32(txtBoxPortSync.Text);
            tcpServer.IpOtherServer = txtIPServ.Text;
            tcpServer.PortOtherServer = Convert.ToInt32(txtOtherServerPort.Text);
            //txtPort.Text = tcpServer1.Port.ToString();
            tcpServer.Open();
        }

        private void startServer_Click(object sender, EventArgs e)
        {
            openTcpPort();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

       
    }
}
