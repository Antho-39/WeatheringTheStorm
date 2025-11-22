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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int startedFires = 0;
        weatherVolume = GetComponentInChildren<BoxCollider2D>();
        
        while (startedFires < numberOfInitialFires)
        {
            UnityEngine.Vector2 targetSpawn = GetRandomPointInSquare(weatherVolume.bounds);

            if (!Physics2D.OverlapCircle(targetSpawn, 1f, InFlammableLayers))
            {
                Instantiate(firePrefab, targetSpawn, quaternion.identity);
                startedFires++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private UnityEngine.Vector2 GetRandomPointInSquare(Bounds boxCollider)
    {
        return new UnityEngine.Vector2(
            Random.Range(boxCollider.min.x, boxCollider.max.x), 
            Random.Range(boxCollider.min.y, boxCollider.max.y));
    }
}
