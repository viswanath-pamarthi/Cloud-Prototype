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
    public partial class PostLogin : Form
    {        
        private string username;

        public PostLogin()
        {
            InitializeComponent();
        }

        public PostLogin(Client.clientCloud clientObj)
        {                        
            clientObject = clientObj;
            InitializeComponent();
            
            this.Show();
            clientObject.Fetch();
        }

        private void PostLogin_Load(object sender, EventArgs e)
        {
            //fetch send "fetch" command
            
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //save to file "save" command
            clientObject.saveData();
        }

        private void PostLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
           int k= clientObject.exit();
           if (k==0)
           {
               Environment.Exit(0);
               this.Close();
               clientObject.sock.Close();
           }
           
        }
    }
}
