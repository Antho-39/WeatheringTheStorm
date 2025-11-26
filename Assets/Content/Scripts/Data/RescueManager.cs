using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescueManager : MonoBehaviour
{
    public static RescueManager Instance;

    [Header("Prefabs")]
    public GameObject[] victimPrefabs;
    public List<Transform> safeZones;
    public LayerMask forbiddenLayers;

    [Header("Spawn Settings")]
    public float spawnInterval = 55f;
    public float rescueTimeLimit = 20f;
    public bool playerCarryingVictim = false;
    public GameObject currentCarriedVictim;
    public bool rescueInProgress = false;

    public Transform playerTransform;

    public Collider2D mapArea;
    private Vector2 mapMinBounds;
    private Vector2 mapMaxBounds;

    private Coroutine currentVictimTimerCoroutine;
    public ActionPhaseUIController HUD;

    private List<GameObject> activeVictims = new List<GameObject>();
    private Coroutine spawnCoroutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        spawnCoroutine = StartCoroutine(SpawnVictimsRoutine());
        mapMinBounds = mapArea.bounds.min;
        mapMaxBounds = mapArea.bounds.max;
    }

    IEnumerator SpawnVictimsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            if (!rescueInProgress)
                SpawnVictim();
        }
    }

    void SpawnVictim()
    {
        Debug.Log(rescueInProgress);
        if (rescueInProgress) return;

        Vector3 spawnPos = Vector3.zero;
        int attempts = 0;
        bool validPos = false;

        while (!validPos && attempts < 20)
        {
            spawnPos = new Vector3(
                Random.Range(mapMinBounds.x, mapMaxBounds.x),
                Random.Range(mapMinBounds.y, mapMaxBounds.y),
                0f
            );

            Collider2D hit = Physics2D.OverlapCircle(spawnPos, 0.5f, forbiddenLayers);
            if (hit == null)
                validPos = true;

            attempts++;
        }

        if (!validPos) return;

        int prefabIndex = Random.Range(0, victimPrefabs.Length);
        GameObject victim = Instantiate(victimPrefabs[prefabIndex], spawnPos, Quaternion.identity);
        activeVictims.Add(victim);

        HUD.ShowRescueAlert(victim);
        currentVictimTimerCoroutine = StartCoroutine(VictimTimer(victim));
        rescueInProgress = true;
    }

    IEnumerator VictimTimer(GameObject victim)
    {
        float timer = rescueTimeLimit;
        while (timer > 0)
        {
            if (victim == null) yield break;
            if (playerCarryingVictim) yield break;

            timer -= Time.deltaTime;
            yield return null;
        }

        if (victim != null)
        {
            activeVictims.Remove(victim);
            Destroy(victim);
            rescueInProgress = false;
            HUD.ShowRescueFailed();
        }
    }

    public void RescueVictim(GameObject victim)
    {
        if (activeVictims.Contains(victim))
        {
            if (currentVictimTimerCoroutine != null)
            {
                StopCoroutine(currentVictimTimerCoroutine);
                currentVictimTimerCoroutine = null;
            }

            activeVictims.Remove(victim);
            playerCarryingVictim = true;
            currentCarriedVictim = victim;
            rescueInProgress = true;
            HUD.ShowRescueCarryMessage();
        }
    }

    public void DropVictim()
    {
        Debug.Log("Drop victim");
        HUD.ShowRescueSuccess();
        playerCarryingVictim = false;

        if (currentCarriedVictim != null)
            Destroy(currentCarriedVictim);

        currentCarriedVictim = null;
        rescueInProgress = false;
    }
}
