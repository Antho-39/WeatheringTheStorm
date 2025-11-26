using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public float dragSpeed = 1f;
    public float zoomSpeed = 5f;
    public float smoothTime = 0.05f;

    [Header("Camera Limits")]
    public Vector2 minBounds = new Vector2(-16f, -10f);
    public Vector2 maxBounds = new Vector2(16f, 11f);

    public PreparationPhaseUIController UIController;

    private Vector3 dragOrigin;
    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;

    private Camera mainCamera;
    private int dragButton = -1;

    void Start()
    {  
        mainCamera = GetComponentInChildren<Camera>();
        targetPosition = transform.position;
    }

    void Update()
    {
        if (mainCamera == null)
            return;

        if (Input.GetMouseButtonDown(2)) // middle mouse
        {
            dragButton = 2;
            dragOrigin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            UIController.SetDragCursor(true);
        }
        else if (Input.GetMouseButtonDown(1))  // right mouse
        {
            dragButton = 1;
            dragOrigin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            UIController.SetDragCursor(true);
        }

        if (dragButton != -1 && Input.GetMouseButton(dragButton))
        {
            Vector3 difference = dragOrigin - mainCamera.ScreenToWorldPoint(Input.mousePosition);
            targetPosition = transform.position + difference * dragSpeed;
        }
        if (dragButton != -1 && Input.GetMouseButtonUp(dragButton))
        {
            dragButton = -1;
            UIController.SetDragCursor(false);
        }

        if (PlacementManager.Instance != null && PlacementManager.Instance.isPlacing)
            return;

        // ---------------------------------------------------------
        // 3. ZOOM CAMERA
        // ---------------------------------------------------------
        mainCamera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, 3f, 5f);

    }


    void LateUpdate()
    {
        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        float minX = minBounds.x + camWidth;
        float maxX = maxBounds.x - camWidth;

        float minY = minBounds.y + camHeight;
        float maxY = maxBounds.y - camHeight;

        // --- CLAMP LIMIT MOVEMENT ---
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }
}
