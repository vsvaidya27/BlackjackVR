using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBetController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Alert(string[] info)
    {
        Debug.LogError("BET AMOUNT IS: " + info);
    }
}
