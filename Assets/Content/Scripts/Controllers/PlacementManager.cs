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

    public bool isPlacing = false;
    private GameObject prefabToPlace;
    private int prefabCost;
    private Camera cam;

    private float currentRotation = 0f;
    private PlaceableObjectDefinition currentDef;

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
        if (!isPlacing) return;

        UpdatePreviewPosition();
        HandleRotation();

        if (Input.GetMouseButtonDown(0))
            TryPlaceObject();

        if (Input.GetMouseButtonDown(1))
            StopPlacing();
    }

    // --------------------------------------------------------------
    // START PLACING
    // --------------------------------------------------------------
    //public void StartPlacing(GameObject prefab, int cost)
    //{
    //    if (GameManager.Instance.money < cost)
    //    {
    //        Debug.Log("Not enough money !");
    //        return;
    //    }

    //    prefabToPlace = prefab;
    //    prefabCost = cost;
    //    isPlacing = true;
    //    currentRotation = 0f;

    //    // Setup preview
    //    SpriteRenderer prefabSprite = prefab.GetComponentInChildren<SpriteRenderer>();
    //    previewRenderer.transform.localScale = prefabSprite.transform.localScale;

    //    if (prefabSprite != null && previewRenderer != null)
    //    {
    //        previewRenderer.sprite = prefabSprite.sprite;
    //        previewRenderer.color = previewColor;
    //        previewRenderer.transform.localRotation = Quaternion.identity;
    //        previewRenderer.gameObject.SetActive(true);
    //    }
    //}

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
        // Rotation avec R (90°)
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentRotation += 90f;
        }

        // Rotation avec molette
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
        {
            currentRotation += scroll * 10f; // plus précis
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


        // Dépenser l’argent
        GameManager.Instance.AddMoney(-currentDef.cost);
        Debug.Log("Placement OK, money updated : " + GameManager.Instance.money);

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
}