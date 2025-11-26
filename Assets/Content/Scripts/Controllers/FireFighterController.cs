using UnityEngine;

public class FireFighterController : MonoBehaviour
{
    public Collider2D ExtinguishCollider;


    void OnEnable()
    {
        if (ExtinguishCollider == null)
            return;
        if (GameManager.Instance.currentPhase == GameManager.Phase.Phase2)
        {
            ExtinguishCollider.enabled = true;
        }
        else
        {
            ExtinguishCollider.enabled = false;
        }
    }
}
