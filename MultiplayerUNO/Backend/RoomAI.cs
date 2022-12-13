using LitJson;
using MultiplayerUNO.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerUNO.Backend
{
    public partial class Room
    {
        /// <summary>
        /// AI playing cards
        /// </summary>
        protected JsonData AutoPseudoActPlayer(Card lastCard, Player.Player turnPlayer, bool ai = false)
        {
            if(currentStatus == GameStatus.Plus4Loop)
            {
                return new JsonData // do not question
                {
                    ["state"] = 4,
                    ["queryID"] = queryID
                };
            }

            if(currentStatus == GameStatus.Plus2Loop)
            {
                return AutoPlus2Action(ai);
            }


            JsonData json = new JsonData
            {
                ["state"] = 2,
                ["queryID"] = queryID
            };  // Not by default

            if (!ai) return json;

            // Chiến lược AI: thoát ra nếu bạn có thể

            Card intendCard = null;
            if(lastCard != null)
            {
                foreach (Card card in turnPlayer.handCards)
                {
                    if (card.CanResponseTo(lastCard))
                    {
                        intendCard = card;
                        break;
                    }
                }
            }
            else
            {
                intendCard = turnPlayer.handCards.First.Value;
            }

            if (intendCard == null) return json; //Không có thẻ để chơi Нет карты для игры

            json["state"] = 1;  // Есть карты
            json["card"] = intendCard.CardId;
            json["color"] = intendCard.Color == Card.CardColor.Invalid ? intendCard.CardId & 3 : -1;
            return json;
        }

        /// <summary>
        /// Có phản hồi với AI sau khi chạm vào thẻ hay không
        /// Отвечать ли ИИ после прикосновения к карте
        /// </summary>
        protected JsonData AutoResponseOrNot(bool ai = false)
        {
            JsonData json = new JsonData
            {
                ["state"] = 1,
                ["action"] = 0,
                ["color"] = 0,
                ["queryID"] = queryID
            };
            // Không theo mặc định
            //Не по умолчанию
            if (!ai || (lastCard != null && !gainCard.CanResponseTo(lastCard, (Card.CardColor)(lastCardInfo & 3)))) return json;
            // Chiến lược AI: thoát ra nếu bạn có thể
            //Стратегия AI: уходи, когда можешь
            json["action"] = 1;
            if (gainCard.Color == Card.CardColor.Invalid) json["color"] = gainCard.CardId & 3;
            return json;
        }

        /// <summary>
        /// AI в стеке +2
        /// </summary>
        protected JsonData AutoPlus2Action(bool ai = false)
        {
            JsonData json = new JsonData
            {
                ["state"] = 4,
                ["queryID"] = queryID
            }; // Theo mặc định, không có thẻ nào được chấp nhận
            //По умолчанию карты не принимаются


            if (!ai) return json;

            // Chiến lược AI: thoát ra nếu bạn có thể
            //Стратегия AI: уходи, когда можешь
            Card intendCard = null;
            foreach (Card card in currentPlayerNode.Value.handCards)
            {
                if (card.Number == 11) // +2 карты
                {
                    intendCard = card;
                    break;
                }
            }
            if (intendCard == null) return json; //Нет карты для игры

            //+2 xếp chồng lên nhau
            //+2 сложены
            json["state"] = 3;// Есть карты
            json["card"] = intendCard.CardId;
            return json;
        }
    }
}
