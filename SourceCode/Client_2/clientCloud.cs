using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace Client
{
    public class clientCloud:Component
    {
       // private TcpListener listener;

        TcpClient Socket1 = null;
        
        public Socket sock ;//= new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        string uName;
        const string  save = "saveCommand";
        byte[] saveCommand = Encoding.ASCII.GetBytes(save);
        const string fetch = "fetchCommand";
        byte[] fetchCommand = Encoding.ASCII.GetBytes(fetch);
        const string exitCom = "exitCommand";
        byte[] exitCommand = Encoding.ASCII.GetBytes(exitCom);
        int loginCount=0;
        bool oneTime=true;

        //Method to login and connect to server
        public bool connect()
        {
            TextBox userName = Application.OpenForms["Form1"].Controls["textBox1"] as TextBox;
            TextBox password = Application.OpenForms["Form1"].Controls["textBox2"] as TextBox;

            TextBox txtServerIP = Application.OpenForms["Form1"].Controls["txtServerIP"] as TextBox;
            TextBox txtServerPort = Application.OpenForms["Form1"].Controls["txtServerPort"] as TextBox;

            
                
            if (oneTime)
            {
                IPAddress host = IPAddress.Parse(txtServerIP.Text);////161.45.167.244");
                IPEndPoint hostep = new IPEndPoint(host, Int32.Parse(txtServerPort.Text));
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                sock.Connect(hostep);
                oneTime = false;
            }
            string UnamePassword = userName.Text + ";_;" + password.Text;
            byte[] msg = Encoding.ASCII.GetBytes(UnamePassword);
            byte[] reply=new byte[5];            
            // Send the data through the socket.
            int bytesSent = sock.Send(msg);
            sock.Receive(reply);
            
            string k = System.Text.Encoding.ASCII.GetString(reply, 0, 5);
            int result=0;
            if (k.Contains("true"))
            {
                result=Int32.Parse(k.Substring(4, 1));
                k="true";
            }
            if (bool.Parse(k))
            {
                if (result == 1)
                {
                    MessageBox.Show("User Didn't exist, Login has been created with credentials entered");
                }
                uName = userName.Text;
                return true;
            }
            else
            {
                loginCount++;
                if(loginCount==3)
                {
                    MessageBox.Show("Limit(3 times) for wrong password reached.");
                    //sock.Close();
                    Environment.Exit(0);                    
                    
                    oneTime = true;
                }
                return false;
            }

            //return bool.Parse(reply.ToString());
        }

        public void sendData()
        {            
            string s = "Hello2";
            byte[] msg = Encoding.ASCII.GetBytes(s);
            sock.Send(msg);           
        }

        public void saveData()
        {
            byte[] okTosendData=new byte[4];
            string okToSendString;
            bool wait = true;
            sock.Send(saveCommand);
            while(wait)
            {
                sock.Receive(okTosendData);
                okToSendString=System.Text.Encoding.ASCII.GetString(okTosendData, 0, 2);
                if (okToSendString == "ok")
                {
                    wait = false;
                }
            }



            //saveCommand            
            TextBox data = Application.OpenForms["PostLogin"].Controls["txtContent"] as TextBox;

            byte[] dataToSave = Encoding.ASCII.GetBytes(data.Text);
            int bytesToBeSent = dataToSave.Length;
            int bytesActuallySent = 0;
            int bytesTosend;

            while(bytesActuallySent < bytesToBeSent)
            {
                if(bytesToBeSent-bytesActuallySent >=800)
                {
                    bytesTosend=800;
                }
                else{
                    bytesTosend=bytesToBeSent-bytesActuallySent;
                }
                 bytesActuallySent += sock.Send(dataToSave, bytesActuallySent,bytesTosend,SocketFlags.None);
            }
            
        }

        public void Fetch()
        {
            TextBox data = Application.OpenForms["PostLogin"].Controls["txtContent"] as TextBox;
            byte[] buffer = new byte[1024];
            byte[] okToStart = new byte[14];
            StringBuilder fileContent = new StringBuilder();
            string okToSendString=null;
            bool wait = true;
            char[] SplitCharacter={';'};
            int read;
            int totalDataToRead = 0; int dataReadTillNow = 0;
            //fetchCommand
            sock.Send(fetchCommand);

            while (wait)
            {
                sock.Receive(okToStart);
                okToSendString = System.Text.Encoding.ASCII.GetString(okToStart, 0, 14);
                if (okToSendString.Contains("ok"))
                {
                    if(okToSendString.StartsWith("ok"))
                    {
                        //string[] h = okToSendString.Split(SplitCharacter);
                        totalDataToRead = Int32.Parse(okToSendString.Split(SplitCharacter)[1]);
                    }
                    wait = false;
                }
            }

            if (okToSendString.StartsWith("ok"))
            {
                if (totalDataToRead > 0)
                {
                    while ((read = sock.Receive(buffer)) > 0)
                    {
                        fileContent.Append(System.Text.Encoding.UTF8.GetString(buffer));
                        //fileIO.Write(System.Text.Encoding.UTF8.GetString(buffer).ToCharArray(), 0, read);
                        Array.Clear(buffer, 0, buffer.Length);
                        dataReadTillNow = dataReadTillNow + read;
                        if (totalDataToRead == dataReadTillNow)
                        {
                            break;
                        }
                    }
                    data.Text = fileContent.ToString();
                }
            }


        }

        public int exit()
        {
            //exit command no save
            int read;            
            sock.Send(exitCommand);
            
            for(int i=0;i<200;i++)
            {

            }
            return 0 ;
            //while ((read = socket.Receive(buffer)) > 0)
            //{
            //    output.Write(buffer, 0, read);
            //}

        }
    }
}
