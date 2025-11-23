using System.Numerics;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    public LayerMask InFlammableLayers;
    public GameObject firePrefab;
    public int numberOfInitialFires;
    private BoxCollider2D weatherVolume;
    private UnityEngine.Vector2 targetSpawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int startedFires = 0;
        weatherVolume = GetComponentInChildren<BoxCollider2D>();
        
        while (startedFires < numberOfInitialFires)
        {
            targetSpawn = GetRandomPointInSquare(weatherVolume.bounds);

            if (!Physics2D.OverlapCircle(targetSpawn, 1f, InFlammableLayers))
            {
                Instantiate(firePrefab, targetSpawn, quaternion.identity);
                startedFires++;
            }
        }

        InvokeRepeating(nameof(FireStarter), 30f, 30f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FireStarter()
    {
        bool fireStarted = false;

        while (!fireStarted)
        {
            targetSpawn = GetRandomPointInSquare(weatherVolume.bounds);
            
            if (!Physics2D.OverlapCircle(targetSpawn, 1f, InFlammableLayers))
            {
                Instantiate(firePrefab, targetSpawn, quaternion.identity);
                fireStarted = true;
            }
        }
    }

    private UnityEngine.Vector2 GetRandomPointInSquare(Bounds boxCollider)
    {
        return new UnityEngine.Vector2(
            Random.Range(boxCollider.min.x, boxCollider.max.x), 
            Random.Range(boxCollider.min.y, boxCollider.max.y));
    }
}
