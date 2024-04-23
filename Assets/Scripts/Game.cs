using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Player player;
    public Player dealer;
    public Deck deck;
    private bool gameRoundEnded = false;

    void Start()
    {
        SetupGame();
    }

    void SetupGame()
    {
        GameObject deckObject = new GameObject("Deck");
        deck = deckObject.AddComponent<Deck>();

        GameObject playerObject = new GameObject("Player");
        player = playerObject.AddComponent<Player>();

        GameObject dealerObject = new GameObject("Dealer");
        dealer = dealerObject.AddComponent<Player>();

        PlayHand();
    }

    void PlayerDecision() //I am not sure how we are detecting with gesture, so this is probably wrong. 
    {

        // HIT 
        if (GestureDetector.DetectHitGesture())
        {
            Hit(player);
        }

        // STAND
        else if (GestureDetector.DetectStandGesture())
        {
            Stand();
        }

        // SPLIT
        else if (GestureDetector.DetectSplitGesture())
        {
            Split(player);
        }
    }

    void PlayHand()
    {

        // MODIFY TO RECEIVE USER INPUT FOR BET AMOUNT
        DealInitialCards();
        // Wait for player actions triggered by gestures detected in Update()


        // PLAYER DECISION

        // ENDPLAYER TURN

        // EVALUATE HANDS

        // RESET HAND DETAILS, PLAY A NEW ONE 
    }

    void DealInitialCards()
    {
        player.AddCard(deck.GetTopCard());
        dealer.AddCard(deck.GetTopCard());
        player.AddCard(deck.GetTopCard());
        dealer.AddCard(deck.GetTopCard());

        Debug.Log("Player hand: " + player.ToString());
        Debug.Log("Dealer hand: " + dealer.ToString());
    }

    public void Hit(Player player)
    {
        if (player.Busted || gameRoundEnded)
            return;

        Card newCard = deck.GetTopCard();
        player.AddCard(newCard);
        Debug.Log("Player hits: " + player.ToString());
        AnimateCard(newCard, player);

        if (player.HandValue > 21)
        {
            player.Busted = true;
            Debug.Log("Player busts with hand: " + player.ToString());
            EndPlayerTurn();
        }
    }

    public void Stand()
    {
        Debug.Log("Player stands.");
        EndPlayerTurn();
    }

    void EndPlayerTurn()
    {
        if (!player.Busted)
        {
            DealerTurn();
        }
    }

    public void Split(Player player)
    {
        if (player.CanSplit())
        {
            player.Split(); //will check this as well. 
            Debug.Log("Player splits.");
            // Additional logic for playing split hands may be added here
        }
    }

    void DealerTurn()
    {
        while (dealer.HandValue < 17)
        {
            Card newCard = deck.GetTopCard();
            dealer.AddCard(newCard);
            AnimateCard(newCard, dealer);
        }
        EvaluateHands();
    }

    void EvaluateHands()
    {
        if (player.Busted)
        {
            Debug.Log("Dealer wins by default.");
        }
        else if (dealer.HandValue > 21 || player.HandValue > dealer.HandValue)
        {
            Debug.Log("Player wins.");
        }
        else if (player.HandValue < dealer.HandValue)
        {
            Debug.Log("Dealer wins.");
        }
        else
        {
            Debug.Log("Push.");
        }
        ResetForNextHand();
    }

    void ResetForNextHand()
    {
        player.ResetForNewRound();
        dealer.ResetForNewRound();
        deck.ShuffleDeck();
        gameRoundEnded = false;
    }

    void AnimateCard(Card card, Player player)
    {
        // Assume some animation logic here
    }
}
