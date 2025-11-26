using UnityEngine;
using UnityEngine.UI;

public class UIRescueArrow : MonoBehaviour
{
    public static UIRescueArrow Instance;

    public RectTransform arrowUI;
    public float edgePadding = 40f;

    private Camera cam;
    private Vector3 targetWorldPos;
    private bool active = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cam = Camera.main;
        arrowUI.gameObject.SetActive(false);
    }

    public void Activate(Vector3 worldPos)
    {
        targetWorldPos = worldPos;
        active = true;
        arrowUI.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        active = false;
        arrowUI.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!active) return;

        Vector3 screenPos = cam.WorldToScreenPoint(targetWorldPos);

        if (screenPos.z > 0 &&
            screenPos.x > 0 && screenPos.x < Screen.width &&
            screenPos.y > 0 && screenPos.y < Screen.height)
        {
            arrowUI.gameObject.SetActive(false);
            return;
        }

        arrowUI.gameObject.SetActive(true);

        Vector3 fromCenter = screenPos - new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        fromCenter.z = 0;

        float angle = Mathf.Atan2(fromCenter.y, fromCenter.x) * Mathf.Rad2Deg;
        arrowUI.rotation = Quaternion.Euler(0, 0, angle - 90f);

        fromCenter = fromCenter.normalized;

        Vector3 pos = fromCenter * ((Screen.height / 2f) - edgePadding);
        arrowUI.anchoredPosition = pos;
    }

    public void UpdateTarget(Vector3 worldPos)
    {
        targetWorldPos = worldPos;
    }
}