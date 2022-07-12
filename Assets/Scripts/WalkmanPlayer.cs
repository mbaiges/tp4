using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class WalkmanPlayer : MonoBehaviour
{
    public AudioSource sound = null;
    public AudioSource beeps = null;
    private XRGrabInteractable grabInteractable = null;
    public void onActivate() {
        sound.Play();
        beeps.Stop();
    }

}
