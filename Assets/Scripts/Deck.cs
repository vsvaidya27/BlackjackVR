using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public GameObject cardPrefab; // Assign a prefab that has the Card component
    private List<Card> deck = new List<Card>();

    void Awake()
    {
        CreateMultipleDecks(8); // Create 8 decks of cards
        ShuffleDeck();
    }

    // Create multiple decks
    void CreateMultipleDecks(int numberOfDecks)
    {
        string[] suits = { "Club", "Diamond", "Heart", "Spade" };
        string[] ranks = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13" };

        for (int i = 0; i < numberOfDecks; i++)
        {
            foreach (string suit in suits)
            {
                for (int j = 0; j < ranks.Length; j++)
                {
                    string cardKey = $"Black_PlayingCards_{suit}{ranks[j]}_00";
                    GameObject cardPrefab = Resources.Load<GameObject>($"Playing Cards/Resource/Prefab/BackColor_Black/{cardKey}");
                    if (cardPrefab != null)
                    {
                        GameObject cardGO = Instantiate(cardPrefab);
                        cardGO.SetActive(true);

                        Card cardComponent = cardGO.AddComponent<Card>();

                        if (cardComponent != null)
                        {

                            // Assuming InitializeCard takes GameObjects for front/back, a rank integer, and a suit string
                            cardComponent.InitializeCard(cardGO, int.Parse(ranks[j]), suit); // parse rank from string to int
                            deck.Add(cardComponent);
                        }

                        cardGO.SetActive(false); // Instantiate but keep inactive
                    }
                }
            }
        }
        Debug.LogError("DECK LEN: " + deck.Count);

    }



    // Shuffle the deck
    void ShuffleDeck()
    {

        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    // Method to get the top card of the deck
    public Card GetTopCard()
    {
        if (deck.Count > 0)
        {
            Card topCard = deck[0];
            deck.RemoveAt(0);
            return topCard;
        }
        else
        {
            Debug.LogError("No cards left in the deck!");
            return null;
        }
    }
}
