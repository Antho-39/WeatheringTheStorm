using UnityEngine;

public class RescueVictim : MonoBehaviour
{
    public float lifetime = 20f;
    private float timer = 0f;
    private bool rescued = false;

    public System.Action<RescueVictim> OnRescueFail;

    [HideInInspector] public bool pickedUp = false;


}
