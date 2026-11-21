
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private CheckpointManager checkpointManager;

    [Tooltip("Checkpoint 01 = 0, Checkpoint 02 = 1, etc.")]
    [SerializeField] private int checkpointIndex;

    [Header("Punto de reaparición")]
    [SerializeField] private Transform respawnPoint;

    private bool activated;

    private void OnTriggerEnter(Collider other)
    {
        if (activated)
            return;

        if (!other.CompareTag("Player"))
            return;

        if (checkpointManager == null || respawnPoint == null)
            return;

        int previousCheckpoint =
            checkpointManager.GetCurrentCheckpoint();

        checkpointManager.ActivateCheckpoint(
            checkpointIndex,
            respawnPoint
        );

        if (checkpointManager.GetCurrentCheckpoint() >
            previousCheckpoint)
        {
            activated = true;
            Debug.Log("¡Checkpoint activado!");
        }
    }
}
