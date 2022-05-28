using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTreeSoundPlayer : MonoBehaviour
{
    // Player
    public GameObject player;

    // Regular Check
    public float checkEvery;
    public float spawningRangeMin;
    public float spawningRangeMax;
    public float despawningDistance;

    // Sounds
    public GameObject[] sounds;
    public int maxNearPlayer = 4;
    public int maxInMap = 100;
    public float timeBetweenMin = 0;
    public float timeBetweenMax = 5;
    public float pitchMin = 0.8f;
    public float pitchMax = 1;
    public float volumeMin = 0.8f;
    public float volumeMax = 1;
    public float treeHeightMin = 0;
    public float treeHeightMax = 0;

    // Trees
    private TreeInstance[] trees;
    private float distance;
    private Terrain terrain;
    private TerrainData terrainData;
    private List<TreeInstance> closestTrees = new List<TreeInstance>();

    // Crickets
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
                float distance = Vector3.Distance(position, playerPosition);
                if (distance >= spawningRangeMin && distance <= spawningRangeMax) {
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
            // y fixed to player pos (instead of sphere around player, its a cylinder)
            float distance = Vector3.Distance(new Vector3(data.transform.position.x, playerPosition.y, data.transform.position.z), playerPosition);
            if (distance <= spawningRangeMax) {
                nearest.Add(data);
            }
        }
        return nearest;
    }

    private float SpawnSoundOnTreeBase(List<GameObject> all, List<GameObject> nearestSpawned, int maxNearPlayer, float nextAt, float timeBetweenMin, float timeBetweenMax) {
        if (closestTrees.Count > 0 && all.Count < maxInMap && nearestSpawned.Count < maxNearPlayer && elapsedTime >= nextAt) {
            // Select a tree nearby
            TreeInstance tree = closestTrees[(int) Random.Range(0, closestTrees.Count-1)];
            Vector3 position = GetTreeBasePosition(tree);
            position.y += Random.Range(treeHeightMin, treeHeightMax);

            // Instantiate sound
            GameObject prefab = sounds[(int) Random.Range(0, sounds.Length-1)];
            GameObject soundSource = GameObject.Instantiate<GameObject>(prefab);
            // the new gameobject to be a child of the gameobject that your script is attached to
            soundSource.transform.parent = this.gameObject.transform;
            soundSource.transform.position = position;
            AudioSource src = soundSource.GetComponent<AudioSource>();

            src.volume = Random.Range(volumeMin, volumeMax);
            Debug.Log(src.volume);
            src.pitch = Random.Range(pitchMin, pitchMax);
            src.spatialize = true;
            src.spatializePostEffects = true;
            src.spatialBlend = 1;
            // src.rolloffMode = AudioRolloffMode.Linear;
            src.Play();
            all.Add(soundSource);

            return elapsedTime + Random.Range(timeBetweenMin, timeBetweenMax);
        }
        return -1;
    }

    private void DeleteFarEnoughSounds(List<GameObject> all) {
        for (int i =0; i < all.Count; i++) {
            GameObject obj = all[i];
            if(Vector3.Distance(obj.transform.position, playerPosition) > despawningDistance) {
                GameObject.Destroy(obj);
                all.RemoveAt(i);
                i--;
            }
        }
    }

    private void HandleSounds() {
        float nextAt = SpawnSoundOnTreeBase(all, NearSoundDatas(all), maxNearPlayer, this.nextAt, timeBetweenMin, timeBetweenMax);
        if (nextAt > 0) { // If we spawned a sound
            // Delete far enough creeks
            DeleteFarEnoughSounds(all);
            // Calculate next cricket appearance time
            this.nextAt = nextAt;
        }
    }

    void Update() {
        elapsedTime = Time.time - startTime;
        playerPosition = player.transform.position;
        FindClosestTrees();
        HandleSounds();
    }
}
