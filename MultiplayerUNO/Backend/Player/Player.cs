using LitJson;
using MultiplayerUNO.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerUNO.Backend.Player
{
    /// <summary>
    ///The abstract class for processing player information at the back end of the game.
    /// The actual processing process does not consider whether the player is local or remote
    /// </summary>
    public abstract class Player
    {
        protected string name;

        public int isRobot; //Is it taken over by AI
        public string Name { get { return name; } } //player name
        public int seatID;  // The player's UID, used for client identification
        public int ingameID;    //The ID of the player in a certain game, representing the seat

        public BlockingCollection<string> sendQueue;  // The backend processing thread will

        /// <summary>
        ///Called by the backend to send a json string to the corresponding player
        /// </summary>
        /// <param name="msg">the JSON string to send</param>
        public void SendMessage(string msg)
        {
            if (isRobot == 1) return;
            sendQueue.Add(msg);
        }


        protected bool isReady; /// Are the players ready ?
        public bool IsReady
        {
            get { return isReady; }
            set {
                isReady = value;
                playerJsonCache = null; // Trạng thái thay đổi, làm mất hiệu lực bộ đệm Json của trình phát
                //Состояние изменяется, что делает недействительным кеш Json игрока.
            }
        }

        protected JsonData playerJsonCache = null; //Bộ đệm json của trình phát, vì trạng thái trình phát thay đổi không thường xuyên,
                                                   ////bộ đệm được giới thiệu

        /// <summary>
        /// /Khi phòng chờ, lấy thông tin trạng thái người chơi Json
        ///Когда комната ждет, получить информацию о статусе игрока Json
        /// </summary>
        /// <param name="ignoreCache">Có nên bỏ qua việc làm mới lực lượng bộ đệm hay không</param>
        /// <returns>player state json</returns>
        public JsonData GetPlayerJson(bool ignoreCache = false)
        {
            if (!ignoreCache && playerJsonCache != null) return playerJsonCache;

            JsonData json = new JsonData();
            json["seatID"] = seatID;
            json["name"] = name;
            json["isReady"] = IsReady;

            playerJsonCache = json;
            return playerJsonCache;
        }

        public LinkedList<Card> handCards = new LinkedList<Card>(); 

        /// <summary>
        ///Backend call, reset player related information when the game starts
        /// </summary>
        public void StartGameReset()
        {
            IsReady = false;
            handCards.Clear();
        }

        /// <summary>
        /// Player gets a hand card
        /// </summary>
        /// <param name="card">acquired cards</param>
        public void GainCard(Card card)
        {
            handCards.AddLast(card);
        }

        /// <summary>
        /// Player gets multiple hands at once
        /// </summary>
        /// <param name="cards">acquired cards</param>
        public void GainCard(Card[] cards)
        {
            foreach (Card card in cards)
                handCards.AddLast(card);
        }

        /// <summary>
        ///Make the player lose a card based on the number of the card
        /// </summary>
        /// <param name="cardID">The number of the card that will be lost</param>
        /// <returns>whether succeed</returns>
        public bool RemoveCardById(int cardID)
        {
            Card cardObj = null;
            foreach(Card card in handCards)
            {
                if(card.CardId == cardID)
                {
                    cardObj = card;
                    break;
                }
            }

            if (cardObj == null) return false;
            return handCards.Remove(cardObj);
        }

        /// <summary>
        /// Causes the player to lose a card
        /// </summary>
        /// <param name="card">cards to lose</param>
        /// <returns>whether succeed</returns>
        public bool RemoveCard(Card card)
        {
            bool res = handCards.Remove(card);
            // if (handCards.Count <= 0) throw new PlayerFinishException(this);
            return res;
        }

        /// <summary>
        /// Construct PlayerMap Json
        /// </summary>
        /// <param name="containsHandcards">Có chứa thông tin ván bài của người chơi hay không  Содержать ли информацию о руке игрока</param>
        /// <returns>The constructed Json</returns>
        public JsonData BuildPlayerMapJson(bool containsHandcards)
        {
            JsonData json = new JsonData
            {
                ["name"] = name,
                ["playerID"] = ingameID,
                ["cardsCount"] = handCards.Count,
                ["isRobot"] = isRobot
            };
            if (containsHandcards) 
            {
                json["handcards"] = new JsonData();
                json["handcards"].SetJsonType(JsonType.Array);
                foreach(Card card in handCards)
                    json["handcards"].Add(card.CardId);
            }

            return json;
        }

        /// <summary>
        /// Phát hiện nếu người chơi không có thẻ nào khác và chỉ có thể chơi thẻ +4
        /// Определите, нет ли у игрока других карт и может ли он играть только +4 карты
        /// </summary>
        /// <param name="lastCard">previous card</param>
        /// <param name="colorID">color of previous card</param>
        /// <returns>Is it possible to play</returns>
        public bool CanHandoutPlus4(Card lastCard, int colorID)
        {
            foreach(Card card in handCards)
            {
                if (card.CardId >= 104) continue;
                if (lastCard == null) return false;
                if (card.CanResponseTo(lastCard, (Card.CardColor)colorID)) return false;
            }

            return true;
        }
    }
}
