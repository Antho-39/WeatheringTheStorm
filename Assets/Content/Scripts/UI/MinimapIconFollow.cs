using UnityEngine;

public class MinimapIconFollow : MonoBehaviour
{
    public Transform target;

    void LateUpdate()
    {
        if (target != null)
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
    }
}
