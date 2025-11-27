using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;

    [Header("Prefabs")]
    public GameObject fireCrewPrefab;
    public GameObject fireLinePrefab;
    public GameObject safeZonePrefab;

    [Header("Forbidden Zone Detection")]
    public LayerMask forbiddenLayer;

    [Header("Preview")]
    public SpriteRenderer previewRenderer;
    public Color previewColor = new Color(1, 1, 1, 0.5f);

    public AudioSource audioSource;
    public AudioClip cashRegister;

    private Camera cam;

    // ----- Placement states -----
    public bool isPlacing = false;         // Placing new object
    public bool isMovingExisting = false;  // Moving an existing object

    private PlaceableObjectDefinition currentDef;
    private SelectableObject selectedObject = null;
    private float currentRotation = 0f;

    private void Awake()
    {
        Instance = this;
        cam = Camera.main;

        if (previewRenderer != null)
            previewRenderer.gameObject.SetActive(false);
    }

    private void Update()
    {
        // ---------- 1. Handle selection when NOT placing or moving ----------
        if (!isPlacing && !isMovingExisting)
        {
            return;
        }

        // ---------- 2. Update preview ----------
        UpdatePreviewPosition();
        HandleRotation();

        // ---------- 3. Moving an existing object ----------
        if (isMovingExisting)
        {
            if (Input.GetMouseButtonDown(0))
            {
                PlaceExistingObject();
                return;
            }

            if (Input.GetMouseButtonDown(1))
            {
                CancelMove();
                return;
            }

            return;
        }

        // ---------- 4. Placing new object ----------
        if (isPlacing && currentDef != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceObject();
                return;
            }

            if (Input.GetMouseButtonDown(1))
            {
                StopPlacing();
                return;
            }
        }
    }

    public void SelectObject(SelectableObject obj)
    {
        if (selectedObject != null)
        {
            selectedObject.SetSelected(false);
            selectedObject = null;
        }

        selectedObject = obj;
        selectedObject.SetSelected(true);
    }

    // ===========================================================================
    //  START PLACING NEW OBJECT
    // ===========================================================================

    public void StartPlacing(PlaceableObjectDefinition def)
    {
        if (GameManager.Instance.money < def.cost)
            return;

        // Cancel any selection
        if (selectedObject != null)
        {
            selectedObject.SetSelected(false);
            selectedObject = null;
        }

        currentDef = def;
        isPlacing = true;
        isMovingExisting = false;

        currentRotation = 0;

        SpriteRenderer sr = def.prefab.GetComponentInChildren<SpriteRenderer>();
        previewRenderer.sprite = sr.sprite;
        previewRenderer.color = previewColor;
        previewRenderer.transform.localScale = sr.transform.localScale;
        previewRenderer.transform.rotation = Quaternion.identity;
        previewRenderer.gameObject.SetActive(true);
    }

    // ===========================================================================
    //  PREVIEW + ROTATION
    // ===========================================================================

    void UpdatePreviewPosition()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = -0.01f;
        previewRenderer.transform.position = mousePos;
    }

    void HandleRotation()
    {
        if (Input.GetKeyDown(KeyCode.R))
            currentRotation += 90f;

        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
            currentRotation += scroll * 10f;

        previewRenderer.transform.rotation = Quaternion.Euler(0, 0, currentRotation);
    }

    // ===========================================================================
    //  PLACE NEW OBJECT
    // ===========================================================================

    void TryPlaceObject()
    {
        Vector3 pos = previewRenderer.transform.position;

        if (IsOverForbiddenZone())
            return;

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

        if (audioSource != null && cashRegister != null)
        {
            audioSource.clip = cashRegister;
            audioSource.Play();
        }

        if (currentDef.id == "FIRE_CREW")
        {
            GameManager.Instance.AddFireCrew(1);
        }
        if (currentDef.id == "FIRE_LINE")
        {
            GameManager.Instance.AddFireLine(1);
        }
        if (currentDef.id == "SAFE_ZONE")
        {
            GameManager.Instance.AddSafeZone(1);
        }

        StopPlacing();
    }

    bool IsOverForbiddenZone()
    {
        return Physics2D.OverlapCircle(previewRenderer.transform.position, 0.1f, forbiddenLayer);
    }

    // ===========================================================================
    //  MOVE EXISTING OBJECT
    // ===========================================================================

    public void SelectObjectForMove(SelectableObject obj)
    {
        isMovingExisting = true;
        isPlacing = true;    


        if (selectedObject != obj)
            SelectObject(obj);

        previewRenderer.gameObject.SetActive(true);
        SpriteRenderer sr = obj.GetComponentInChildren<SpriteRenderer>();
        previewRenderer.sprite = sr.sprite;
        previewRenderer.color = previewColor;
        previewRenderer.transform.localScale = sr.transform.localScale;

        currentRotation = obj.transform.eulerAngles.z;
    }

    void PlaceExistingObject()
    {
        if (IsOverForbiddenZone())
            return;

        Vector3 pos = previewRenderer.transform.position;
        pos.z = -0.01f;

        selectedObject.transform.position = pos;
        selectedObject.transform.rotation = Quaternion.Euler(0, 0, currentRotation);

        // Update saved data
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

    void CancelMove()
    {
        EndMove();
    }

    void EndMove()
    {
        if (selectedObject != null)
            selectedObject.SetSelected(false);

        isMovingExisting = false;
        isPlacing = false;
        previewRenderer.gameObject.SetActive(false);
        selectedObject = null;
    }

    public void StopPlacing()
    {
        isPlacing = false;
        currentDef = null;

        previewRenderer.gameObject.SetActive(false);
    }
}
