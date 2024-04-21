using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    public List<Card> Cards { get; private set; }

    // Constructor initializes and optionally shuffles the deck
    public Deck(bool shuffleDeck = true)
    {
        Cards = new List<Card>();
        CreateFullDeck();
        if (shuffleDeck)
        {
            Shuffle();
        }
    }

    // Creates one card for each combination of Suit and Rank
    private void CreateFullDeck()
    {
        foreach (Suit suit in System.Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank rank in System.Enum.GetValues(typeof(Rank)))
            {
                // Assume cardSprites is a dictionary loaded with all card sprites, keyed by "Rank_of_Suit"
                string key = $"{rank}_of_{suit}";
                Sprite image = Resources.Load<Sprite>($"Cards/{key}");
                if (image == null)
                {
                    Debug.LogError($"Card image not found for {key}");
                    continue;
                }
                Cards.Add(new Card(suit, rank, image));
            }
        }
    }

    // Shuffles the deck of cards
    public void Shuffle()
    {
        System.Random rng = new System.Random();
        int n = Cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card value = Cards[k];
            Cards[k] = Cards[n];
            Cards[n] = value;
        }
    }

    // Deals the top card from the deck, removing it from the deck
    public Card DealTopCard()
    {
        if (Cards.Count == 0)
        {
            Debug.LogError("Attempted to deal from an empty deck.");
            return null;
        }
        Card topCard = Cards[0];
        Cards.RemoveAt(0);
        return topCard;
    }
}
