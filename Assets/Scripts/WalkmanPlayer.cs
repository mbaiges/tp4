using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class WalkmanPlayer : MonoBehaviour
{
    public AudioSource sound = null;
    private XRGrabInteractable grabInteractable = null;
    public void onActivate() {
        sound.Play();
    }

}
