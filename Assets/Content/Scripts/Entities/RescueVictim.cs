using UnityEngine;

public class RescueVictim : MonoBehaviour
{
    public float lifetime = 15f;   // temps avant échec
    private float timer = 0f;
    private bool rescued = false;

    public System.Action<RescueVictim> OnRescueFail;

    [HideInInspector] public bool pickedUp = false;

    void Update()
    {
        if (pickedUp) return;

        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            OnRescueFail?.Invoke(this);
            Destroy(gameObject);
        }
    }

    public void Rescue()
    {
        pickedUp = true;
        GetComponent<AudioSource>().Stop();
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
