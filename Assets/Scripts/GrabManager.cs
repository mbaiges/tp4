using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabManager : MonoBehaviour
{
    private XRRayInteractor interactor;
    private XRBaseInteractable grabbed = null;
    
    void Start() {
        this.interactor = GetComponent<XRRayInteractor>();
    }

    public void OnSelectEnter() {
        this.grabbed = this.interactor.selectTarget;
        Debug.Log("Grabbed: " + this.grabbed);
    }

    public void OnSelectExit() {
        this.grabbed = null;
        // Debug.Log("Ungrabbed");
    }

    public XRBaseInteractable GetGrabbed() {
        return this.grabbed;
    }
}
