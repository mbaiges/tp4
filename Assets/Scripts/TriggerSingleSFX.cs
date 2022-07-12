using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSingleSFX : MonoBehaviour
{
    
    public AudioSource playSound;
    private bool played;

    void Start()
    {
        played = false;
    }


    void OnTriggerEnter(Collider other) {
        if(!played){
            playSound.Play();
            played = true;
        }
        
        
    }
}
