using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace MultiplayerUNO.Utils
{
    /// <summary>
    /// Remotely connect to other people's servers Front-end interface
    /// </summary>
    class RemotePlayerAdapter : PlayerAdapter
    {
        protected string _hostname;
        protected int _port;
        protected Socket serverSocket;

        public static int BUFFERSIZE = 8192;

        /// <summary>
        /// For remote connection, you need to specify the host and port
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        public RemotePlayerAdapter(string hostName, int port)
        {
            _hostname = hostName;
            _port = port;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        ///The initialization operation of the remote player needs to establish a tcp connection and enable the receiving and sending threads
        /// </summary>
        public override void Initialize()
        {
            serverSocket.Connect(_hostname, _port); // connect to the server

            sendQueue = new BlockingCollection<string>();
            recvQueue = new BlockingCollection<string>();

            // client send thread
            sendThread = new Thread(() =>
            {
                while (true)
                {
                    string msg = null;
                    try
                    {
                        msg = sendQueue.Take(); // Get the information to be sent
                    }
                    catch (InvalidOperationException e)
                    {
                        Console.WriteLine(e.Message);
                        break;
                    }

                    if (msg == null) continue;

                    try
                    {
                        byte[] contents = Encoding.UTF8.GetBytes(msg + "$");
                        serverSocket.Send(contents); // send to server
                    }catch(ObjectDisposedException e)
                    {
                        Console.WriteLine(e.Message);
                        break;
                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine(e.Message);
                        break;
                    }
                }
            });
            sendThread.IsBackground = true; // set as background thread

            // client receive thread
            recvThread = new Thread(() =>
            {
                while (true)
                {
                    byte[] content = new byte[BUFFERSIZE];
                    string msg = null;
                    try
                    {
                        int n = serverSocket.Receive(content); // Receive data from server
                        msg = Encoding.UTF8.GetString(content, 0, n);
                        
                    }catch(ObjectDisposedException e)
                    {
                        Console.WriteLine(e.Message);
                        recvQueue.CompleteAdding();
                        Close();
                        break;
                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine(e.Message);
                        recvQueue.CompleteAdding();
                        Close();
                        break;
                    }

                    if (msg != null)
                    {
                        foreach(string sw in msg.Split('$')){
                            if (sw.Length <= 0) continue;
                            recvQueue.Add(sw); // After splitting, it is stored in the collection queue for the front-end Take
                        }
                    }

                }
            });
            recvThread.IsBackground = true; // set to background thread

            sendThread.Start();
            recvThread.Start();
        }

        /// <summary>
        /// internal call to close the connection
        /// </summary>
        protected void Close()
        {
            serverSocket.Close();
            sendQueue.CompleteAdding();
        }

        /// <summary>
        /// Called by the front end to close the connection
        /// </summary>
        public void Disconnect()
        {
            serverSocket.Close();
            recvQueue.CompleteAdding();
            sendQueue.CompleteAdding();
        }

        /// <summary>
        /// The remote player sends a message, and stores the message in the sending queue for the sending thread to retrieve
        /// </summary>
        /// <param name="msg">message to send</param>
        public override void SendMsg2Server(string msg)
        {
            sendQueue.Add(msg);
        }
    }
}
