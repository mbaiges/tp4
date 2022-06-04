using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UShallNotPass : MonoBehaviour
{
    [SerializeField] private LayerMask terrainLayer;
    [SerializeField] private float margin = 1;
    [SerializeField] private float spawnHeight = 1;

    void Update()
    {
        RaycastHit hit;
        Vector3 position = transform.position;
        if (Physics.Raycast(position, Vector3.up, out hit, 100, terrainLayer)) {
            Debug.Log("Chocó contra " + hit.transform);
            Debug.Log("Levantando hacia " + hit.point.y);
            transform.position = new Vector3(position.x, hit.point.y + spawnHeight, position.z);
        }
    }
}
