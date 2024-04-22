using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    private Deck deck;
    private Player player;
    private Player dealer;
    private bool playerTurn = true;  // Track whose turn it is

    void Start()
    {
        deck = new Deck();  // Create and shuffle a new deck
        player = new Player();
        dealer = new Player();  // Could customize a 'Dealer' class that inherits 'Player' if dealer rules differ
        Debug.LogError("HIT");
        InitialDeal();
    }
/*
    void Update()
    {
        if (playerTurn)
        {
            // Handle player input for actions (e.g., hit, stand, split)
            // This would likely involve listening for input events or UI button presses
        }
        else
        {
            // Dealer's logic
            DealerPlay();
        }
    }
*/
    private void InitialDeal()
    {
        for (int i = 0; i < 2; i++)  // Deal two cards to each player initially
        {
            player.AddCard(deck.DealTopCard());
            dealer.AddCard(deck.DealTopCard());

        }
        Debug.LogError("Player Hand: " + player.ToString());
        Debug.LogError("DEALER Hand: " + dealer.ToString());


        if (player.CanSplit())  // Check if the player can split their hand
        {
            // Provide option to split via UI
        }

        CheckForBlackjack();
    }

    private void DealerPlay()
    {
        while (DealerShouldHit())  // Basic dealer logic to hit until a certain condition
        {
            dealer.AddCard(deck.DealTopCard());
        }

        EndGame();
    }

    private bool DealerShouldHit()
    {
        // Example condition: dealer hits until reaching 17
        return CalculateHandValue(dealer.Hands[0]) < 17;
    }

    private void EndGame()
    {
        // Compare hands to determine winner
        // Update game state and UI accordingly
    }

    private void CheckForBlackjack()
    {
        if (CalculateHandValue(player.Hands[0]) == 21)
        {
            Debug.Log("Blackjack! Player wins!");
            EndGame();
        }
        else if (CalculateHandValue(dealer.Hands[0]) == 21)
        {
            Debug.Log("Blackjack! Dealer wins!");
            EndGame();
        }
    }

    private int CalculateHandValue(List<Card> hand)
    {
        int value = 0;
        int aceCount = 0;

        foreach (Card card in hand)
        {
            value += card.Value;
            if (card.Rank == Rank.Ace) aceCount++;
        }

        while (value > 21 && aceCount > 0)
        {
            value -= 10;  // Adjusting Ace from 11 to 1 if total value exceeds 21
            aceCount--;
        }

        return value;
    }
}
