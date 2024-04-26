using System.Collections;
using Meta.WitAi.Data;
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
    private bool splitDetected = false;
    private bool hitDetected = false;
    private bool standDetected = false;
    private bool isNewGame = false;
    private Vector3 initialPlayerCardPosition = new Vector3(-2.2f, 10.2f, 226.8f);
    private Vector3 splitsHandSpawnVec = new Vector3(-32.2f, 10.2f, 226.8f);
    private Vector3 initialDealerCardPosition = new Vector3(24.3f, 10.5f, 100.4f);
    private float playerSpawnZOffset = -20f;  // Offset for each new card in the x and y directions
    private float playerSpawnYOffset = 0.3f;  // Offset for each new card in the x and y directions
    private float playerSpawnXOffset = -15f;  // Offset for each new card in the x and y directions
    private float dealerSpawnXOffset = -30f;
    private Vector3 playerCardSpawnOffset;
    private Vector3 dealerCardSpawnOffset;
    public WitIntValue betAmount;
    public SpawnCards spawnCards;

    public AudioSource audioSource;
    public AudioClip handDealing, winSound, loseSound, lobbySound;

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


        StartGameLoop();
    }

    void SetupGame()
    {

        splitDetected = false;
        hitDetected = false;
        standDetected = false;

        Debug.LogError("GAME SETUP DONE");

    }

    void DealerFlipSecondCard(Card secondCard)
    {

        SpawnCard(secondCard, secondCard.transform.position);
    }

    void DealerTurn()
    {

        // FLIP DEALERS SECOND CARD 
        DealerFlipSecondCard(dealer.Hands[dealer.ActiveHandIndex].Cards[1]);

        while (dealer.GetCurrentHandValue() < 17)
        {
            Card newCard = deck.GetTopCard();
            dealer.AddCard(newCard);
            SpawnCard(newCard, initialDealerCardPosition);

        }
    }
    void EvaluateHands()
    {
        int playerHandValue = player.GetCurrentHandValue();
        int dealerHandValue = dealer.GetCurrentHandValue();

        // Check for player bust
        if (playerHandValue > 21)
        {
            Debug.LogError("DEBUG PLAYER BUST: Dealer wins.");
            isDealerWin = true;
        }
        // Check for dealer bust
        else if (dealerHandValue > 21)
        {
            Debug.LogError("DEBUG DEALER BUST: Player wins.");
            isPlayerWin = true;
        }
        // Check for push
        else if (playerHandValue == dealerHandValue)
        {
            Debug.LogError("DEBUG PUSH: Player and Dealer tie.");
            isPush = true;
        }
        // Check if player has a blackjack and dealer does not
        else if (playerHandValue == 21 && dealerHandValue != 21)
        {
            Debug.LogError("DEBUG BLACKJACK: Player wins with a Blackjack.");
            isBlackjack = true;
        }
        // Check if dealer has a blackjack and player does not
        else if (dealerHandValue == 21 && playerHandValue != 21)
        {
            Debug.LogError("DEBUG BLACKJACK: Dealer wins with a Blackjack.");
            isDealerWin = true;
        }
        // Other cases where player's hand value is greater than dealer's
        else if (playerHandValue > dealerHandValue)
        {
            Debug.LogError("DEBUG PLAYER WIN: Player's hand is higher.");
            isPlayerWin = true;
        }
        // Remaining case: Dealer's hand value is higher than player's
        else
        {
            Debug.LogError("DEBUG DEALER WIN: Dealer's hand is higher.");
            isDealerWin = true;
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


        initialPlayerCardPosition = new Vector3(-2.2f, 10.2f, 226.8f);
        initialDealerCardPosition = new Vector3(24.3f, 10.5f, 100.4f);

    }

    public void captureNewGame()
    {
        isNewGame = true;
        Debug.LogError("DEBUG: THUMBS UP CAPTURED");
    }


    // DEBUGGING VERSION 
    void EndGameUpdates()
    {

        if (isPush)
        {
        }

        else if (isPlayerWin || isBlackjack)
        {
            audioSource.clip = winSound;
            audioSource.Play();
        }

        else if (isDealerWin)
        {

            audioSource.clip = loseSound;
            audioSource.Play();
        }

    }

    void StartGameLoop()
    {
        StartCoroutine(playLoop());
    }

    IEnumerator checkForNewGame()
    {
        yield return new WaitUntil(() => isNewGame);

    }
    IEnumerator playLoop()
    {
        while (true)
        {

            yield return StartCoroutine(checkForNewGame());
            isNewGame = false;
            Debug.LogError("DEBUG: NEW GAME STARTING");


            yield return StartCoroutine(PlayFullGame());
        }
    }

    IEnumerator PlayFullGame()
    {
        // Assume initial setup and betting here

        SetupGame();
        DealInitialCards();

        SpawnCardsForGame(player);
        SpawnCardsForGame(dealer, true);



        for (int i = 0; i < player.Hands.Count; i++)
        {

            player.SetActiveHand(i);

            while (player.GetCurrentHandValue() < 21)
            {
                yield return StartCoroutine(WaitForPlayerDecision());
                Debug.LogError("DEBUG PLAYER STARTING HAND " + player.ToString());
                Debug.LogError("DEBUG DEALER STARTING HAND " + dealer.ToString());
                ProcessPlayerDecision();

                splitDetected = false;
                hitDetected = false;
                standDetected = false;

                if (player.GetCurrentHandValue() >= 21 || player.Hands[player.ActiveHandIndex].IsStanding) break;

            }

            Debug.LogError("DEBUG DEALER IS ABOUT TO PLAY" + dealer.ToString());
            DealerTurn();
            Debug.LogError("DEBUG DEALER AFTER PLAYING HAND " + dealer.ToString());

            yield return new WaitForSeconds(3);
            EvaluateHands();

            // REGISTER ROUND UPDATES - DISPLAY WINNER - PAYOUT ETC.
            EndGameUpdates();

        }

        ResetForNextRound();
    }


    public void getVoiceInput(string amount)
    {

        Debug.LogError("AMOUNT " + betAmount.GetIntValue(amount));
    }


    public void SpawnCardsForGame(Player player, bool isDealer = false, bool isSplit = false)
    {
        // Determine the initial spawn position based on whether the player is a dealer or not
        Vector3 spawnPosition = isDealer ? initialDealerCardPosition : initialPlayerCardPosition;


        if (player.ActiveHandIndex > 0 && !isDealer)  // Assuming the first hand (index 0) is never a split
        {
            spawnPosition = splitsHandSpawnVec + new Vector3(playerCardSpawnOffset.x * player.ActiveHandIndex, 0, 0);
        }
        // Adjust spawn position for split hands


        // Determine the offset to apply after each card spawn
        Vector3 spawnOffset = isDealer ? dealerCardSpawnOffset : playerCardSpawnOffset;

        // Iterate through the cards in the active hand
        foreach (Card card in player.Hands[player.ActiveHandIndex].Cards)
        {
            // For dealer, the second card might need to be flipped
            bool shouldFlip = isDealer && player.Hands[player.ActiveHandIndex].Cards.IndexOf(card) == 1;
            SpawnCard(card, spawnPosition, shouldFlip);
            spawnPosition += spawnOffset; // Move the position for the next card
        }

        // Update the initial position to the last used position for the next round's first card
        if (isDealer)
        {
            initialDealerCardPosition = spawnPosition;
        }
        else if (!isSplit) // Only update if not a split to prevent overriding the split position
        {
            initialPlayerCardPosition = spawnPosition;
        }
    }


    IEnumerator PauseBriefly()
    {
        yield return new WaitForSeconds(5);
    }

    public void SpawnCard(Card card, Vector3 position, bool flipped = false)
    {
        if (card != null)
        {
            Debug.Log($"Spawning card: {card.ToString()} at position {position}");

            // Set the rotation based on the flipped flag
            Quaternion rotation = flipped ? Quaternion.Euler(0, 0, 180) : Quaternion.identity;


            // Move the card to the specified position with the given rotation
            StartCoroutine(spawnCards.MoveCard(card, position, rotation, 1.0f)); // Modified to include rotation handling
            audioSource.clip = handDealing;
            audioSource.Play();
            StartCoroutine(PauseBriefly());

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

    void HandleSplitSpawn()
    {
        // Check if there is at least one hand and the hand has at least two cards for a split

        // Get the second card of the first hand
        Card cardToMove = player.Hands[player.ActiveHandIndex].Cards[1];

        // Assuming the second hand exists and is expecting at least one card
        if (player.Hands.Count > 1)
        {
            // Calculate new position for the first card of the second hand
            Vector3 newCardPosition = splitsHandSpawnVec;

            // Assuming Card class has a GameObject field named CardObject representing the card in the scene
            if (cardToMove.CardPrefab != null)
            {
                cardToMove.CardPrefab.transform.position = newCardPosition;
            }
        }

    }

    void Split()
    {
        player.Split();
        HandleSplitSpawn();
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
}
