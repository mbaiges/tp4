using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Interactables : MonoBehaviour
{
    public XRRayInteractor leftInteractorRay;
    public GrabManager leftGrabManager;
    public XRRayInteractor rightInteractorRay;
    public GrabManager rightGrabManager;

    [SerializeField] private float searchNearbyEvery = 2;
    [SerializeField] private string grabbableLayer = "Grab";
    [SerializeField] private Material highlightedMaterial;
    [SerializeField] private bool useOnlyHighlightedColor = false;
    [SerializeField] private float radius;
    [SerializeField] private float frequency;

    private Material leftLastSelectedMaterial;
    private Transform _leftSelection;
    private GameObject leftGrabbed;
    private Material rightLastSelectedMaterial;
    private Transform _rightSelection;
    private GameObject rightGrabbed;
    private LayerMask grabLayer;

    private List<GameObject> highlighted = new List<GameObject>();
    private Dictionary<int, Material> originalMaterials = new Dictionary<int, Material>();

    private float startTime;
    private float elapsedTime;
    private float nextSearchAt;


    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        elapsedTime = Time.time - startTime;
        nextSearchAt = 0;
        grabLayer = LayerMask.NameToLayer(grabbableLayer);
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime = Time.time - startTime;
        OnPoint(_leftSelection, leftLastSelectedMaterial, out leftGrabbed);
        OnPoint(_rightSelection, rightLastSelectedMaterial, out rightGrabbed);
        HandleHighlights();
        ResetOnGrabbed(leftGrabManager);
        ResetOnGrabbed(rightGrabManager);
    }

    private void OnPoint(Transform _selection, Material lastSelectedMaterial, out GameObject grabbed) {
        RaycastHit hit;
        GameObject currentGrabbed = null;
        if (rightInteractorRay && rightInteractorRay.TryGetCurrent3DRaycastHit(out hit)) {
            Transform selection = hit.transform;
            if (selection) {
                LayerMask layer = transform.gameObject.layer;
                if (hit.transform.gameObject.layer == grabLayer) {
                    var selectionRenderer = selection.GetComponent<Renderer>();
                    if (selectionRenderer) {

                        Material originalMaterial = new Material(selectionRenderer.material);

                        // Save original material if it is not already set
                        if (!originalMaterials.ContainsKey(hit.transform.gameObject.GetInstanceID())) {
                            originalMaterials.Add(hit.transform.gameObject.GetInstanceID(), originalMaterial);
                        }

                        // Get original material
                        Material orMaterial;
                        originalMaterials.TryGetValue(hit.transform.gameObject.GetInstanceID(), out orMaterial);

                        selectionRenderer.material = HighlightMaterial(orMaterial, 1);
                        _selection = selection;
                        currentGrabbed = hit.transform.gameObject;
                    }
                } else {
                    _selection = null;
                }
            }
        }
        grabbed = currentGrabbed;
    }

    private void HandleHighlights() {
        List<GameObject> nearby = highlighted;
        if (elapsedTime > nextSearchAt) {
            nearby = FindNearbyObjects();
            nextSearchAt = elapsedTime + searchNearbyEvery;
        }
        List<GameObject> newHighlighted = HighlightNearby(nearby);

        ResetNoHighlighted(newHighlighted);
        highlighted = newHighlighted;
    }

    private List<GameObject> FindNearbyObjects() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        List<GameObject> ret = new List<GameObject>();
        // Debug.Log("Searching Nearby Objects");
        foreach (Collider hitCollider in hitColliders) {
            GameObject gameObject = hitCollider.transform.gameObject;
            if (gameObject.layer == grabLayer) {
                XRGrabInteractable interactable = gameObject.GetComponent<XRGrabInteractable>();
                
                // Check if object is being grabbed
                if (gameObject != leftGrabbed && gameObject != rightGrabbed) {
                    ret.Add(gameObject);
                } else {
                    // Debug.Log("Already grabbed: " + gameObject);
                }
                // Debug.Log(gameObject);
            }
        }
        return ret;
    }

    private List<GameObject> HighlightNearby(List<GameObject> nearby) {
        List<GameObject> ret = new List<GameObject>();

        float highlightLevel = GetHighlightLevel();

        foreach(GameObject gameObject in nearby) {
            Renderer renderer = gameObject.GetComponent<Renderer>();

            Material originalMaterial = new Material(renderer.material);

            // Save original material if it is not already set
            if (!originalMaterials.ContainsKey(gameObject.GetInstanceID())) {
                originalMaterials.Add(gameObject.GetInstanceID(), originalMaterial);
            }

            // Get original material
            Material orMaterial;
            originalMaterials.TryGetValue(gameObject.GetInstanceID(), out orMaterial);

            // Debug.Log("Highlighting " + gameObject.GetInstanceID());
            // Debug.Log("Original Material " + orMaterial);

            // Highlight
            renderer.material = HighlightMaterial(orMaterial, highlightLevel);

            ret.Add(gameObject);
        }

        return ret;
    }

    private float GetHighlightLevel() {
        return Mathf.Cos(2*Mathf.PI*frequency * Time.time);
    }

    private void ResetNoHighlighted(List<GameObject> newHighlighted) {
        // Debug.Log("Highlighted: " + highlighted);
        foreach(GameObject lastHighlight in highlighted) {
            if (!CurrentlyHighlighted(newHighlighted, lastHighlight)) {
                GameObject gameObject = lastHighlight.gameObject;
                Renderer renderer = gameObject.GetComponent<Renderer>();

                Material originalMaterial;
                originalMaterials.TryGetValue(gameObject.GetInstanceID(), out originalMaterial);
                renderer.material = originalMaterial;
            }
        }
    }

    private bool CurrentlyHighlighted(List<GameObject> newHighlighted, GameObject highlightData) {
        foreach(GameObject newHighlight in newHighlighted) {
            if (highlightData == newHighlight) {
                return true;
            }
        }
        return false;
    }

    private Material HighlightMaterial(Material material, float highlightLevel) {
        Material ret = new Material(material);
        // Debug.Log("Materials");
        // Debug.Log("Original: " + ret.color);
        // Debug.Log("At Highlight Level: " + highlightLevel);
        if (useOnlyHighlightedColor) {
            ret.SetColor("_Color", Color.Lerp(material.color, highlightedMaterial.color, highlightLevel));
        } else {
            ret.Lerp(material, highlightedMaterial, highlightLevel);
        }
        // Debug.Log("Highlighted: " + ret.color);

        return ret;
    }

    private void ResetOnGrabbed(GrabManager grabManager) {
        XRBaseInteractable a = grabManager.GetGrabbed();
        // Debug.Log("a: " + a);
        if (a != null) {
            // Debug.Log("Resetting " + a.gameObject);
            GameObject gameObject = a.gameObject;
            Material originalMaterial;
            originalMaterials.TryGetValue(gameObject.GetInstanceID(), out originalMaterial);
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null) {
                renderer.material = originalMaterial;
            }
        }
    }

}
