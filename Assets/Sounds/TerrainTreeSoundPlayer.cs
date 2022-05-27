using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTreeSoundPlayer : MonoBehaviour
{
    // Player
    public GameObject player;

    // Regular Check
    public float checkEvery;
    public float spawningRange;
    public float despawningDistance;

    // Crickets
    public AudioClip[] cricketSounds;
    public float cricketTimeBetweenMin = 0;
    public float cricketTimeBetweenMax = 5;
    public float cricketVolumeMin = 0.8f;
    public float cricketVolumeMax = 1;
    public int cricketsMaxNearPlayer = 4;

    // Trees
    private TreeInstance[] trees;
    private float distance;
    private Terrain terrain;
    private TerrainData terrainData;
    private List<TreeInstance> closestTrees = new List<TreeInstance>();

    // Crickets
    private List<GameObject> cricketAll = new List<GameObject>();
    private float cricketNextAt;

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
        cricketNextAt = cricketTimeBetweenMax; // We start with max time
        InitTrees();
        FindClosestTrees();
    }

    private void InitTrees() {
        terrain = Terrain.activeTerrain;
        terrainData = terrain.terrainData;
        trees = terrainData.treeInstances;
        
        Debug.Log("Total Trees in Map: " + trees.Length);
    }

    private Vector3 GetTreeBasePosition(TreeInstance tree) {
        return Vector3.Scale(tree.position, terrainData.size) + terrain.transform.position;
    }

    private void FindClosestTrees() {
        if (elapsedTime >= checkNextAt) {
            List<TreeInstance> closest = new List<TreeInstance>();  

            foreach (TreeInstance tree in trees) {
                Vector3 position = GetTreeBasePosition(tree);
                if (Vector3.Distance(position, playerPosition) < spawningRange) {
                    closest.Add(tree);
                }
            }

            closestTrees = closest;
            checkNextAt = elapsedTime + checkEvery;
        }
    }

    private List<GameObject> NearSoundDatas(List<GameObject> datas) {
        List<GameObject> nearest = new List<GameObject>();
        foreach (GameObject data in datas) {
            if (Vector3.Distance(data.transform.position, playerPosition) < spawningRange) {
                nearest.Add(data);
            }
        }
        return nearest;
    }

    private float SpawnSoundOnTreeBase(AudioClip[] sources, List<GameObject> all, List<GameObject> nearestSpawned, int maxNearPlayer, float nextAt, float timeBetweenMin, float timeBetweenMax) {
        if (closestTrees.Count > 0 && nearestSpawned.Count < maxNearPlayer && elapsedTime >= nextAt) {
            // Select a tree nearby
            TreeInstance tree = closestTrees[(int) Random.Range(0, closestTrees.Count-1)];
            Vector3 position = GetTreeBasePosition(tree);

            // Instantiate sound
            GameObject soundSource = new GameObject("Temporal Sound Source");
            // the new gameobject to be a child of the gameobject that your script is attached to
            soundSource.transform.parent = this.gameObject.transform;
            soundSource.transform.position = position;
            AudioSource src = soundSource.AddComponent<AudioSource>();
            src.clip = cricketSounds[(int) Random.Range(0, cricketSounds.Length-1)];
            src.volume = Random.Range(cricketVolumeMin, cricketVolumeMax);
            src.pitch = Random.Range(cricketVolumeMin, cricketVolumeMax);
            src.spatialize = true;
            src.spatializePostEffects = true;
            src.spatialBlend = 1;
            src.Play();
            all.Add(soundSource);

            return Random.Range(timeBetweenMin, timeBetweenMax);
        }
        return -1;
    }

    private void DeleteFarEnoughSounds(List<GameObject> all) {
        Debug.Log("Trying to despawn");
        for (int i =0; i < all.Count; i++) {
            GameObject obj = all[i];
            if(Vector3.Distance(obj.transform.position, playerPosition) > despawningDistance) {
                Debug.Log("Despawning at " + obj.transform.position);
                GameObject.Destroy(obj);
                all.RemoveAt(i);
                i--;
            }
        }
    }

    private void HandleCrickets() {
        float nextAt = SpawnSoundOnTreeBase(cricketSounds, cricketAll, NearSoundDatas(cricketAll), cricketsMaxNearPlayer, cricketNextAt, cricketTimeBetweenMin, cricketTimeBetweenMax);
        if (nextAt > 0) { // If we spawned a sound
            // Delete far enough creeks
            DeleteFarEnoughSounds(cricketAll);
            // Calculate next cricket appearance time
            cricketNextAt = nextAt;
        }
    }

    void Update() {
        elapsedTime = Time.time - startTime;
        playerPosition = player.transform.position;
        FindClosestTrees();
        HandleCrickets();
    }
}
