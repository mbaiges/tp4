using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowsGenerator : MonoBehaviour
{
    // Player
    public GameObject player;

    // Regular Check
    public float checkEvery;
    public float spawningRangeMin;
    public float spawningRangeMax;
    public float despawningDistance;

    // Shadows
    public GameObject[] shadowPrefabs;
    public int maxNearPlayer = 4;
    public float timeBetweenMin = 0;
    public float timeBetweenMax = 5;

    // Trees
    private TreeInstance[] trees;
    private float distance;
    private Terrain terrain;
    private TerrainData terrainData;
    private List<TreeInstance> closestTrees = new List<TreeInstance>();

    // Shadows
    private List<GameObject> all = new List<GameObject>();
    private float nextAt;

    // Regular Check
    private float checkNextAt;

    // Time
    private float startTime;
    private float elapsedTime;

    // Player Position
    Vector3 playerPosition;

    void Start() {
        startTime = Time.time;
        elapsedTime = 0;
        playerPosition = player.transform.position;
        checkNextAt = startTime;
        nextAt = Random.Range(0, timeBetweenMax); // We start with max time
        InitTrees();
        FindClosestTrees();
    }

    private void InitTrees() {
        terrain = Terrain.activeTerrain;
        terrainData = terrain.terrainData;
        trees = terrainData.treeInstances;
    }

    private Vector3 GetTreeBasePosition(TreeInstance tree) {
        return Vector3.Scale(tree.position, terrainData.size) + terrain.transform.position;
    }

    private void FindClosestTrees() {
        if (elapsedTime >= checkNextAt) {
            List<TreeInstance> closest = new List<TreeInstance>();  

            foreach (TreeInstance tree in trees) {
                Vector3 position = GetTreeBasePosition(tree);
                float distance = Vector3.Distance(position, playerPosition);
                if (distance >= spawningRangeMin && distance <= spawningRangeMax) {
                    closest.Add(tree);
                }
            }

            closestTrees = closest;
            checkNextAt = elapsedTime + checkEvery;
        }
    }

    private List<GameObject> NearShadows(List<GameObject> shadows) {
        List<GameObject> nearest = new List<GameObject>();
        foreach (GameObject shadow in shadows) {
            // y fixed to player pos (instead of sphere around player, its a cylinder)
            if (shadow != null && Vector3.Distance(new Vector3(shadow.transform.position.x, playerPosition.y, shadow.transform.position.z), playerPosition) <= spawningRangeMax) {
                nearest.Add(shadow);
            }
        }
        return nearest;
    }

    private float SpawnShadowOnTreeBase(List<GameObject> all, List<GameObject> nearestSpawned, int maxNearPlayer, float nextAt, float timeBetweenMin, float timeBetweenMax) {
        if (closestTrees.Count > 0 && nearestSpawned.Count < maxNearPlayer && elapsedTime >= nextAt) {
            // Select a tree nearby
            TreeInstance tree = closestTrees[(int) Random.Range(0, closestTrees.Count-1)];
            Vector3 position = GetTreeBasePosition(tree);

            // Instantiate sound
            GameObject prefab = shadowPrefabs[(int) Random.Range(0, shadowPrefabs.Length-1)];
            GameObject shadow = GameObject.Instantiate<GameObject>(prefab);
            // the new gameobject to be a child of the gameobject that your script is attached to
            shadow.transform.parent = this.gameObject.transform;
            shadow.transform.position = position;

            // Start shadow movement (something like shadow.Play())
            ShadowController shadowController = shadow.GetComponent<ShadowController>();

            if (shadowController != null) {
                shadowController.Play();
            }

            all.Add(shadow);

            return elapsedTime + Random.Range(timeBetweenMin, timeBetweenMax);
        }
        return -1;
    }

    private void DeleteFarEnoughShadows(List<GameObject> all) {
        for (int i =0; i < all.Count; i++) {
            GameObject obj = all[i];
            if(obj == null || Vector3.Distance(obj.transform.position, playerPosition) > despawningDistance) {
                if (obj != null) {
                    GameObject.Destroy(obj);
                }
                all.RemoveAt(i);
                i--;
            }
        }
    }

    private void HandleShadows() {
        float nextAt = SpawnShadowOnTreeBase(all, NearShadows(all), maxNearPlayer, this.nextAt, timeBetweenMin, timeBetweenMax);
        if (nextAt > 0) { // If we spawned a sound
            // Delete far enough creeks
            DeleteFarEnoughShadows(all);
            // Calculate next cricket appearance time
            this.nextAt = nextAt;
        }
    }

    void Update() {
        elapsedTime = Time.time - startTime;
        playerPosition = player.transform.position;
        FindClosestTrees();
        HandleShadows();
    }
}
