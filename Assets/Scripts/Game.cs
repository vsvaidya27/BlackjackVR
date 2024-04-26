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

    public IEnumerator SpawnInitialCards()
    {
        // Assuming the first two cards in each hand are meant for the initial deal
        // Spawn cards alternating between player and dealer

        // Spawn the first round of cards (player-dealer-player-dealer)
        for (int i = 0; i < 2; i++) // For two rounds of card distribution
        {
            // Player card
            if (i < player.Hands[player.ActiveHandIndex].Cards.Count)
            {
                Card playerCard = player.Hands[player.ActiveHandIndex].Cards[i];

                yield return StartCoroutine(SpawnCard(playerCard, initialPlayerCardPosition, true));
                initialPlayerCardPosition = CalculateNextCardPosition(initialPlayerCardPosition, playerCardSpawnOffset);
            }

            // Dealer card, the second dealer card is face down
            if (i < dealer.Hands[dealer.ActiveHandIndex].Cards.Count)
            {

                Card dealerCard = dealer.Hands[dealer.ActiveHandIndex].Cards[i];

                bool isFaceUp = i != 1; // Face up unless it's the second card
                yield return StartCoroutine(SpawnCard(dealerCard, initialDealerCardPosition, isFaceUp, i == 1));
                initialDealerCardPosition = CalculateNextCardPosition(initialDealerCardPosition, dealerCardSpawnOffset);
            }
        }
    }

    private IEnumerator SpawnCard(Card card, Vector3 position, bool faceUp = true, bool dealerSecond = false)
    {

        Quaternion targetRotation = faceUp ? Quaternion.identity : Quaternion.Euler(0, 0, 180);
        Debug.LogError("DEBUG SPAWN1 " + card.ToString() + targetRotation + " " + faceUp + " " + dealerSecond);

        spawnCards.MoveCard(card, position, targetRotation, 2.0f, dealerSecond);
        yield return new WaitForSeconds(1); // Waiting time for the card to move into position
    }

    private Vector3 CalculateNextCardPosition(Vector3 currentPosition, Vector3 offset)
    {
        return currentPosition + offset;
    }



    public void DealerHit(bool faceUp = true, bool dealerSecond = false)
    {
        Card newCard = deck.GetTopCard();
        Vector3 spawnPosition = dealerSecond ? initialDealerCardPosition + dealerCardSpawnOffset : initialDealerCardPosition;
        StartCoroutine(SpawnCard(newCard, spawnPosition, faceUp, dealerSecond));
        dealer.AddCard(newCard, dealer.ActiveHandIndex);
        initialDealerCardPosition += dealerCardSpawnOffset;
        Debug.LogError("DEALER DECISION IS HIT");
    }

    IEnumerator DealerTurn()
    {

        // FLIP DEALERS SECOND CARD 
        StartCoroutine(DealerFlipSecondCard(dealer.Hands[dealer.ActiveHandIndex].Cards[1]));


        while (dealer.GetCurrentHandValue() < 17)
        {
            StartCoroutine(DealerHit());
        }
        yield return new WaitForSeconds(1);
    }

    IEnumerator DealerHit()
    {
        Card newCard = deck.GetTopCard();
        dealer.AddCard(newCard, dealer.ActiveHandIndex);
        StartCoroutine(SpawnCard(newCard, CalculateNextCardPosition(initialDealerCardPosition, dealerCardSpawnOffset), true));

        initialDealerCardPosition = CalculateNextCardPosition(initialDealerCardPosition, dealerCardSpawnOffset);
        yield return new WaitForSeconds(1);
    }


    IEnumerator DealerFlipSecondCard(Card card)
    {
        // Assuming each card has a GameObject with a Card component that can be accessed to flip it
        yield return StartCoroutine(FlipCard(card));

    }

    IEnumerator FlipCard(Card card)
    {
        // Assuming flipping involves rotating the card to show its face
        Quaternion startRotation = card.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, 0, 0); // Adjust as needed for correct orientation
        float duration = 0.5f; // Duration for the flip animation
        float elapsed = 0;

        while (elapsed < duration)
        {
            card.transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        card.transform.rotation = endRotation; // Ensure the rotation is exact
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
    public void Hit()
    {
        Card newCard = deck.GetTopCard();
        StartCoroutine(SpawnCard(newCard, initialPlayerCardPosition, true)); // Assuming face-up by default
        player.AddCard(newCard, player.ActiveHandIndex);
        initialPlayerCardPosition += playerCardSpawnOffset;
        Debug.LogError("DEBUG DECISION IS HIT");
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
            ResetForNextRound();
        }
    }

    IEnumerator PlayFullGame()
    {
        // Assume initial setup and betting here

        SetupGame();
        DealInitialCards();

        Debug.LogError("DEBUG PLAYER STARTING HAND " + player.ToString());
        Debug.LogError("DEBUG DEALER STARTING HAND " + dealer.ToString());
        StartCoroutine(SpawnInitialCards());



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


    }


    public void getVoiceInput(string amount)
    {

        Debug.LogError("AMOUNT " + betAmount.GetIntValue(amount));
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

    void Split()
    {
        player.Split();
        HandleSplitSpawn();
        Debug.LogError("DEBUG DECISION IS SPLIT");

    }

    void HandleSplitSpawn()
    {
        if (player.Hands.Count > 1 && player.Hands[1].Cards.Count > 0)
        {
            Card cardToMove = player.Hands[player.ActiveHandIndex + 1].Cards[0];  // Assuming this is the card moved to the new hand

            Vector3 newCardPosition = initialPlayerCardPosition + dealerCardSpawnOffset;  // Adjust if needed
            Quaternion targetRotation = Quaternion.identity;  // Assuming no specific rotation is needed

            // StartCoroutine(spawnCards.MoveCard(cardToMove, newCardPosition, targetRotation, 1.0f, false));
        }
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
