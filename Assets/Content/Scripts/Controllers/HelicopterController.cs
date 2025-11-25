using UnityEngine;
using System.Collections;
using Unity.Mathematics;
using System.Numerics;

public class HelicopterController : MonoBehaviour
{
    public Transform[] helicopterBlades; // Array to hold references to helicopter blades
    public Transform Body;            // Reference to the helicopter body
    public Transform TailRotor;      // Reference to the tail rotor for steering
    public Collider2D mapArea;

    [Header("Movement")]
    public float moveSpeed = 10f;        // Move speed (unit / second)
    public float rotationSpeed = 5f;     // Speed roation to aim at the move direction
    public GameObject MiniMapCamera;
    public GameObject MiniMapCanvas;

    public AudioClip bladeAudio;
    public AudioClip waterCanonAudio;
    // I'm very sure I don't need to make a public layer mask for this, just not sure on syntax to specify explicitly only the water layer
    public LayerMask water;

    public float height = 0f;
    public float climbSpeed = 3f;
    private GaugeController gauge;
    private Rigidbody2D chopperRigidbody; // Reference to helicoper rigidbody

    private Camera mainCamera;
    private ParticleSystem waterParticles; // Reference to particle system
    private UnityEngine.Quaternion targetRotation;
    private ParticleSystem.EmissionModule emission;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = GetComponentInChildren<Camera>();
        gauge = GetComponent<GaugeController>();
        chopperRigidbody = GetComponent<Rigidbody2D>();
        waterParticles = GetComponentInChildren<ParticleSystem>();

        // I guess this is a terrible way to stop the camera inheriting the chopper's rotation? 
        targetRotation = mainCamera.transform.rotation;

        emission = waterParticles.emission;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var blade in helicopterBlades)
        {
            blade.localRotation *= UnityEngine.Quaternion.Euler(0, 0, 360f * Time.deltaTime);
        }

        float cannonInput = Input.GetAxisRaw("Jump"); // space key

        // float angle = 0.0f;

        if (cannonInput > 0 && gauge.water > 0f)
        {
            emission.enabled = true;
            gauge.ConsumeValue(0.03f);
        }
        else
        {
            emission.enabled = false;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            height = Mathf.Min(height + climbSpeed * Time.deltaTime, 10f);
        }            
        else
        {
            height = Mathf.Max(height - climbSpeed * Time.deltaTime, 0f);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            bool miniMapActive = MiniMapCamera.activeSelf;
            MiniMapCamera.SetActive(!miniMapActive);
            MiniMapCanvas.SetActive(!miniMapActive);
        }  

        if (Physics2D.OverlapCircle(transform.position, 0.2f, water) && gauge.water < 100f)
        {
            gauge.ConsumeValue(-0.05f);
        }

        // Stop chopper from leaving map area
        Bounds mapBounds = mapArea.bounds;
        UnityEngine.Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(transform.position.x, mapBounds.min.x, mapBounds.max.x);
        clampedPosition.y = Mathf.Clamp(transform.position.y, mapBounds.min.y, mapBounds.max.y);
        clampedPosition.z = transform.position.z;

        transform.position = clampedPosition;
        // float waterBurned = Time.deltaTime + input.magnitude * Time.deltaTime; // Decrease water based on movement
        // Update gauge with animated water value
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal"); // Arrow left/right or A/D
        float v = Input.GetAxisRaw("Vertical");   // Arrow up/down or W/S
        UnityEngine.Vector3 input = new UnityEngine.Vector3(h, v, 0.0f);
        float throttle = v * moveSpeed;

        // Dividing the horizontal axis values because they are far too high. Probably a way better way to do this?
        float steering = (h / 7) * rotationSpeed;
        
        if (input.sqrMagnitude > 0.0001f)
        {
            // Normalization of the input vector to get the direction
            UnityEngine.Vector3 direction = input.normalized;

            // Move the helicopter in the input direction
            //transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);

            // Apply force to helicopter rigidbody in the direction the chopper is facing
            chopperRigidbody.AddForce(transform.up * throttle);
            // Steering has to be inverted for the force at the tail
            chopperRigidbody.AddForceAtPosition(transform.right * (steering * -1), TailRotor.position);

            // Rotate around the Z axis to face the move direction
            // angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90.0f;
            // Quaternion targetRot = Quaternion.Euler(0f, 0f, angle);
            // Body.rotation = Quaternion.Slerp(Body.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    void LateUpdate()
    {
        mainCamera.transform.rotation = targetRotation;
    }
}
