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

    private XROrigin _xrOrigin;
    private CapsuleCollider _collider;
    private Rigidbody _body;

    private Vector3 spawn; 

    private float startTime;
    private float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        _xrOrigin = GetComponent<XROrigin>();
        _collider = GetComponent<CapsuleCollider>();
        _body = GetComponent<Rigidbody>();
        spawn = transform.position;
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime = Time.time - startTime;

        JumpHandler();
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

    private void ResetScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
