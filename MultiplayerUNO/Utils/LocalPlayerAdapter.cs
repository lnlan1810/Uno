using MultiplayerUNO.Backend;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LitJson;

namespace MultiplayerUNO.Utils
{
    /// <summary>
    /// The front-end interface used by local players
    /// </summary>
    public class LocalPlayerAdapter : PlayerAdapter
    {
        protected Room gameRoom;
        public IPEndPoint EndPoint { get; }
        public string PlayerName { get; }

        /// <summary>
        ///Open the server locally and create the interface of the room
        /// </summary>
        /// <param name="port">port</param>
        /// <param name="initJson">The json used by players to greet</param>
        public LocalPlayerAdapter(int port, string initJson)
        {
            PlayerName = (string)JsonMapper.ToObject(initJson)["name"];

            recvQueue = new BlockingCollection<string>();
            EndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            gameRoom = new Room(this);  // create room
        }

        /// <summary>
        /// Initialization, for local server developers, the room needs to be initialized
        /// </summary>
        public override void Initialize()
        {
            gameRoom.InitializeRoom();
        }

        /// <summary>
        /// For local server openers, only need to store the message in the information processing queue of the room
        /// </summary>
        /// <param name="msg">message json</param>
        public override void SendMsg2Server(string msg)
        {
            gameRoom.InfoQueue.Add(new Room.MsgArgs()
            {
                msg = msg,
                player = gameRoom.LocalPlayer
            });
        }
    }
}
