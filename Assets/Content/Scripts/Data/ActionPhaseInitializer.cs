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

            Instantiate(
                def.prefab,
                data.position,
                Quaternion.Euler(0, 0, data.rotation)
            );
        }

        // Optionnel : vider après utilisation
        GameManager.Instance.placedObjects.Clear();
    }
}
