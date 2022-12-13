using MultiplayerUNO.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerUNO.Backend.Player
{
    /// <summary>
    ///  Chỉ có một người chơi máy chủ mặt đất trong phòng
    /// </summary>
    public class LocalPlayer : Player
    {
        protected LocalPlayerAdapter adapter;

        public LocalPlayer(LocalPlayerAdapter localPlayerAdapter)
        {
            adapter = localPlayerAdapter;
            name = adapter.PlayerName;
            sendQueue = adapter.RecvQueue; // Máy chủ trực tiếp gửi đến địa phương tương ứng ReceiveQueue
            //Сервер напрямую отправляет на соответствующий локальный ReceiveQueue
        }
    }
}
