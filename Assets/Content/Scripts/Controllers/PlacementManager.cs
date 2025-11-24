using UnityEngine;
using UnityEngine.UIElements;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;


    [Header("Prefabs")]
    public GameObject fireCrewPrefab;
    public GameObject fireLinePrefab;
    public GameObject safeZonePrefab;

    [Header("Costs")]
    public int fireCrewCost = 2500;
    public int fireLineCost = 500;
    public int safeZoneCost = 500;

    [Header("Forbidden Zone Detection")]
    public LayerMask forbiddenLayer; // ex: layer "Water"

    [Header("Preview")]
    public SpriteRenderer previewRenderer;
    public Color previewColor = new Color(1, 1, 1, 0.5f);


    public AudioSource audioSource;
    public AudioClip cashRegister;

    public bool isPlacing = false;
    private GameObject prefabToPlace;
    private int prefabCost;
    private Camera cam;

    private float currentRotation = 0f;
    private PlaceableObjectDefinition currentDef;

    private SelectableObject selectedObject = null;
    private bool isMovingExisting = false;

    private void Awake()
    {
        Instance = this;
        cam = Camera.main;

        // Desactivate preview at start
        if (previewRenderer != null)
            previewRenderer.gameObject.SetActive(false);
    }

    void Update()
    {
        // If nothing to place, exit
        if (!isPlacing && !isMovingExisting)
            return;

        // Update preview position
        UpdatePreviewPosition();
        HandleRotation();

        // Move existing object
        if (isMovingExisting)
        {
            // Validate move
            if (Input.GetMouseButtonDown(0))
            {
                PlaceExistingObject();
                return;
            }
            // Cancel move
            if (Input.GetMouseButtonDown(1))
            {
                CancelMove();
                return;
            }
            // Do not continue to placement checks
            return;
        }
        // Placing new object
        if (isPlacing && currentDef != null)
        {
            // Validate placement
            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceObject();
                return;
            }
            // Cancel placement
            if (Input.GetMouseButtonDown(1))
            {
                StopPlacing();
                return;
            }
        }
    }

    // --------------------------------------------------------------
    // START PLACING
    // --------------------------------------------------------------
    public void StartPlacing(PlaceableObjectDefinition def)
    {
        if (GameManager.Instance.money < def.cost)
        {
            Debug.Log("Pas assez d'argent !");
            return;
        }

        currentDef = def;
        isPlacing = true;
        currentRotation = 0;

        // Setup preview
        SpriteRenderer sr = def.prefab.GetComponentInChildren<SpriteRenderer>();
        previewRenderer.transform.localScale = sr.transform.localScale;
        previewRenderer.sprite = sr.sprite;
        previewRenderer.color = new Color(1, 1, 1, 0.5f);
        previewRenderer.transform.rotation = Quaternion.identity;
        previewRenderer.gameObject.SetActive(true);
    }


    // --------------------------------------------------------------
    // UPDATE PREVIEW POSITION
    // --------------------------------------------------------------
    void UpdatePreviewPosition()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        previewRenderer.transform.position = mousePos;
    }

    // --------------------------------------------------------------
    // ROTATION
    // --------------------------------------------------------------
    void HandleRotation()
    {
        // Rotation with R (90°)
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentRotation += 90f;
        }

        // Rotation with wheel
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
        {
            currentRotation += scroll * 10f;
        }

        previewRenderer.transform.rotation = Quaternion.Euler(0, 0, currentRotation);
    }

    // --------------------------------------------------------------
    // TRY PLACE OBJECT
    // --------------------------------------------------------------
    void TryPlaceObject()
    {
        if (GameManager.Instance.money < currentDef.cost)
        {
            Debug.Log("Not enough money !");
            StopPlacing();
            return;
        }

        Vector3 pos = previewRenderer.transform.position;

        if (IsOverForbiddenZone())
        {
            Debug.Log("Forbidden Zone !");
            return;
        }

        Instantiate(
            currentDef.prefab,
            pos,
            Quaternion.Euler(0, 0, currentRotation)
        );

        GameManager.Instance.placedObjects.Add(new PlacedObjectData()
        {
            id = currentDef.id,
            position = pos,
            rotation = currentRotation
        });


        GameManager.Instance.AddMoney(-currentDef.cost);
        Debug.Log("Placement OK, money updated : " + GameManager.Instance.money);

        audioSource.clip = cashRegister;
        audioSource.Play();

        StopPlacing();
    }

    // --------------------------------------------------------------
    // FORBIDDEN ZONE CHECK
    // --------------------------------------------------------------
    bool IsOverForbiddenZone()
    {
        Collider2D hit = Physics2D.OverlapCircle(previewRenderer.transform.position, 0.1f, forbiddenLayer);
        return hit != null;
    }

    // --------------------------------------------------------------
    // STOP PLACING
    // --------------------------------------------------------------
    public void StopPlacing()
    {
        isPlacing = false;
        prefabToPlace = null;

        if (previewRenderer != null)
            previewRenderer.gameObject.SetActive(false);
    }

    // --------------------------------------------------------------
    // SELECT OBJECT
    // --------------------------------------------------------------
    public void SelectObjectForMove(SelectableObject obj)
    {
        // Désélectionner un éventuel autre objet
        if (selectedObject != null)
            selectedObject.SetSelected(false);

        selectedObject = obj;
        selectedObject.SetSelected(true);

        isMovingExisting = true;
        isPlacing = true;
        currentDef = null;

        if(previewRenderer != null)
        {
            previewRenderer.gameObject.SetActive(true);
            previewRenderer.sprite = selectedObject.GetComponentInChildren<SpriteRenderer>().sprite;
            previewRenderer.color = new Color(1, 1, 1, 0.5f);
            previewRenderer.transform.localScale = selectedObject.transform.GetChild(0).localScale;

        }

        currentRotation = selectedObject.transform.eulerAngles.z;
    }

    // --------------------------------------------------------------
    // MOVE EXISTING OBJECT
    // --------------------------------------------------------------
    void PlaceExistingObject()
    {
        
        if (IsOverForbiddenZone())
        {
            Debug.Log("Zone interdite !");
            return;
        }

        Vector3 pos = previewRenderer.transform.position;

        
        selectedObject.transform.position = pos;
        selectedObject.transform.rotation = Quaternion.Euler(0, 0, currentRotation);


        float epsilon = 0.01f;
        var data = GameManager.Instance.placedObjects
            .Find(o => Vector2.Distance(o.position, selectedObject.transform.position) < epsilon);

        if (data != null)
        {
            data.position = pos;
            data.rotation = currentRotation;
        }

        EndMove();
    }
    // --------------------------------------------------------------
    // CANCEL MOVE
    // --------------------------------------------------------------
    void CancelMove()
    {
        EndMove();
        selectedObject = null;
    }

    // --------------------------------------------------------------
    // END MOVE
    // --------------------------------------------------------------
    void EndMove()
    {
        selectedObject?.SetSelected(false);
        isMovingExisting = false;
        isPlacing = false;
        previewRenderer.gameObject.SetActive(false);
    }
}