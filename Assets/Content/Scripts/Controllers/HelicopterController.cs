using UnityEngine;
using System.Collections;

public class HelicopterController : MonoBehaviour
{
    public Transform[] helicopterBlades; // Array to hold references to helicopter blades
    public Transform Body;            // Reference to the helicopter body

    [Header("Movement")]
    public float moveSpeed = 10f;        // Move speed (unit / second)
    public float rotationSpeed = 5f;     // Speed roation to aim at the move direction

    public float height = 0f;
    public float climbSpeed = 3f;
    private GaugeController gauge;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gauge = GetComponent<GaugeController>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var blade in helicopterBlades)
        {
            blade.localRotation *= Quaternion.Euler(0, 0, 360f * Time.deltaTime);
        }

        float h = Input.GetAxisRaw("Horizontal"); // Arrow left/right or A/D
        float v = Input.GetAxisRaw("Vertical");   // Arrow up/down or W/S

        Vector3 input = new Vector3(h, v, 0.0f);
        float angle = 0.0f;

        if (input.sqrMagnitude > 0.0001f)
        {
            // Normalization of the input vector to get the direction
            Vector3 direction = input.normalized;

            // Move the helicopter in the input direction
            transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);

            // Rotate around the Z axis to face the move direction
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90.0f;
            Quaternion targetRot = Quaternion.Euler(0f, 0f, angle);
            Body.rotation = Quaternion.Slerp(Body.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            height = Mathf.Min(height + climbSpeed * Time.deltaTime, 10f);
        }            
        else
        {
            height = Mathf.Max(height - climbSpeed * Time.deltaTime, 0f);
        }        

        float fuelBurned = Time.deltaTime + input.magnitude * Time.deltaTime; // Decrease fuel based on movement
        gauge.ConsumeValue(fuelBurned); // Update gauge with animated fuel value
    }
}
