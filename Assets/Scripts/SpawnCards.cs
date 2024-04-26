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

    // public IEnumerator MoveCard(Card card, Vector3 targetPosition, Quaternion targetRotation, float speed)
    // {

    //     card.transform.position = targetPosition;
    //     card.transform.rotation = targetRotation;
    //     yield return null;
    // }
    public void MoveCard(Card card, Vector3 targetPosition, Quaternion targetRotation, float duration, bool dealerSecond = false)
    {
        // Store the starting position and rotation
        Vector3 startPosition = card.transform.position;
        Quaternion startRotation = card.transform.rotation;

        float elapsedTime = 0;

        // Continue the operation until the card reaches the target position and rotation
        while (elapsedTime < duration)
        {
            // Calculate the interpolation fraction
            float fraction = elapsedTime / duration;

            // Update the card's position
            card.transform.position = Vector3.Lerp(startPosition, targetPosition, fraction);

            // Conditionally update the rotation based on the dealerSecond flag
            if (dealerSecond)
            {
                card.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, fraction);
            }
            else
            {
                // Keep the rotation constant to prevent the card's face from being revealed
                card.transform.rotation = startRotation;
            }

            // Increment the elapsed time by the delta time since last frame
            elapsedTime += Time.deltaTime;

            // Yield until the next frame
        }

        // Set the final position
        card.transform.position = targetPosition;

        // Conditionally set the final rotation
        // if (!dealerSecond)
        // {
        // card.transform.rotation = targetRotation;
        // }
        // else
        // {
        //     // Ensure the card's rotation remains as it started if dealerSecond is true
        //     card.transform.rotation = startRotation;
        // }
    }


}
