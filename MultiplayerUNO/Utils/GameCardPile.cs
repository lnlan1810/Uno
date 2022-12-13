using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MultiplayerUNO.Backend;

namespace MultiplayerUNO.Utils
{
    /// <summary>
    /// Handle the deck of a game, including the deck and the discard pile
    /// </summary>
    public class GameCardPile
    {
        private LinkedList<Card> CardPile = new LinkedList<Card>(); // pile of cards
        private LinkedList<Card> DiscardPile = new LinkedList<Card>(); //discard pile

        public int CardPileLeft { get { return CardPile.Count; } }
        public int DiscardPileLeft { get { return DiscardPile.Count; } }

        public GameCardPile()
        {
            for (int i = 0; i < 108; i++)
                CardPile.AddLast(new Card(i)); // Initialization: 108 shuffled cards in the deck
        }

        /// <summary>
        /// shuffle
        /// </summary>
        public void ShuffleCards() {
            //The discard pile is concat into the pile, and then the pile is shuffled
            CardPile = new LinkedList<Card>(CardPile.Concat(DiscardPile).OrderBy(p => Guid.NewGuid().ToString()));
            
            DiscardPile.Clear(); // empty the discard pile
        }

        /// <summary>
        /// draw cards
        /// </summary>
        /// <param name="number">draw number</param>
        /// <returns>drawn cards</returns>
        public Card[] DrawCards(int number)
        {
            if (number < 1) throw new ArgumentOutOfRangeException("number should be greater than 0.");
            if (CardPile.Count + DiscardPile.Count < number) throw new TieExceptions();

            if (CardPile.Count < number) ShuffleCards();

            Card[] res = new Card[number];

            for(int i = 0; i < number; i++)
            {
                res[i] = CardPile.First.Value;
                CardPile.RemoveFirst();
            }

            return res;
        }

        /// <summary>
        /// draw a card
        /// </summary>
        /// <returns>Climb by touch</returns>
        public Card DrawOneCard()
        {
            return DrawCards(1)[0];
        }

        /// <summary>
        /// Put a card into the discard pile
        /// </summary>
        /// <param name="card">the placed card</param>
        public void Move2DiscardPile(Card card)
        {
            DiscardPile.AddLast(card);
        }
    }
}
