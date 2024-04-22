using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    public List<Card> Cards { get; private set; }

    // Constructor initializes and optionally shuffles the deck
    public Deck(bool shuffleDeck = true)
    {
        PrintResources();
        Cards = new List<Card>();
        // CreateFullDeck();
        if (shuffleDeck)
        {
            Shuffle();
        }
    }

    void PrintResources()
    {
        Object[] objects = Resources.LoadAll("Prefab/BackColor_Black", typeof(Object));  // Loads all assets in the Resources root
        foreach (var obj in objects)
        {
            Debug.Log("Loaded resource: " + obj.name);
        }

        Debug.LogError("DONE");
    }


    // Creates one card for each combination of Suit and Rank
    private void CreateFullDeck()
    {
        foreach (Suit suit in System.Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank rank in System.Enum.GetValues(typeof(Rank)))
            {
                string rankKey = FormatRankKey(rank);
                string key = $"Black_PlayingCards_{suit}{rankKey}_00";  // Concatenate to form something like Diamond02, Club10
                GameObject cardPrefab = Resources.Load<GameObject>("Prefab/BackColor_Black" + key);
                if (cardPrefab == null) {
                    Debug.LogError("Failed to load prefab at path: " + "Prefab/BackColor_Black" + key);
                }
                if (cardPrefab == null)
                {
                    Debug.LogError($"Card prefab not found for {key}");
                    continue;
                }
                Cards.Add(new Card(suit, rank, cardPrefab));
            }
        }
    }

    private string FormatRankKey(Rank rank)
    {
        switch (rank)
        {
            case Rank.Two:
            case Rank.Three:
            case Rank.Four:
            case Rank.Five:
            case Rank.Six:
            case Rank.Seven:
            case Rank.Eight:
            case Rank.Nine:
                return ((int)rank).ToString("D2");  // Formats the number as two digits (e.g., 2 becomes 02)
            case Rank.Ten:
                return "10";
            case Rank.Jack:
                return "11";  // Assuming your Jack, Queen, King, Ace are saved as Jack10, Queen10, etc.
            case Rank.Queen:
                return "12";
            case Rank.King:
                return "13";
            case Rank.Ace:
                return "01"; 
            default: 

                return "00";
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
