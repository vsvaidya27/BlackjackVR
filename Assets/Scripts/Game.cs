using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{


    Player player;
    Player dealer;
    Deck deck;

    private bool isDone = false;
    // Start is called before the first frame update
    void Start()
    {


        GameObject deckObject = new GameObject("Deck");
        deck = deckObject.AddComponent<Deck>();

        GameObject playerObject = new GameObject("Player");
        GameObject dealerObject = new GameObject("Dealer");

        player = playerObject.AddComponent<Player>();
        dealer = dealerObject.AddComponent<Player>();
        GameLoop();
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    private void GameLoop()
    {

        Debug.LogError("HIT");
        handStartDeal();

    }

    private void handStartDeal()
    {
        for (int i = 0; i < 2; i++)
        {
            player.AddCard(deck.GetTopCard());
            dealer.AddCard(deck.GetTopCard());
        }
        Debug.LogError("Player hand " + player.ToString());
        Debug.LogError("Dealer hand " + dealer.ToString());

    }
}
