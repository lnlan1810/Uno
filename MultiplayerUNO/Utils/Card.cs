using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerUNO.Utils
{
    /// <summary>
    /// represents a card
    /// </summary>
    public class Card
    {
        public enum CardColor
        {
            Invalid = -1,
            Red,
            Yellow,
            Green,
            Blue
        }

        public Card(int cardId)
        {
            CardId = cardId;
            if (cardId < 4 || cardId >= 104) Color = CardColor.Invalid; 
            else Color = (CardColor)(cardId & 3);
        }

        public CardColor Color { get; }
        public int CardId { get; }
        public int Number
        {
            get
            {
                if (CardId < 4) return -1; //Wild card, Number is meaningless
                if (CardId >= 104) return 0xf; // +4 cards, returns 15
                return CardId >> 3; //For the rest, refer to the manual
            }
        }

        /// <summary>
        /// Can the card correspond to
        /// </summary>
        /// <param name="newCard">responding card</param>
        /// <param name="oldCard">responded card</param>
        /// <param name="color">Valid only when the color of the responding card (Universal/+4)</param>
        /// <returns></returns>
        public static bool CanResponseTo(Card newCard, Card oldCard, CardColor color = CardColor.Invalid)
        {
            if (newCard.Color == CardColor.Invalid) return true; // Magnum and +4 can respond to any card
            if (newCard.Color == oldCard.Color) return true; // consistent color
            if (oldCard.Color == CardColor.Invalid && newCard.Color == color) return true; ///universal/+4 as declared
            return newCard.Number == oldCard.Number; // Numbers/patterns, universal and +4 have returned true directly in the first if
        }

        /// <summary>
        /// Is it possible to respond to another card
        /// </summary>
        /// <param name="oldCard">responded card</param>
        /// <param name="color">Valid only when the color of the responding card (Universal/+4)</param>
        /// <returns></returns>
        public bool CanResponseTo(Card oldCard, CardColor color = CardColor.Invalid)
        {
            return CanResponseTo(this, oldCard, color);
        }

        /// <summary>
        /// is it +4
        /// </summary>
        /// <returns></returns>
        public bool IsPlus4()
        {
            return CardId >= 104;
        }
        /// <summary>
        /// is it +2
        /// </summary>
        /// <returns></returns>
        public bool IsPlus2() {
            return (CardId >> 3) == 11;
        }
        /// <summary>
        /// Is it a flip?
        /// </summary>
        /// <returns></returns>
        public bool IsReverse() {
            return (CardId >> 3) == 12;
        }
    }
}
