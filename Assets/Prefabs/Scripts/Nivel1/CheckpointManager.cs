using System.Collections;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private Transform playerTransform;

    [Header("Respawn")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private float respawnDelay = 3f;

    private Transform currentRespawnPoint;
    private int currentCheckpointIndex = -1;
    private bool isRespawning = false;

    private void Start()
    {
        currentRespawnPoint = startPoint;
    }

    public void ActivateCheckpoint(int index, Transform respawnPoint)
    {
        if (index <= currentCheckpointIndex)
            return;

        currentCheckpointIndex = index;
        currentRespawnPoint = respawnPoint;

        Debug.Log("Checkpoint activado: " + (index + 1));
    }
    public int GetCurrentCheckpoint()
    {
        return currentCheckpointIndex;
    }
    public void PlayerFell()
    {
        if (isRespawning)
            return;

        StartCoroutine(RespawnPlayer());
    }

    private IEnumerator RespawnPlayer()
    {
        isRespawning = true;

        // Detener el movimiento físico.
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        // Esperar 3 segundos.
        yield return new WaitForSeconds(respawnDelay);

        // Volver al último checkpoint.
        if (currentRespawnPoint != null)
        {
            playerTransform.position = currentRespawnPoint.position;
            playerTransform.rotation = currentRespawnPoint.rotation;
        }

        // Limpiar velocidades.
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        PlayerController playerController = playerTransform != null
            ? playerTransform.GetComponent<PlayerController>()
            : null;

        if (playerController != null)
            playerController.StartCalibration();

        // Sincronizar física.
        Physics.SyncTransforms();

        isRespawning = false;

        Debug.Log("Jugador reapareció en el último checkpoint.");
    }
}