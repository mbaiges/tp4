using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.XR.CoreUtils;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool canJump = true;
    [SerializeField] private InputActionReference jumpActionReference;
    [SerializeField] private float jumpForce = 100.0f;

    // Drowning
    [SerializeField] private float waterHeightTolerance = 0.7f;
    [SerializeField] private int deathDelaySeconds = 1;
    [SerializeField] private AudioSource[] drowningSounds;
    [SerializeField] private AudioSource[] drownSounds;
    [SerializeField] private AudioSource[] outOfWaterSounds;

    private XROrigin _xrOrigin;
    private CapsuleCollider _collider;
    private Rigidbody _body;

    private Vector3 spawn;
    private GameObject water;
    private float drownSoundAt = -1f;
    private float dieAt = -1f;
    private AudioSource soundingSource = null;

    private float startTime;
    private float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        _xrOrigin = GetComponent<XROrigin>();
        _collider = GetComponent<CapsuleCollider>();
        _body = GetComponent<Rigidbody>();
        spawn = transform.position;
        water = GameObject.Find("/World/Water");
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime = Time.time - startTime;

        JumpHandler();
        CheckDeath();
    }

    // Updaters

    // Action handlers

    private void JumpHandler() {
        if (canJump) {
            float gripValue = jumpActionReference.action.ReadValue<float>();
            if (gripValue > 0) {
                OnJump();
            }
        }
    }
    private void OnJump() {
        RaycastHit hit;
        Vector3 headPosition = new Vector3(transform.position.x, transform.position.y +_collider.height, transform.position.z);
        Vector3 direction = Vector3.down;
        float maxDistance = 2.1f;
        if (Physics.Raycast(headPosition, direction, out hit, maxDistance)) {
            Vector3 jump = Vector3.up * jumpForce;
            _body.AddForce(jump);
        };
    }

    private AudioSource RandAudio(AudioSource[] arr) {
        int idx = UnityEngine.Random.Range(0, arr.Length);
        return arr[idx];
    }
    private void CheckDeath() {
        if (water != null) {
            AudioSource audio = null;

            if (_body.transform.position.y < water.transform.position.y - waterHeightTolerance) {
                if (dieAt < 0) {
                    Debug.Log("Drowning");
                    audio = RandAudio(drowningSounds);
                    drownSoundAt = elapsedTime + (int) (0.5f * deathDelaySeconds);
                    dieAt = elapsedTime + deathDelaySeconds;
                } else if (elapsedTime > dieAt) {
                    Debug.Log("Drown");
                    ResetScene();
                } else if (drownSoundAt > 0 && elapsedTime > drownSoundAt) {
                    audio = RandAudio(drownSounds);
                    drownSoundAt = -1f;
                }
            }
            else if (dieAt > 0) {
                Debug.Log("Saved");
                audio = RandAudio(outOfWaterSounds);
                dieAt = -1f;
            }

            if (audio != null) {
                if (soundingSource != null) {
                    Debug.Log("Stopping sound");
                    soundingSource.Stop();
                    soundingSource = null;
                }
                audio.Play();
                soundingSource = audio;
            }
        }
    }

    private void ResetScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
