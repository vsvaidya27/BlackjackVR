using System.Collections.Generic;
using UnityEngine;  

public enum Suit
{
    Clubs, Diamonds, Hearts, Spades
}

public enum Rank
{
    Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten,
    Jack = 10, Queen = 10, King = 10, Ace = 11  // Initial values for game logic
}

public class Card
{
    public Suit Suit { get; private set; }
    public Rank Rank { get; private set; }
    public int Value { get; private set; }
    public Sprite Image { get; private set; }  // Assuming Sprite is assigned and managed externally

    // Constructor to initialize a card
    public Card(Suit suit, Rank rank, Sprite image)
    {
        Suit = suit;
        Rank = rank;
        Image = image;
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
