using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class HelicopterController : MonoBehaviour
{
    [Header("References")]
    public Transform[] helicopterBlades;
    public Transform Body;
    public Transform TailRotor;
    public Collider2D mapArea;

    [Header("Movement")]
    public float moveSpeed = 10f;
    public float rotationSpeed = 5f;
    public GameObject MiniMapCamera;
    public GameObject MiniMapCanvas;

    [Header("Audio")]
    public AudioSource helicopterAudioSource;
    public AudioSource chopperAudioSource;
    public AudioClip bladeAudio;
    public AudioClip waterCanonAudio;

    public LayerMask water;

    [Header("Water")]
    public float climbSpeed = 3f;
    private GaugeController gauge;
    private Rigidbody2D chopperRigidbody;
    private ParticleSystem waterParticles;
    private ParticleSystem.EmissionModule emission;

    [Header("Camera")]
    private Camera mainCamera;
    private Quaternion targetRotation;

    [Header("Rescue")]
    public float pickupRadius = 1f;
    public float dropRadius = 1f;
    public Transform victimHoldPoint;
    private float rescueDuration;
    private float rescueTimer = 0f;
    private bool isRescuing = false;
    private bool isDropping = false;
    private bool isTransportingVictim = false;
    private Collider2D currentVictim;

    [Header("Rescue Gauge")]
    public Canvas rescueCanvas;
    public UnityEngine.UI.Image rescueFillImage;


    private GameObject carriedVictim;

    void Start()
    {
        mainCamera = GetComponentInChildren<Camera>();
        gauge = GetComponent<GaugeController>();
        chopperRigidbody = GetComponent<Rigidbody2D>();
        waterParticles = GetComponentInChildren<ParticleSystem>();
        emission = waterParticles.emission;
        targetRotation = mainCamera.transform.rotation;

        // Setup helicopter audio
        helicopterAudioSource.clip = bladeAudio;
        helicopterAudioSource.loop = true;
        helicopterAudioSource.playOnAwake = false;
        helicopterAudioSource.volume = 0f;
        helicopterAudioSource.Play();

        chopperAudioSource.clip = waterCanonAudio;
        chopperAudioSource.loop = true;
        chopperAudioSource.playOnAwake = false;
        chopperAudioSource.volume = 0f;
        rescueDuration = 1.0f;
    }

    void Update()
    {
        RotateBlades();

        if (!isTransportingVictim)
        {
            HandleWaterCannon();
            RefillWater();
        }

        ToggleMiniMap();
        ClampPosition();

        HandleRescuePickup();
        HandleRescueDrop();
        UpdateCarriedVictimPosition();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void LateUpdate()
    {
        mainCamera.transform.rotation = targetRotation;
    }

    #region Helicopter
    private void RotateBlades()
    {
        foreach (var blade in helicopterBlades)
        {
            blade.localRotation *= Quaternion.Euler(0, 0, 360f * Time.deltaTime * rotationSpeed);
        }

        float speed = chopperRigidbody.linearVelocity.magnitude;
        helicopterAudioSource.volume = Mathf.InverseLerp(0f, 25f, speed);
    }

    private void HandleWaterCannon()
    {
        float cannonInput = Input.GetAxisRaw("Jump");

        if (cannonInput > 0 && gauge.water > 0f)
        {
            emission.enabled = true;
            gauge.ConsumeValue(0.03f);

            if (!chopperAudioSource.isPlaying) chopperAudioSource.Play();
            chopperAudioSource.volume = 0.7f;
        }
        else
        {
            emission.enabled = false;
            if (chopperAudioSource.volume > 0f)
            {
                chopperAudioSource.volume -= Time.deltaTime * 3f;
                if (chopperAudioSource.volume <= 0.01f) chopperAudioSource.Stop();
            }
        }
    }

    private void ToggleMiniMap()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            bool active = MiniMapCamera.activeSelf;
            MiniMapCamera.SetActive(!active);
            MiniMapCanvas.SetActive(!active);
        }
    }

    private void RefillWater()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, water) && gauge.water < 100f)
        {
            gauge.ConsumeValue(-0.05f);
        }
    }

    private void ClampPosition()
    {
        Bounds bounds = mapArea.bounds;
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, bounds.min.x, bounds.max.x);
        pos.y = Mathf.Clamp(pos.y, bounds.min.y, bounds.max.y);
        transform.position = pos;
    }

    private void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, v, 0f);

        if (input.sqrMagnitude < 0.0001f) return;

        Vector3 direction = input.normalized;
        float throttle = v * moveSpeed;
        float steering = (h / 7f) * rotationSpeed;

        chopperRigidbody.AddForce(transform.up * throttle);
        chopperRigidbody.AddForceAtPosition(transform.right * (-steering), TailRotor.position);
    }
    #endregion

    #region Rescue
    private void HandleRescuePickup()
    {
        if (carriedVictim != null) return;


        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pickupRadius);
        Collider2D victim = null;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Victim"))
            {
                victim = hit;
                break;
            }
        }

        if (victim != null)
        {
            if (!isRescuing)
            {

                isRescuing = true;
                rescueTimer = 0f;
                currentVictim = victim;
            }
            else
            {

                rescueTimer += Time.deltaTime;
                rescueFillImage.fillAmount = rescueTimer / rescueDuration;
                if (rescueTimer >= rescueDuration)
                {
                    carriedVictim = currentVictim.gameObject;
                    RescueManager.Instance.RescueVictim(carriedVictim);
                    carriedVictim.transform.SetParent(victimHoldPoint);
                    carriedVictim.transform.position = victimHoldPoint.position;

                    isRescuing = false;
                    currentVictim = null;

                    isTransportingVictim = true;
                    emission.enabled = false;
                    chopperAudioSource.Stop();
                }
            }
        }
        else
        {
            isRescuing = false;
            rescueTimer = 0f;
            rescueFillImage.fillAmount = 0f;
            currentVictim = null;
        }

    }


    private void HandleRescueDrop()
    {
        if (carriedVictim == null) return;

        Transform safeZone = null;
        foreach (var zone in RescueManager.Instance.safeZones)
        {
            if (Vector2.Distance(transform.position, zone.position) <= dropRadius)
            {
                safeZone = zone;
                break;
            }
        }

        if (safeZone != null)
        {
            if (!isDropping)
            {
                isDropping = true;
                rescueTimer = 0f;
            }
            else
            {
                rescueTimer += Time.deltaTime;
                rescueFillImage.fillAmount = rescueTimer / rescueDuration;

                if (rescueTimer >= rescueDuration)
                {
                    carriedVictim.transform.SetParent(null);
                    RescueManager.Instance.DropVictim();
                    carriedVictim = null;

                    isDropping = false;
                    rescueTimer = 0f;
                    isTransportingVictim = false;
                }
            }
        }
        else
        {
            isDropping = false;
            rescueFillImage.fillAmount = 0.0f;
            rescueTimer = 0f;
        }
    }


    private void UpdateCarriedVictimPosition()
    {
        if (carriedVictim != null)
        {
            carriedVictim.transform.position = victimHoldPoint.position;
        }
    }
    #endregion
}
