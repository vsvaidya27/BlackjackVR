using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public List<(List<Card> Cards, bool IsStanding)> Hands { get; private set; }
    public int ActiveHandIndex { get; private set; }  // Instance variable to keep track of the current active hand

    public Player()
    {
        Hands = new List<(List<Card>, bool)> { (new List<Card>(), false) };
        ActiveHandIndex = 0;
    }

    public void SetActiveHand(int index)
    {
        if (index < 0 || index >= Hands.Count)
        {
            Debug.LogError("Invalid hand index.");
            return;
        }
        ActiveHandIndex = index;
    }

    public void AddCard(Card card, int handIndex = 0)
    {
        if (handIndex < Hands.Count)
        {
            Hands[handIndex].Cards.Add(card);
        }
        else
        {
            Debug.LogError("Invalid hand index.");
        }
    }

    public bool CanSplit(int handIndex = 0)
    {
        var hand = Hands[handIndex];
        return hand.Cards.Count == 2 && hand.Cards[0].CardRank == hand.Cards[1].CardRank;
    }

    public void Split(int handIndex = 0)
    {
        if (!CanSplit(handIndex))
        {
            Debug.LogError("Cannot split this hand.");
            return;
        }

        var handToSplit = Hands[handIndex];
        List<Card> newHandCards = new List<Card> { handToSplit.Cards[1] };
        handToSplit.Cards.RemoveAt(1);

        Hands.Add((newHandCards, false));
    }

    public void ResetForNewRound()
    {
        foreach (var hand in Hands)
        {
            foreach (var card in hand.Cards)
            {
                // Set the card's GameObject to inactive
                if (card.CardPrefab != null)
                    card.CardPrefab.SetActive(false);
            }
        }

        // Clear the hands after deactivating the card objects
        Hands.Clear();
        Hands.Add((new List<Card>(), false));
        ActiveHandIndex = 0;
    }

    public override string ToString()
    {
        string playerHandRepresentation = "[";

        // Assuming `ActiveHandIndex` is the index of the currently active hand
        var activeHand = Hands[ActiveHandIndex];

        foreach (Card card in activeHand.Cards)
        {
            playerHandRepresentation += $"{card.CardRank}, ";
        }

        // Trim the trailing comma and space, then close the bracket
        playerHandRepresentation = playerHandRepresentation.TrimEnd(',', ' ') + "]";

        return playerHandRepresentation;
    }


    public int GetCurrentHandValue()
    {
        var currentHand = Hands[ActiveHandIndex].Cards;
        int totalValue = 0;
        int aceCount = 0;

        foreach (Card card in currentHand)
        {
            int cardValue = card.CardRank == 1 ? 11 : card.CardRank;  // Default Ace to 11
            if (card.CardRank == 1) aceCount++;
            totalValue += cardValue;
        }

        while (totalValue > 21 && aceCount > 0)
        {
            totalValue -= 10;  // Convert an Ace from 11 to 1
            aceCount--;
        }

        return totalValue;
    }
}



