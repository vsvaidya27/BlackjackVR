using UnityEngine;
using UnityEngine.UIElements;

public class Card : MonoBehaviour
{
    // Public variables for the front and back sides of the card
    public GameObject CardPrefab;

    // Private storage for suit and rank
    private string cardSuit;
    private int cardRank; // 1 = Ace, 11 = Jack, 12 = Queen, 13 = King

    // Public getters for suit and rank
    public string CardSuit => cardSuit;
    public int CardRank => cardRank;

    // Flag to determine if the card is face up
    private bool isFaceUp = true;

    // Constructor-like initialization for Unity MonoBehaviour
    public void InitializeCard(GameObject cardPrefab, int rank, string suit)
    {
        CardPrefab = cardPrefab;

        if (rank == 11 || rank == 12 || rank == 13)
        {
            cardRank = 10;
        }
        else
        {
            cardRank = rank;
        }

        cardSuit = suit;
    }

    // Method to flip the card
    public void FlipCard()
    {
        isFaceUp = !isFaceUp;

    }

    public override string ToString()
    {
        string rankString = cardRank switch
        {
            1 => "Ace",
            11 => "Jack",
            12 => "Queen",
            13 => "King",
            _ => cardRank.ToString()
        };

        return $"{rankString} of {cardSuit}";
    }

}
