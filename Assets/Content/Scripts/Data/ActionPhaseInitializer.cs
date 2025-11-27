using UnityEngine;

public class ActionPhaseInitializer : MonoBehaviour
{
    public GameObject fireCrewPrefab;
    public GameObject fireLinePrefab;
    public GameObject safeZonePrefab;

    void Start()
    {
        foreach (var data in GameManager.Instance.placedObjects)
        {
            var def = GameManager.Instance.database.GetById(data.id);
            if (def == null)
            {
                Debug.LogError("OBJ NOT FOUND: " + data.id);
                continue;
            }

            var prefab = Instantiate(
                def.prefab,
                data.position,
                Quaternion.Euler(0, 0, data.rotation)
            );

            Vector3 newPosition = prefab.transform.position;
            newPosition.z = -0.01f;
            transform.position = newPosition;

            if (data.id == "SAFE_ZONE")
            {
                RescueManager.Instance.safeZones.Add(prefab.transform);
            }
        }

        GameManager.Instance.placedObjects.Clear();
    }
}
