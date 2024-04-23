using System.Collections.Generic;
using UnityEngine;

public class Player: MonoBehaviour
{
    public List<List<Card>> Hands { get; private set; }
    public int ActiveHandIndex { get; private set; }  // Instance variable to keep track of the current active hand

    public Player()
    {
        Hands = new List<List<Card>> { new List<Card>() };  // Start with one empty hand
        ActiveHandIndex = 0;  // Initialize the active hand index to the first hand
    }

    // Adds a card to the specified hand
    public void AddCard(Card card, int handIndex = 0)
    {
        if (handIndex < Hands.Count)
        {
            Hands[handIndex].Add(card);
            ActiveHandIndex = handIndex;  // Update the active hand index whenever a card is added
        }
        else
        {
            Debug.LogError("Invalid hand index.");
        }
    }

    // Checks if the player can split the specified hand
    public bool CanSplit(int handIndex = 0)
    {
        if (handIndex >= Hands.Count || Hands[handIndex].Count != 2)
        {
            return false;  // Can't split if the hand doesn't exist or doesn't have exactly two cards
        }

        Card firstCard = Hands[handIndex][0];
        Card secondCard = Hands[handIndex][1];
        return firstCard.CardRank == secondCard.CardRank;  // Can split if both cards have the same rank
    }

    // Splits the specified hand into two hands
    public void Split(int handIndex = 0)
    {
        if (!CanSplit(handIndex))
        {
            Debug.LogError("Cannot split this hand.");
            return;
        }

        List<Card> handToSplit = Hands[handIndex];
        List<Card> newHand = new List<Card> { handToSplit[1] };  // Create a new hand with the second card
        handToSplit.RemoveAt(1);  // Remove the second card from the original hand

        Hands.Add(newHand);  // Add the new hand to the player's hands
        ActiveHandIndex = Hands.Count - 1;  // Update the active hand index to the new hand
    }

    // Resets the player's hands and active hand index for a new round
    public void ResetForNewRound()
    {
        Hands.Clear();
        Hands.Add(new List<Card>());  // Start with one empty hand again
        ActiveHandIndex = 0;  // Reset the active hand index to the first hand
    }

    // Returns a string representation of the player's hands
    public override string ToString()
    {
        string playerHands = "";
        for (int i = 0; i < Hands.Count; i++)
        {
            playerHands += $"Hand {i + 1}: [";
            foreach (Card card in Hands[i])
            {
                if (card == null)
                {
                    Debug.LogError("CARD IS NULL");
                }
                playerHands += $"{card.ToString()}, ";
            }
            playerHands = playerHands.TrimEnd(',', ' ') + "]\n";
        }
        return playerHands;
    }
}