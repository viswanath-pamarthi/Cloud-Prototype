using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace Server
{
    public delegate void tcpServerConnectionChanged(ServerConnection connection);
    public partial class ServerCloud : Component
    {
        private List<ServerConnection> connections;
        private TcpListener listener;
        private TcpListener syncListener;
        private Thread listenThread;
        private Thread sendThread;
        private Thread connCheck;
        private Thread syncThread;
        public event tcpServerConnectionChanged OnConnect = null;
        public event tcpServerConnectionChanged OnDataAvailable = null;
        private bool m_isOpen;
        private int m_port;
        private int m_portSync;
        private int m_maxSendAttempts;
        private int m_idleTime;
        private int activeThreads;
        private Encoding m_encoding;
        string ipOtherServer;
        int portOtherServer;

        public ServerCloud()
        {

            initialise();

        }

        public ServerCloud(IContainer container)
        {
            container.Add(this);

            //InitializeComponent();

            initialise();
        }

        private void initialise()
        {
            //Create the logins file
            if (!File.Exists(ConfigurationManager.AppSettings.Get("logInFile")))
            {
                File.Create(ConfigurationManager.AppSettings.Get("logInFile")).Dispose();
            }

            connections = new List<ServerConnection>();
            listener = null;

            listenThread = null;
            sendThread = null;

            m_port = -1;
            m_maxSendAttempts = 3;
            m_isOpen = false;
            m_idleTime = 50;
            //m_maxCallbackThreads = 100;
            //m_verifyConnectionInterval = 100;
            m_encoding = Encoding.ASCII;

            //sem = new SemaphoreSlim(0);
            //waiting = false;

            activeThreads = 0;
        }

        private void syncListner()
        {
            if (!File.Exists(ConfigurationManager.AppSettings.Get("dataFile")))
            {
                File.Create(ConfigurationManager.AppSettings.Get("dataFile")).Dispose();
            }

            while (m_isOpen && m_port >= 0)
            {
                try
                {

                    if (syncListener.Pending())
                    {
                        bool loop = true, wait = true;
                        int read;
                        byte[] okToStart = new byte[1024];
                        byte[] buffer = new byte[1024];
                        byte[] bytesFrom = new byte[10025];
                        string dataFromServer = null;
                        string okToSendString = null;
                        int totalDataToRead = 0; int dataReadTillNow = 0;
                        char[] SplitCharacter = { ';' };
                        TcpClient socket = syncListener.AcceptTcpClient();

                        //DO all the sync stuff here

                        NetworkStream syncData = socket.GetStream();

                        while (wait)
                        {
                            syncData.Read(okToStart, 0, socket.Available);
                            okToSendString = System.Text.Encoding.ASCII.GetString(okToStart, 0, 14);
                            //syncData.Read(okToStart, 0, socket.Available);
                            //okToSendString = System.Text.Encoding.ASCII.GetString(okToStart, 0, 14);

                            if (okToSendString.StartsWith("startSync"))
                            {
                                //string[] h = okToSendString.Split(SplitCharacter);
                                totalDataToRead = Int32.Parse(okToSendString.Split(SplitCharacter)[1]);
                                wait = false;
                            }


                        }
                        StringBuilder data = new StringBuilder();
                        NetworkStream syncData1 = socket.GetStream();
                        //while (loop)
                        //{
                        //int count = syncData.Read(bytesFrom, 0, socket.Available);
                        if (totalDataToRead > 0)
                        {
                            //dataFromServer = System.Text.Encoding.ASCII.GetString(bytesFrom, 0, count);
                            //if(string.Compare(dataFromServer,"startSync")==0)
                            // {
                            bool endLoop = false; string endData;

                            while (((read = syncData1.Read(buffer, 0, socket.Available)) > 0) || (!endLoop))
                            {
                                if (read > 0)
                                {
                                    endData = System.Text.Encoding.ASCII.GetString(buffer, 0, read);

                                    if (!endData.Contains("endSync"))
                                    {
                                        dataReadTillNow = dataReadTillNow + read;
                                        data.Append(System.Text.Encoding.UTF8.GetString(buffer));
                                        Array.Clear(buffer, 0, buffer.Length);
                                    }
                                    else
                                    {
                                        using (System.IO.StreamWriter fileIO = new System.IO.StreamWriter(ConfigurationManager.AppSettings.Get("dataFile")))
                                        {
                                            fileIO.Write(data.ToString().ToCharArray());
                                        }
                                        endLoop = true;
                                    }
                                }

                            }
                            // }
                            //loop = false;
                            // }
                        }
                        else
                        {

                        }


                        //}
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(m_idleTime);
                    }
                }
                catch (ThreadInterruptedException) { } //thread is interrupted when we quit
                catch (Exception e)
                {
                    //if (m_isOpen && OnError != null)
                    //{
                    //    OnError(this, e);
                    //}
                }
            }
        }

        private void runListener()
        {
            while (m_isOpen && m_port >= 0)
            {
                try
                {

                    if (listener.Pending())
                    {
                        TcpClient socket = listener.AcceptTcpClient();


                        Thread newConn = new Thread(() => newConnection(socket));
                        newConn.Start();


                    }
                    else
                    {
                        System.Threading.Thread.Sleep(m_idleTime);
                    }
                }
                catch (ThreadInterruptedException) { } //thread is interrupted when we quit
                catch (Exception e)
                {
                    //if (m_isOpen && OnError != null)
                    //{
                    //    OnError(this, e);
                    //}
                }
            }
        }

        private void newConnection(TcpClient s)
        {
            ServerConnection conn = new ServerConnection(s, m_encoding, ipOtherServer, portOtherServer);
            lock (conn)
            {
                connections.Add(conn);
            }


        }
        private void checkConnOpen()
        {
            while (m_isOpen && m_port >= 0)
            {
                try
                {
                    for (int i = 0; i < connections.Count; i++)
                    {
                        if (connections[i].CallbackThread != null) { }
                        else if (connections[i].connected() && (connections[i].verifyConnected()))
                        {
                            //moreWork = moreWork || processConnection(connections[i]);
                        }
                        else
                        {
                            lock (connections)
                            {
                                connections.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
                catch (Exception e)
                {

                }

            }
        }
        private void runSender()
        {
            while (m_isOpen && m_port >= 0)
            {
                try
                {
                    bool moreWork = false;
                    for (int i = 0; i < connections.Count; i++)
                    {
                        if (connections[i].CallbackThread != null)
                        {
                            try
                            {
                                connections[i].CallbackThread = null;
                                //lock (activeThreadsLock)
                                //{
                                activeThreads--;
                                //}
                            }
                            catch (Exception)
                            {
                                //an exception is thrown when setting thread and old thread hasn't terminated
                                //we don't need to handle the exception, it just prevents decrementing activeThreads
                            }
                        }

                        if (connections[i].CallbackThread != null) { }
                        else if (connections[i].connected() && (connections[i].verifyConnected()))
                        {
                            moreWork = moreWork || processConnection(connections[i]);
                        }
                        else
                        {
                            lock (connections)
                            {
                                connections.RemoveAt(i);
                                i--;
                            }
                        }
                    }

                    if (!moreWork)
                    {
                        System.Threading.Thread.Yield();
                        //lock (sem)
                        //{
                        foreach (ServerConnection conn in connections)
                        {
                            if (conn.hasMoreWork())
                            {
                                moreWork = true;
                                break;
                            }
                        }
                        //}
                        if (!moreWork)
                        {
                            //waiting = true;
                            //sem.Wait(m_idleTime);
                            //waiting = false;
                        }
                    }
                }
                catch (ThreadInterruptedException) { } //thread is interrupted when we quit
                catch (Exception e)
                {
                    //if (m_isOpen && OnError != null)
                    //{
                    //    OnError(this, e);
                    //}
                }
            }
        }

        private bool processConnection(ServerConnection conn)
        {
            bool moreWork = false;
            if (conn.processOutgoing(m_maxSendAttempts))
            {
                moreWork = true;
            }

            if (OnDataAvailable != null && conn.Socket.Available > 0)
            {
                //lock (activeThreadsLock)
                //{
                activeThreads++;
                //}
                conn.CallbackThread = new Thread(() =>
                {
                    OnDataAvailable(conn);
                });
                conn.CallbackThread.Start();
                Thread.Yield();
            }
            return moreWork;
        }
        public void Open()
        {
            lock (this)
            {
                if (m_isOpen)
                {
                    //already open, no work to do
                    return;
                }
                if (m_port < 0)
                {
                    throw new Exception("Invalid port");
                }

                try
                {
                    listener.Start(5);
                    syncListener.Start(5);
                }
                catch (Exception)
                {
                    listener.Stop();
                    listener = new TcpListener(IPAddress.Any, m_port);
                    listener.Start(5);
                    syncListener.Stop();
                    syncListener = new TcpListener(IPAddress.Any, m_portSync);
                    syncListener.Start(5);
                }

                m_isOpen = true;

                listenThread = new Thread(new ThreadStart(runListener));
                listenThread.Start();
                connCheck = new Thread(new ThreadStart(checkConnOpen));
                connCheck.Start();

                syncThread = new Thread(new ThreadStart(syncListner));
                syncThread.Start();
                //sendThread = new Thread(new ThreadStart(runSender));
                //sendThread.Start();
            }
        }

        public void Close()
        {
            if (!m_isOpen)
            {
                return;
            }

            lock (this)
            {
                m_isOpen = false;
                foreach (ServerConnection conn in connections)
                {
                    conn.forceDisconnect();
                }
                try
                {
                    if (listenThread.IsAlive)
                    {
                        listenThread.Interrupt();

                        Thread.Yield();
                        if (listenThread.IsAlive)
                        {
                            listenThread.Abort();
                        }
                    }
                }
                catch (System.Security.SecurityException) { }
                try
                {
                    if (sendThread.IsAlive)
                    {
                        sendThread.Interrupt();

                        Thread.Yield();
                        if (sendThread.IsAlive)
                        {
                            sendThread.Abort();
                        }
                    }
                }
                catch (System.Security.SecurityException) { }
            }
            listener.Stop();

            lock (connections)
            {
                connections.Clear();
            }

            listenThread = null;
            sendThread = null;
            GC.Collect();
        }

        public int Port
        {
            get
            {
                return m_port;
            }
            set
            {
                if (value < 0)
                {
                    return;
                }

                if (m_port == value)
                {
                    return;
                }

                if (m_isOpen)
                {
                    throw new Exception("Invalid attempt to change port while still open.\nPlease close port before changing.");
                }

                m_port = value;
                if (listener == null)
                {
                    //this should only be called the first time.
                    listener = new TcpListener(IPAddress.Any, m_port);
                }
                else
                {
                    listener.Server.Bind(new IPEndPoint(IPAddress.Any, m_port));
                }
                if (syncListener == null)
                {
                    syncListener = new TcpListener(IPAddress.Any, m_portSync);
                }
                else
                {
                    syncListener.Server.Bind(new IPEndPoint(IPAddress.Any, m_portSync));
                }
            }
        }

        public string IpOtherServer
        {

            get
            {
                return ipOtherServer;
            }
            set
            {
                ipOtherServer = value;
            }

        }

        public int PortOtherServer
        {

            get
            {
                return portOtherServer;
            }
            set
            {
                portOtherServer = value;
            }

        }

        public int PortSync
        {
            get
            {
                return m_portSync;
            }
            set
            {
                if (value < 0)
                {
                    return;
                }

                if (m_portSync == value)
                {
                    return;
                }

                //if (m_isOpen)
                //{
                //    throw new Exception("Invalid attempt to change port while still open.\nPlease close port before changing.");
                //}

                m_portSync = value;
                if (syncListener == null)
                {
                    //this should only be called the first time.
                    syncListener = new TcpListener(IPAddress.Any, m_portSync);
                }
                else
                {
                    syncListener.Server.Bind(new IPEndPoint(IPAddress.Any, m_portSync));
                }

            }
        }
    }
}
