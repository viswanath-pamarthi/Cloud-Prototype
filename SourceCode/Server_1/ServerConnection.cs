using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Configuration;
using System.Security.Cryptography;

namespace Server
{
    public class ServerConnection
    {
        string fileLogins = "logins";
        //private TcpClient m_socket;
        private TcpClient m_socket;
        TcpClient SyncClient;
        private List<byte[]> messagesToSend;
        private int attemptCount;
        private string initialChecksum = null;
        private Thread m_thread = null;
        string ipOtherServer = null;
        int portOtherServer;
        private DateTime m_lastVerifyTime;

        private Encoding m_encoding;


        public ServerConnection(TcpClient sock, Encoding encoding, string ip, int port)
        {
            m_socket = sock;
            messagesToSend = new List<byte[]>();
            attemptCount = 0;
            ipOtherServer = ip;
            portOtherServer = port;
            m_lastVerifyTime = DateTime.UtcNow;
            m_encoding = encoding;
            // Thread ctThread = new Thread(processIncoming);

            // ctThread.Start();
            //SyncClient = new TcpClient();
            //SyncClient.Connect(ipOtherServer, portOtherServer);
            processIncoming();


        }

        public bool connected()
        {
            try
            {
                return m_socket.Connected;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool verifyConnected()
        {
            //note: `Available` is checked before because it's faster,
            //`Available` is also checked after to prevent a race condition.
            bool connected = m_socket.Client.Available != 0 ||
                !m_socket.Client.Poll(1, SelectMode.SelectRead) ||
                m_socket.Client.Available != 0;
            m_lastVerifyTime = DateTime.UtcNow;
            return connected;
        }

        public void processIncoming()
        {
            lock (m_socket)
            {
                byte[] bytesFrom = new byte[10025];
                string dataFromClient = null;
                Byte[] sendBytes = null;
                string serverResponse = null;
                string rCount = null;
                //requestCount = 0;
                dynamic length;
                bool loggedInLoop = true;
                bool userExists = false;
                string[] lines = File.ReadAllLines(ConfigurationManager.AppSettings.Get("logInFile"));

                //Indefinitely wait for login and after logged in break loop and proceed to listen to the next data
                while (loggedInLoop)
                {
                    NetworkStream logIndata = m_socket.GetStream();

                    //string s = data.Read(length, 0, m_socket.Available);
                    int count = logIndata.Read(bytesFrom, 0, m_socket.Available);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom, 0, count);
                    if (count > 0)
                    {
                        if (lines.Count() == 0)
                        {
                            using (StreamWriter stream = File.AppendText(ConfigurationManager.AppSettings.Get("logInFile")))
                            {
                                stream.WriteLine(dataFromClient);
                            }

                        }
                        else
                        {
                            if (lines.Count() > 0)
                            {
                                foreach (string line in lines)
                                {
                                    if (dataFromClient.Split(new string[] { ";_;" }, StringSplitOptions.RemoveEmptyEntries)[0] == line.Split(new string[] { ";_;" }, StringSplitOptions.RemoveEmptyEntries)[0])
                                    {
                                        userExists = true;
                                        //User exists and password matches
                                        if (dataFromClient.Split(new string[] { ";_;" }, StringSplitOptions.RemoveEmptyEntries)[1] == line.Split(new string[] { ";_;" }, StringSplitOptions.RemoveEmptyEntries)[1])
                                        {
                                            //success
                                            loggedInLoop = false;
                                            logIndata.Write(ASCIIEncoding.ASCII.GetBytes("true0"), 0, 5);

                                        }
                                        else
                                        {
                                            //failed login
                                            logIndata.Write(ASCIIEncoding.ASCII.GetBytes("false"), 0, 5);
                                        }
                                    }
                                }
                                if (!userExists)
                                {
                                    using (StreamWriter stream = File.AppendText(ConfigurationManager.AppSettings.Get("logInFile")))
                                    {
                                        stream.WriteLine(dataFromClient);
                                    }
                                    loggedInLoop = false;
                                    logIndata.Write(ASCIIEncoding.ASCII.GetBytes("true1"), 0, 5);

                                }
                            }
                        }

                    }

                }

                NetworkStream networkStream = m_socket.GetStream();
                while ((true))
                {                   

                    Array.Clear(bytesFrom, 0, bytesFrom.Length);
                    int count = networkStream.Read(bytesFrom, 0, m_socket.Available);
                    if (count > 0)
                    {
                        dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom, 0, count);
                        switch (dataFromClient)
                        {
                            case "fetchCommand":
                                fetchCommand();
                                break;
                            case "saveCommand":
                                networkStream.Write(ASCIIEncoding.ASCII.GetBytes("ok"), 0, 2);
                                saveCommand();

                                break;
                            case "exitCommand":
                                exitCommand();
                                break;
                            case "syncCommand":
                                syncCommand();
                                break;
                        }
                        // }
                    }
                  
                }
            }
        }

