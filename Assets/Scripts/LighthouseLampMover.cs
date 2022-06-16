using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LighthouseLampMover : MonoBehaviour
{
    // Start is called before the first frame update
    public float rotationSpeed = 10.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime );
        
    }
}
