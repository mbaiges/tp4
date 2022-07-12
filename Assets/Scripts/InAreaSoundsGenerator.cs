using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAreaSoundsGenerator : MonoBehaviour
{
    // Start is called before the first frame update
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
    public float heightMin = 0;
    public float heightMax = 0;


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

    private Vector3 GetRandomPositionNearby(){
        Vector3 pos = new Vector3();
        int signX = Random.Range(0,1) > 0.5f ? 1 : -1;
        int signZ = Random.Range(0,1) > 0.5f ? 1 : -1;
        pos.x = playerPosition.x + (Random.Range(spawningRangeMin, spawningRangeMax) * signX);
        pos.z = playerPosition.z + (Random.Range(spawningRangeMin, spawningRangeMax) * signZ);
        pos.y = Terrain.activeTerrain.SampleHeight(pos) +  Random.Range(heightMin, heightMax);
        return pos;
    }

    private float SpawnSound(List<GameObject> all, List<GameObject> nearestSpawned, int maxNearPlayer, float nextAt, float timeBetweenMin, float timeBetweenMax) {
        if (all.Count < maxInMap && nearestSpawned.Count < maxNearPlayer && elapsedTime >= nextAt) {
            // Select a position nearby

            Vector3 position = GetRandomPositionNearby();


            // Instantiate sound
            GameObject prefab = sounds[(int) Random.Range(0, sounds.Length-1)];
            GameObject soundSource = GameObject.Instantiate<GameObject>(prefab);
            // the new gameobject to be a child of the gameobject that your script is attached to
            soundSource.transform.parent = this.gameObject.transform;
            soundSource.transform.position = position;
            AudioSource src = soundSource.GetComponent<AudioSource>();
            src.volume = Random.Range(volumeMin, volumeMax);
            src.pitch = Random.Range(pitchMin, pitchMax);
            src.spatialize = true;
            src.spatializePostEffects = true;
            src.spatialBlend = 1;
            // src.rolloffMode = AudioRolloffMode.Linear;
            src.Play();
            all.Add(soundSource);

            return Random.Range(timeBetweenMin, timeBetweenMax);
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
        float nextAt = SpawnSound(all, NearSoundDatas(all), maxNearPlayer, this.nextAt, timeBetweenMin, timeBetweenMax);
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
        HandleSounds();
    }
}
