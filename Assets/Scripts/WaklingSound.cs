using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaklingSound : MonoBehaviour
{
    public AudioClip grassClip;
    public AudioClip woodClip;
    public float velocityCap = 0;
    private Rigidbody rb;

    void start(){
        
    }

    bool IsGrounded() {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
        return Physics.Raycast(pos, Vector3.down, 0.35f);
    }

    // Update is called once per frame
    void Update()
    {
        rb = GetComponent<Rigidbody>();

        RaycastHit hit;
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
        Physics.Raycast(pos, Vector3.down, out hit);
        if(hit.transform != null){
            if(hit.transform.tag == "Wood"){
                GetComponent<AudioSource>().clip = woodClip;
            }else{
                GetComponent<AudioSource>().clip = grassClip;
        }
        }
        

        if(IsGrounded() && rb.velocity.magnitude > velocityCap && !GetComponent<AudioSource>().isPlaying){
            GetComponent<AudioSource>().volume = Random.Range(0.8f,1f);
            GetComponent<AudioSource>().pitch = Random.Range(0.8f,1.1f);
            GetComponent<AudioSource>().Play();
        }
    }
}