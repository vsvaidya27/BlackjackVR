using System.Collections.Generic;
using UnityEngine;  

public enum Suit
{
    Club, Diamond, Heart, Spade
}

public enum Rank
{
    Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten,
    Jack, Queen, King, Ace  // Initial values for game logic
}

public class Card
{
    public Suit Suit { get; private set; }
    public Rank Rank { get; private set; }
    public int Value { get; private set; }
    public GameObject CardPrefab { get; private set; }

    // Constructor to initialize a card
    public Card(Suit suit, Rank rank, GameObject cardPrefab )
    {
        Suit = suit;
        Rank = rank;
        CardPrefab = cardPrefab; 
        AssignValue();  // Set the card's value based on its rank
    }

    private void AssignValue()
    {
        if (Rank >= Rank.Two && Rank <= Rank.Ten)
        {
            Value = (int)Rank;  // Value is the same as the rank for number cards
        }
        else if (Rank == Rank.Jack || Rank == Rank.Queen || Rank == Rank.King)
        {
            Value = 10;  // Face cards are worth 10
        }
        else if (Rank == Rank.Ace)
        {
            Value = 11;  // Ace is worth 11, but can be adjusted to 1 based on game situation
        }
    }

    public override string ToString()
    {
        return $"{Rank} of {Suit} (Value: {Value})";
    }
}
