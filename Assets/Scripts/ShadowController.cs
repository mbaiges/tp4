using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowController : MonoBehaviour
{
    private float startTime;
    private float elapsedTime;

    public float targetMaxDistance = 5;
    public float timeMin = 3;
    public float timeMax = 5;

    // Trees

    private TreeInstance[] trees;
    private float distance;
    private Terrain terrain;
    private TerrainData terrainData;
    private List<TreeInstance> closestTrees = new List<TreeInstance>();

    private Vector3 start;
    private Vector3 end;

    private float endTime;

    private void InitTrees() {
        terrain = Terrain.activeTerrain;
        terrainData = terrain.terrainData;
        trees = terrainData.treeInstances;
    }

    private Vector3 GetTreeBasePosition(TreeInstance tree) {
        return Vector3.Scale(tree.position, terrainData.size) + terrain.transform.position;
    }

    private void FindClosestTrees() {
        List<TreeInstance> closest = new List<TreeInstance>();  

        foreach (TreeInstance tree in trees) {
            Vector3 position = GetTreeBasePosition(tree);
            float distance = Vector3.Distance(position, transform.position);
            if (distance <= targetMaxDistance) {
                closest.Add(tree);
            }
        }

        closestTrees = closest;
    }

    // Updaters

    public void Update() {
        elapsedTime = Time.time - startTime;

        // Debug.Log("Updating");

        if (elapsedTime > endTime) {
            DestroyGameObject(); // finished
        }

        float t = 1.0f*elapsedTime/endTime;

        // Debug.Log("Moving shadow");
        // Debug.Log("Start: " + start);
        // Debug.Log("End: " + end);
        // Debug.Log("t: " + t);
        transform.position = Vector3.Lerp(start, end, t);
    }

    private void DestroyGameObject() {
        Destroy(gameObject);
    }

    public void Play() {
        InitTrees();
        FindClosestTrees();
        Debug.Log("Playing shadow");
        
        if (closestTrees.Count > 1) {
            startTime = Time.time;
            start = transform.position;
            TreeInstance target = closestTrees[(int) UnityEngine.Random.Range(0, closestTrees.Count-1)];
            Vector3 pos = GetTreeBasePosition(target);
            while (Vector3.Distance(pos, transform.position) < 0.5f) {
                target = closestTrees[(int) UnityEngine.Random.Range(0, closestTrees.Count-1)];
                pos = GetTreeBasePosition(target);
            }
            end = pos;
            endTime = UnityEngine.Random.Range(timeMin, timeMax);
        } else {
            // No tree nearby
            // Debug.Log("No tree nearby");
            DestroyGameObject();
        }
    }
}
