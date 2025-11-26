using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescueManager : MonoBehaviour
{
    public static RescueManager Instance;

    [Header("Prefabs")]
    public GameObject[] victimPrefabs;       // Personnes ou animaux
    public Transform[] safeZones;            // Points où déposer les victimes
    public LayerMask forbiddenLayers;        // Eau et feu

    [Header("Spawn Settings")]
    public float spawnInterval = 10f;
    public float rescueTimeLimit = 15f;
    public bool playerCarryingVictim = false;
    public GameObject currentCarriedVictim;

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
    }

    IEnumerator SpawnVictimsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnVictim();
        }
    }

    void SpawnVictim()
    {
        Vector3 spawnPos = Vector3.zero;
        int attempts = 0;
        bool validPos = false;

        while (!validPos && attempts < 20)
        {
            spawnPos = new Vector3(
                Random.Range(-15f, 15f),
                Random.Range(-10f, 10f),
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

        HUD.ShowRescueAlert(spawnPos);
        StartCoroutine(VictimTimer(victim));
    }

    IEnumerator VictimTimer(GameObject victim)
    {
        float timer = rescueTimeLimit;
        while (timer > 0)
        {
            if (victim == null) yield break;
            timer -= Time.deltaTime;
            //HUD.UpdateRescueTimer(timer / rescueTimeLimit);
            yield return null;
        }

        if (victim != null)
        {
            activeVictims.Remove(victim);
            Destroy(victim);
            HUD.ShowRescueFailed();
        }
    }

    public void RescueVictim(GameObject victim)
    {
        if (activeVictims.Contains(victim))
        {
            activeVictims.Remove(victim);
            playerCarryingVictim = true;
            currentCarriedVictim = victim;
            HUD.ShowRescueCarryMessage();
        }
    }

    public void DropVictim()
    {
        if (currentCarriedVictim == null) return;

        HUD.ShowRescueSuccess();
        playerCarryingVictim = false;
        Destroy(currentCarriedVictim);
        currentCarriedVictim = null;
    }
}
