using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCards : MonoBehaviour
{
    public GameObject cardPrefab;  // Prefab for spawning cards
    private GameObject firstCard;
    private GameObject secondCard;

    public float speed1 = 1.0f;
    public float speed2 = 0.8f;

    // Method to spawn cards
    public void SpawnInitialCards(Vector3 deckPosition)
    {
        firstCard = Instantiate(cardPrefab, deckPosition, Quaternion.identity);
        secondCard = Instantiate(cardPrefab, deckPosition, Quaternion.identity);
    }

    // void Update()
    // {
    //     if (firstCard != null)
    //         MoveCard(firstCard, new Vector3(49, 11, 220), speed1);

    //     if (secondCard != null && firstCard.transform.position == new Vector3(49, 11, 220))
    //         MoveCard(secondCard, new Vector3(40, 11.5f, 220), speed2);
    // }

    public void MoveCard(Card card, Vector3 targetPosition, Quaternion targetRotation, float speed)
    {

        card.transform.position = targetPosition;
        card.transform.rotation = targetRotation;

    }
}