        public string checkMD5(string filename)
        {

            string stream = File.ReadAllText(filename);
            if (!string.IsNullOrEmpty(stream))
            {
                // return  Encoding.Default.GetString()
                string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(stream)).Select(s => s.ToString("x2")));
            }
            return null;



        }

        
        void syncCommand()
        {


        }
        public string GenerateMD5(string yourString)
        {
            return string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(yourString)).Select(s => s.ToString("x2")));
        }


        public static String GetMD5Hash(String TextToHash)
        {
            //Check wether data was passed
            if ((TextToHash == null) || (TextToHash.Length == 0))
            {
                return String.Empty;
            }

            //Calculate MD5 hash. This requires that the string is splitted into a byte[].
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] textToHash = Encoding.Default.GetBytes(TextToHash);
            byte[] result = md5.ComputeHash(textToHash);

            //Convert result back to string.
            return System.BitConverter.ToString(result);
        }

        public void fetchCommand()
        {
            NetworkStream networkStream = m_socket.GetStream();

            if (File.Exists(ConfigurationManager.AppSettings.Get("dataFile")))
            {

                byte[] dataToSend = Encoding.ASCII.GetBytes(File.ReadAllText(ConfigurationManager.AppSettings.Get("dataFile")));

                string response = "ok" + ";" + dataToSend.Length.ToString();
                networkStream.Write(ASCIIEncoding.ASCII.GetBytes(response), 0, response.Length);
                for (int i = 0; i < 500; i++) ;
                int bytesToBeSent = dataToSend.Length;
                int bytesActuallySent = 0;
                int bytesTosend;

                while (bytesActuallySent < bytesToBeSent)
                {
                    if (bytesToBeSent - bytesActuallySent >= 800)
                    {
                        bytesTosend = 800;
                    }
                    else
                    {
                        bytesTosend = bytesToBeSent - bytesActuallySent;

                    }
                    networkStream.Write(dataToSend, bytesActuallySent, bytesTosend);
                    bytesActuallySent += bytesTosend;

                }

                string stream = File.ReadAllText(ConfigurationManager.AppSettings.Get("dataFile"));
                initialChecksum = GetMD5Hash(stream);
            }
            else
            {
                networkStream.Write(ASCIIEncoding.ASCII.GetBytes("notOk"), 0, 2);
            }

        }


        public void exitCommand()
        {
            //check the log file and synchronize the data in the files to  other server
            // new socket to the other server?
            SyncClient = new TcpClient();
            SyncClient.Connect(ipOtherServer, portOtherServer);
            //client.Connect();

            if (SyncClient.Connected)
            {
                int i = 0;
                if (File.Exists(ConfigurationManager.AppSettings.Get("logFile")))
                {
                    NetworkStream networkStream = SyncClient.GetStream();
                    string lines = File.ReadAllText(ConfigurationManager.AppSettings.Get("logFile"));
                    byte[] dataToSend = Encoding.ASCII.GetBytes(File.ReadAllText(ConfigurationManager.AppSettings.Get("dataFile")));
                    //networkStream.Write(ASCIIEncoding.ASCII.GetBytes("test"), 0, 4);
                    System.Threading.Thread.Sleep(2000);
                    if (lines.Count() > 0 && dataToSend.Count() > 0)
                    {
                        string response = "startSync" + ";" + dataToSend.Length.ToString();
                        //for (i = 0; i < 500; i++) ;
                        networkStream.Write(ASCIIEncoding.ASCII.GetBytes(response), 0, response.Length);
                        System.Threading.Thread.Sleep(15000);
                        int bytesToBeSent = dataToSend.Length;
                        int bytesActuallySent = 0;
                        int bytesTosend;

                        while (bytesActuallySent < bytesToBeSent)
                        {
                            if (bytesToBeSent - bytesActuallySent >= 800)
                            {
                                bytesTosend = 800;
                            }
                            else
                            {
                                bytesTosend = bytesToBeSent - bytesActuallySent;

                            }
                            networkStream.Write(dataToSend, bytesActuallySent, bytesTosend);
                            bytesActuallySent += bytesTosend;

                        }
                        byte[] endResponse = ASCIIEncoding.ASCII.GetBytes("endSync");
                        System.Threading.Thread.Sleep(5000);
                        networkStream.Write(endResponse, 0, endResponse.Length);
                        System.Threading.Thread.Sleep(5000);
                        networkStream.Write(ASCIIEncoding.ASCII.GetBytes("e"), 0, 1);
                        File.WriteAllText(ConfigurationManager.AppSettings.Get("logFile"), string.Empty);

                    }
                }
            }
            for (int i = 0; i < 14000; i++) ;
            if(SyncClient.Connected)
            {
                SyncClient.Close();
            }
        }

        public void saveCommand()
        {
            int read;
            byte[] buffer = new byte[1024];
            NetworkStream logIndata = m_socket.GetStream();
            //string currentCheckSum = null;
            StringBuilder dataToFile = new StringBuilder();

            if (!File.Exists(ConfigurationManager.AppSettings.Get("dataFile")))
            {
                File.Create(ConfigurationManager.AppSettings.Get("dataFile")).Dispose();
            }
            using (System.IO.StreamWriter fileIO = new System.IO.StreamWriter(ConfigurationManager.AppSettings.Get("dataFile")))
            {
                while ((read = logIndata.Read(buffer, 0, m_socket.Available)) > 0)
                {
                    fileIO.Write(System.Text.Encoding.UTF8.GetString(buffer).ToCharArray(), 0, read);
                    dataToFile.Append(System.Text.Encoding.UTF8.GetString(buffer));
                    Array.Clear(buffer, 0, buffer.Length);
                }
            }

            //currentCheckSum = GetMD5Hash(dataToFile.ToString());
            //if (string.Compare(initialChecksum, currentCheckSum, true) == 0 || string.IsNullOrEmpty(initialChecksum))
            //{
            if (!File.Exists(ConfigurationManager.AppSettings.Get("logFile")))
            {
                File.Create(ConfigurationManager.AppSettings.Get("logFile")).Dispose();
                using (StreamWriter w = File.AppendText(ConfigurationManager.AppSettings.Get("logFile")))
                {
                    w.WriteLine("Save-@-{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                }
            }
            else
            {
                using (StreamWriter w = File.AppendText(ConfigurationManager.AppSettings.Get("logFile")))
                {
                    w.WriteLine("Save-@-{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                }
            }
            //    initialChecksum = currentCheckSum;
            //}
        }


        public bool validateCredentials()
        {
            //read the data streams
            return false;
        }

        public bool processOutgoing(int maxSendAttempts)
        {
            lock (m_socket)
            {
                if (!m_socket.Connected)
                {
                    messagesToSend.Clear();
                    return false;
                }

                if (messagesToSend.Count == 0)
                {
                    return false;
                }

                NetworkStream stream = m_socket.GetStream();
                try
                {
                    stream.Write(messagesToSend[0], 0, messagesToSend[0].Length);

                    lock (messagesToSend)
                    {
                        messagesToSend.RemoveAt(0);
                    }
                    attemptCount = 0;
                }
                catch (System.IO.IOException)
                {
                    //occurs when there's an error writing to network
                    attemptCount++;
                    if (attemptCount >= maxSendAttempts)
                    {
                        //TODO log error

                        lock (messagesToSend)
                        {
                            messagesToSend.RemoveAt(0);
                        }
                        attemptCount = 0;
                    }
                }
                catch (ObjectDisposedException)
                {
                    //occurs when stream is closed
                    m_socket.Close();
                    return false;
                }
            }
            return messagesToSend.Count != 0;
        }

        public void sendData(string data)
        {
            byte[] array = m_encoding.GetBytes(data);
            lock (messagesToSend)
            {
                messagesToSend.Add(array);
            }
        }

        public void forceDisconnect()
        {
            lock (m_socket)
            {
                m_socket.Close();
            }
        }

        public bool hasMoreWork()
        {
            return messagesToSend.Count > 0 || (Socket.Available > 0 && canStartNewThread());
        }

        private bool canStartNewThread()
        {
            if (m_thread == null)
            {
                return true;
            }
            return (m_thread.ThreadState & (ThreadState.Aborted | ThreadState.Stopped)) != 0 &&
                   (m_thread.ThreadState & ThreadState.Unstarted) == 0;
        }

        public TcpClient Socket
        {
            get
            {
                return m_socket;
            }
            set
            {
                m_socket = value;
            }
        }

        public Thread CallbackThread
        {
            get
            {
                return m_thread;
            }
            set
            {
                if (!canStartNewThread())
                {
                    throw new Exception("Cannot override TcpServerConnection Callback Thread. The old thread is still running.");
                }
                m_thread = value;
            }
        }

        public DateTime LastVerifyTime
        {
            get
            {
                return m_lastVerifyTime;
            }
        }

        public Encoding Encoding
        {
            get
            {
                return m_encoding;
            }
            set
            {
                m_encoding = value;
            }
        }
    }
}
