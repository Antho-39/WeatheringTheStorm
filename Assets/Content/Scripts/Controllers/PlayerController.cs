using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public float dragSpeed = 1f;
    public float zoomSpeed = 5f;
    public float smoothTime = 0.05f;

    private Vector3 dragOrigin;
    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;

    private Camera mainCamera;

    void Start()
    {  
        mainCamera = GetComponentInChildren<Camera>();
        targetPosition = transform.position;
    }

    void Update()
    {
        if (mainCamera == null)
        {
            return;
        }
        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 difference = dragOrigin - mainCamera.ScreenToWorldPoint(Input.mousePosition);
            targetPosition = transform.position + difference * dragSpeed;
        }
        mainCamera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, 2f, 20f);
    }
    void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }
}
