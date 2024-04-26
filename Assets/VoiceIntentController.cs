using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Platform;
using Oculus.Voice;
using UnityEngine;
using UnityEngine.UIElements;

public class VoiceIntentController : MonoBehaviour
{


    public AppVoiceExperience appVoiceExperience;

    // Start is called before the first frame update
    void Start()
    {
        appVoiceExperience.Activate();
        Debug.LogError("DEBUG: ACTIVATED");

    }

    // Update is called once per frame
    void Update()
    {



    }


    public void ReceiveInput(String[] info)
    {
        Debug.LogError("DEBUG: INPUT RECEIVED " + info[0]);
    }
}


