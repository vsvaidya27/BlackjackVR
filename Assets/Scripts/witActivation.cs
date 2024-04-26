using System.Collections;
using System.Collections.Generic;
using Meta.WitAi;
using UnityEngine;

public class WitActivation : MonoBehaviour
{
    [SerializeField] private Wit wit;

    private void OnValidate()
    {
        if (!wit) wit = GetComponent<Wit>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            Debug.LogError("DEBUG: WIT ACTIVED");
            wit.Activate();
        }
    }
} 