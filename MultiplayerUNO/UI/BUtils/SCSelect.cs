using MultiplayerUNO.UI.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerUNO.UI.BUtils {
    /// <summary>
    /// Some configuration information selected by the server and client
    /// </summary>
    public static class SCSelect {
        /// <summary>
        /// Is it server or client
        /// </summary>
        public static PlayerKind PlayerKind = PlayerKind.Client;

        /// <summary>
        /// Default information (domain name, port, name)
        /// </summary>
        public const string DEFAULT_HOST = "127.0.0.1",
                            DEFAULT_PORT = "25564",
                            DEFAULT_NAME = "Alice";

        /// <summary>
        /// Descriptive information (domain name, port)
        /// </summary>
        private static string hostInfo = "domain information", portInfo = "port information";
        public static string HostInfo {
            get =>
                (PlayerKind == PlayerKind.Client ? "server" : "yourself") + hostInfo;
        }
        public static string PortInfo {
            get =>
                (PlayerKind == PlayerKind.Client ? "server" : "yourself") + portInfo;
        }

        /// <summary>
        /// User information (domain name, port, name)
        /// </summary>
        public static string UserHost, UserPort, UserName;
    }
}
