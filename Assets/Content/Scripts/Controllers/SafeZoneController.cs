using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!RescueManager.Instance.playerCarryingVictim)
            return;

        if (other.CompareTag("Player")) // hélicoptère
        {
            RescueManager.Instance.DropVictim();
        }
    }
}