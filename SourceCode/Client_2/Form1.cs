using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            clientObject.sendData();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            bool result=clientObject.connect();


            if (result)
            {
                MessageBox.Show("Login success");
                this.Hide();
                PostLogin po = new PostLogin(clientObject);

            }
            else
            {
                MessageBox.Show("Login Failed");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Environment.Exit(0);
        }
    }
}
