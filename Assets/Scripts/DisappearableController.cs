using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DisappearableController : MonoBehaviour
{

    [SerializeField] private GameObject player;
    [SerializeField] private bool resetToOrigin;
    // In case resetToOrigin is false
    [SerializeField] private float radius;

    private float distanceTolerance = 100f;
    private float distanceOverPlayer = 0.5f;

    private Vector3 origin;
    private Vector3 playerPosition;

    private Rigidbody _body;

    void Start()
    {
        origin = transform.position;
        _body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Debug.Log("Checking distance");
        playerPosition = player.transform.position;
        Debug.Log("Player Position: " + playerPosition);
        Debug.Log(message: "Thing Position: " + transform.position);
        float distance = Mathf.Abs(playerPosition.y - transform.position.y);
        Debug.Log(distance);
        if (distance > distanceTolerance) {
            // Reset pos
            Vector3 newPos;
            if (resetToOrigin) {
                newPos = origin;
            } else {
                float angle = Random.Range(0, 2*Mathf.PI);
                newPos = new Vector3(playerPosition.x + radius*Mathf.Cos(angle), playerPosition.y + distanceOverPlayer, playerPosition.z + radius*Mathf.Sin(angle));
            }
            transform.position = newPos;
            _body.velocity = Vector3.zero;
        }
    }
}
