using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiplayerUNO.Utils
{
    /// <summary>
    /// The interface between the front end and the back end
    /// Provide the RecvQueue property for the front end to extract server information
    /// Provide the SendMsg2Server method for the front end to send data to the server
    /// </summary>
    public abstract class PlayerAdapter
    {
        protected BlockingCollection<string> sendQueue;
        protected BlockingCollection<string> recvQueue;
        
        public BlockingCollection<string> RecvQueue { get
            {
                return recvQueue;
            }
        } // Messages received from Server will be stored in this queue

        protected Thread sendThread;
        protected Thread recvThread;

        public abstract void Initialize();
        public abstract void SendMsg2Server(string msg);
    }
}
