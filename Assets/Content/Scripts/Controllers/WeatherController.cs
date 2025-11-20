using System.Numerics;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    public GameObject firePrefab;
    public int numberOfInitialFires;
    private BoxCollider2D weatherVolume;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weatherVolume = GetComponentInChildren<BoxCollider2D>();

        for (int i = 0; i < numberOfInitialFires; i++)
        {
            Instantiate(firePrefab, GetRandomPointInSquare(weatherVolume.bounds), quaternion.identity);
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
