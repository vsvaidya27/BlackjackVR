using System.Collections;
using Meta.WitAi.Data;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

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
    private Vector3 newCardPosition = new Vector3(0f, 0f, 0f);
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
    public TMP_Text startMessage;
    public TMP_Text winMessage;
    public TMP_Text loosetMessage;

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

        startMessage.gameObject.SetActive(true);
        winMessage.gameObject.SetActive(false);
        loosetMessage.gameObject.SetActive(false);


        StartGameLoop();
    }

    void SetupGame()
    {

        splitDetected = false;
        hitDetected = false;
        standDetected = false;


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
        audioSource.clip = handDealing;
        audioSource.Play();
        spawnCards.MoveCard(card, position, targetRotation, 2.0f, dealerSecond);

        yield return new WaitForSeconds(1); // Waiting time for the card to move into position
    }

    private Vector3 CalculateNextCardPosition(Vector3 currentPosition, Vector3 offset)
    {
        return currentPosition + offset;
    }





    IEnumerator DealerTurn()
    {
        // Ensure the dealer's second card is flipped and wait for it to complete
        yield return StartCoroutine(DealerFlipSecondCard(dealer.Hands[dealer.ActiveHandIndex].Cards[1]));

        // Continue hitting while the dealer's hand value is below 17
        while (dealer.GetCurrentHandValue() < 17)
        {
            Debug.Log("DEBUG: DEALER BEFORE HIT " + dealer.ToString());
            yield return StartCoroutine(DealerHit());  // Wait for each hit to complete
            Debug.Log("DEBUG: DEALER AFTER HIT " + dealer.ToString());

        }

        // Optionally wait for a second to observe the final hand
        yield return new WaitForSeconds(1);

        // Now that all dealer actions are complete, evaluate hands
    }



    public IEnumerator DealerHit()
    {
        Card newCard = deck.GetTopCard();
        Vector3 spawnPosition = initialDealerCardPosition;


        yield return StartCoroutine(SpawnCard(newCard, spawnPosition));
        dealer.AddCard(newCard, dealer.ActiveHandIndex);
        initialDealerCardPosition = CalculateNextCardPosition(initialDealerCardPosition, dealerCardSpawnOffset);
        Debug.LogError("DEALER DECISION IS HIT");
        yield return new WaitForSeconds(1);
    }


    IEnumerator DealerFlipSecondCard(Card card)
    {
        // Assuming each card has a GameObject with a Card component that can be accessed to flip it
        yield return StartCoroutine(FlipCard(card));
        yield return new WaitForSeconds(1);

    }

    IEnumerator FlipCard(Card card)
    {
        // Assuming flipping involves rotating the card to show its face
        Quaternion startRotation = card.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, 0, 0); // Adjust as needed for correct orientation
        float duration = 0.5f; // Duration for the flip animation
        float elapsed = 0;
        audioSource.clip = handDealing;
        audioSource.Play();
        while (elapsed < duration)
        {
            card.transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        card.transform.rotation = endRotation; // Ensure the rotation is exact
    }
    IEnumerator EvaluateHands()
    {
        int dealerHandValue = dealer.GetCurrentHandValue();

        // Iterate over each hand of the player
        for (int i = 0; i < player.Hands.Count; i++)
        {
            player.SetActiveHand(i);
            int playerHandValue = player.GetCurrentHandValue(); // Assuming GetCurrentHandValue can accept a hand

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
            EndGameUpdates();
            isDealerWin = false;
            isBlackjack = false;
            isPush = false;
            isPlayerWin = false;
            yield return StartCoroutine(PauseBriefly());
        }

        // You might need to adjust how updates are handled if multiple hands are won/lost
    }

    public void Hit()
    {
        Card newCard = deck.GetTopCard();

        if (newCardPosition != Vector3.zero && player.ActiveHandIndex != 0 && player.Hands[player.ActiveHandIndex].Cards.Count == 1)
        {
            initialPlayerCardPosition = CalculateNextCardPosition(newCardPosition, playerCardSpawnOffset);
        }

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

        startMessage.gameObject.SetActive(true);
        winMessage.gameObject.SetActive(false);
        loosetMessage.gameObject.SetActive(false);

        dealer.ResetForNewRound();
        player.ResetForNewRound();


        initialPlayerCardPosition = new Vector3(-2.2f, 10.2f, 226.8f);
        initialDealerCardPosition = new Vector3(24.3f, 10.5f, 100.4f);
        newCardPosition = Vector3.zero;

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
            winMessage.gameObject.SetActive(true);
        }

        else if (isDealerWin)
        {

            audioSource.clip = loseSound;
            audioSource.Play();
            loosetMessage.gameObject.SetActive(true);
        }

    }

    void StartGameLoop()
    {
        startMessage.gameObject.SetActive(true);
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
            startMessage.gameObject.SetActive(false);

            yield return StartCoroutine(PlayFullGame());
            ResetForNextRound();
        }
    }

    IEnumerator PlayFullGame()
    {
        // Assume initial setup and betting here

        SetupGame();
        DealInitialCards();

        StartCoroutine(SpawnInitialCards());



        int i = 0; // Initialization outside the loop
        while (i < player.Hands.Count) // Condition check
        {

            player.SetActiveHand(i);

            if (i > 0)
            {
                initialPlayerCardPosition += initialPlayerCardPosition - (2 * dealerCardSpawnOffset) - playerCardSpawnOffset;
            }


            while (player.GetCurrentHandValue() < 21)
            {
                Debug.LogError("DEBUG ACTIVE HAND INDEX " + player.ActiveHandIndex);

                Debug.LogError("DEBUG PLAYER ACTIVE HAND BEFORE" + player.ToString());
                yield return StartCoroutine(WaitForPlayerDecision());
                ProcessPlayerDecision();
                Debug.LogError("DEBUG PLAYER ACTIVE HAND AFTER" + player.ToString());


                splitDetected = false;
                hitDetected = false;
                standDetected = false;

                if (player.GetCurrentHandValue() >= 21 || player.Hands[player.ActiveHandIndex].IsStanding) break;

            }
            i += 1;

        }
        Debug.LogError("DEBUG DEALER BEFORE PLAY" + dealer.ToString());
        yield return StartCoroutine(DealerTurn());
        Debug.LogError("DEBUG DEALER AFTER  HAND " + dealer.ToString());

        yield return StartCoroutine(PauseBriefly());
        yield return StartCoroutine(EvaluateHands());

    }

    IEnumerator PauseBriefly()
    {
        yield return new WaitForSeconds(2);
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

            newCardPosition = initialPlayerCardPosition - (2 * dealerCardSpawnOffset) - (2 * playerCardSpawnOffset);  // Adjust if needed
            Quaternion targetRotation = Quaternion.identity;  // Assuming no specific rotation is needed

            spawnCards.MoveCard(cardToMove, newCardPosition, targetRotation, 1.0f, false);
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
