using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Player player;
    public Player dealer;
    public Deck deck;

    private bool isPlayerWin = false;
    private bool isDealerWin = false;
    private bool isPush = false;
    private bool isBlackjack = false;

    public bool splitDetected;
    public bool hitDetected;
    public bool standDetected;

    private Vector3 initialPlayerCardPosition = new Vector3(-2.2f, 10.2f, 226.8f);
    private Vector3 initialDealerCardPosition = new Vector3(24.3f, 10.5f, 100.4f);
    private float playerSpawnZOffset = -20f;  // Offset for each new card in the x and y directions
    private float playerSpawnYOffset = 0.3f;  // Offset for each new card in the x and y directions
    private float playerSpawnXOffset = -15f;  // Offset for each new card in the x and y directions
    private float dealerSpawnXOffset = -15f;
    private Vector3 playerCardSpawnOffset;
    private Vector3 dealerCardSpawnOffset;


    public SpawnCards spawnCards;


    void Start()
    {

        playerCardSpawnOffset = new Vector3(playerSpawnXOffset, playerSpawnYOffset, playerSpawnZOffset);
        dealerCardSpawnOffset = new Vector3(dealerSpawnXOffset, 0f, 0f);

        GameObject deckObject = new GameObject("Deck");
        deck = deckObject.AddComponent<Deck>();

        GameObject playerObject = new GameObject("Player");
        player = playerObject.AddComponent<Player>();

        GameObject dealerObject = new GameObject("Dealer");

        dealer = dealerObject.AddComponent<Player>();
        SetupGame();
    }

    void SetupGame()
    {

        splitDetected = false;
        hitDetected = false;
        standDetected = false;

        Debug.LogError("GAME SETUP DONE");
        StartCoroutine(PlayFullGame());

    }

    void DealerTurn()
    {
        while (dealer.GetCurrentHandValue() < 17)
        {
            Card newCard = deck.GetTopCard();
            SpawnCard(newCard, initialDealerCardPosition);
            dealer.AddCard(newCard);
            // AnimateCard(newCard, dealer);
        }
    }

    void EvaluateHands()
    {


        if (player.GetCurrentHandValue() > 21)
        {
            Debug.LogError("DEBUG DEALER WIN");

            isDealerWin = true;
            return;
        }


        else if (player.GetCurrentHandValue() == dealer.GetCurrentHandValue())
        {

            Debug.LogError("DEBUG PUSH");

            isPush = true;
            return;
        }

        else if (player.GetCurrentHandValue() == 21)
        {
            Debug.LogError("DEBUG BJ");

            isBlackjack = true;
            return;
        }
        else
        {
            Debug.LogError("DEBUG PLAYER WIN");

            isPlayerWin = true;
            return;
        }
    }

    void ResetForNextRound()
    {
        splitDetected = false;
        hitDetected = false;
        standDetected = false;

        isPlayerWin = false;
        isDealerWin = false;
        isPush = false;
        isBlackjack = false;

        dealer.ResetForNewRound();
        player.ResetForNewRound();

    }


    // DEBUGGING VERSION 
    void EndGameUpdates()
    {

        if (isPush)
        {
        }

        else if (isPlayerWin || isBlackjack)
        {
        }

        else if (isDealerWin)
        {

        }

    }


    IEnumerator PlayFullGame()
    {
        // Assume initial setup and betting here
        DealInitialCards();

        SpawnCardsForGame(player);
        SpawnCardsForGame(dealer, true);


        Debug.LogError("DEBUG PLAYER STARTING HAND " + player.ToString());
        Debug.LogError("DEBUG DEALER STARTING HAND " + dealer.ToString());

        for (int i = 0; i < player.Hands.Count; i++)
        {

            player.SetActiveHand(i);

            while (player.GetCurrentHandValue() < 21)
            {
                yield return StartCoroutine(WaitForPlayerDecision());
                ProcessPlayerDecision();


                // Debug.LogError("DEBUG PLAYER HAND AFTER DECISION" + player.ToString());
                // Debug.LogError("DEBUG DECISION STAND CHECKER: " + player.Hands[player.ActiveHandIndex].IsStanding);
                // Debug.LogError("DEBUG BREAK: " + (player.GetCurrentHandValue() >= 21 || player.Hands[player.ActiveHandIndex].IsStanding));


                splitDetected = false;
                hitDetected = false;
                standDetected = false;

                if (player.GetCurrentHandValue() >= 21 || player.Hands[player.ActiveHandIndex].IsStanding) break;

            }
        }

        Debug.LogError("DEBUG DEALER IS ABOUT TO PLAY" + player.ToString());
        DealerTurn();
        Debug.LogError("DEBUG DEALER AFTER PLAYING HAND " + dealer.ToString());


        EvaluateHands();

        // REGISTER ROUND UPDATES - DISPLAY WINNER - PAYOUT ETC.
        EndGameUpdates();

        ResetForNextRound();
    }


    public void SpawnCardsForGame(Player player, bool isDealer = false)
    {
        // Start position for the player's cards

        if (!isDealer)
        {

            Vector3 spawnPosition = initialPlayerCardPosition;

            foreach (Card card in player.Hands[player.ActiveHandIndex].Cards)
            {
                SpawnCard(card, spawnPosition);
                spawnPosition += playerCardSpawnOffset; // Move the position for the next card
            }

            initialPlayerCardPosition = spawnPosition;

        }
        else
        {
            int cardIndex = 0;
            Vector3 spawnPosition = initialDealerCardPosition;

            // Vector3 spawnPosition = initialDealer;
            // You can adjust this for the dealer's logic if needed
            foreach (Card card in player.Hands[player.ActiveHandIndex].Cards)
            {
                bool flipped = (cardIndex == 1);
                SpawnCard(card, spawnPosition, flipped);
                spawnPosition += dealerCardSpawnOffset; // Adjust position similarly for dealer's cards
                cardIndex += 1;
            }
            initialDealerCardPosition = spawnPosition;
        }
    }

    public void SpawnCard(Card card, Vector3 position, bool flipped = false)
    {
        if (card != null)
        {
            Debug.Log($"Spawning card: {card.ToString()} at position {position}");

            // Set the rotation based on the flipped flag
            Quaternion rotation = flipped ? Quaternion.Euler(0, 0, 180) : Quaternion.identity;

            // Move the card to the specified position with the given rotation
            spawnCards.MoveCard(card, position, rotation, 1.0f); // Modified to include rotation handling
        }
        else
        {
            Debug.LogError("Attempted to spawn a null card.");
        }
    }



    IEnumerator WaitForPlayerDecision()
    {
        yield return new WaitUntil(() => splitDetected || hitDetected || standDetected);
        // Reset detection flags here if necessary
    }

    void ProcessPlayerDecision()
    {
        Debug.LogError("DEBUG DECISION IS SPLIT = " + splitDetected);
        Debug.LogError("DEBUG DECISION IS HIT = " + hitDetected);
        Debug.LogError("DEBUG DECISION IS STAND = " + standDetected);



        if (splitDetected && player.CanSplit())
        {
            Split();
        }
        else if (hitDetected)
        {
            Hit();
        }
        else if (standDetected)
        {
            Stand();
        }
    }

    void Split()
    {
        player.Split();
        Debug.LogError("DEBUG DECISION IS SPLIT");

    }

    void Hit()
    {
        Card newCard = deck.GetTopCard();
        SpawnCard(newCard, initialPlayerCardPosition);
        player.AddCard(newCard, player.ActiveHandIndex);
        Debug.LogError("DEBUG DECISION IS HIT");

    }

    void Stand()
    {
        // Mark the hand as standing
        // Get the current hand tuple
        var currentHand = player.Hands[player.ActiveHandIndex];

        // Create a new tuple with the same cards and updated IsStanding property
        var updatedHand = (currentHand.Cards, IsStanding: true);

        // Replace the old tuple with the new updated tuple
        player.Hands[player.ActiveHandIndex] = updatedHand;
        Debug.LogError("DEBUG DECISION IS STAND");


    }


    public void setSplitDetected()
    {
        splitDetected = true;
    }

    public void setStandDetected()
    {
        standDetected = true;
    }

    public void setHitDetected()
    {
        hitDetected = true;
    }



    void DealInitialCards()
    {
        player.AddCard(deck.GetTopCard());
        dealer.AddCard(deck.GetTopCard());
        player.AddCard(deck.GetTopCard());
        dealer.AddCard(deck.GetTopCard());


    }



    void AnimateCard(Card card, Player player)
    {
        // Assume some animation logic here
    }
}
