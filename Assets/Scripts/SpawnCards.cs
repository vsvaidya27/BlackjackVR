using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCards : MonoBehaviour
{
    public GameObject firstCard;
    public GameObject secondCard;

    public float speed1 = 1.0f;
    public float speed2 = 0.8f;

    void Start()
    {
        Vector3 deckPosition = new Vector3(-0.1297f, 183, -0.165f);
        Instantiate(firstCard, deckPosition, Quaternion.identity);
        Instantiate(secondCard, deckPosition, Quaternion.identity);
    }

    // Update is called once per frame

    //while the poistion of the first card is not at Vector3(49, 11, 220) keep moving it to that position
    //once the first card is in the right position, move the second card to Vector3(0, 11, 220) in the same way
    void Update()
    {
        firstCardMove();
        secondCardMove();
    }

    void firstCardMove()
    {
        firstCard.transform.position = Vector3.Lerp(firstCard.transform.position, new Vector3(49, 11, 220), speed1 * Time.deltaTime);
    }

    void secondCardMove()
    {
        secondCard.transform.position = Vector3.Lerp(secondCard.transform.position, new Vector3(40, 11.5f, 220), speed2 * Time.deltaTime);
    }
}
