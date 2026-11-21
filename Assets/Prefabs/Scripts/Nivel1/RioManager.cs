using UnityEngine;

public class RiverCollider : MonoBehaviour
{
    [SerializeField] private CheckpointManager checkpointManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            checkpointManager.PlayerFell();
        }
    }
}