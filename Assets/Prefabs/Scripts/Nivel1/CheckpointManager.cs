using System.Collections;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private Transform playerTransform;

    [Header("Respawn")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform[] checkpointRespawnPoints;
    [SerializeField] private float respawnDelay = 3f;

    private Transform currentRespawnPoint;
    private int currentCheckpointIndex = -1;
    private bool isRespawning = false;

    private void Start()
    {
        currentRespawnPoint = startPoint;

        int selectedCheckpoint = NivelManager.CheckpointSeleccionado;

        if (checkpointRespawnPoints != null &&
            selectedCheckpoint >= 0 &&
            selectedCheckpoint < checkpointRespawnPoints.Length)
        {
            currentCheckpointIndex = selectedCheckpoint;
            currentRespawnPoint = checkpointRespawnPoints[selectedCheckpoint];

            if (playerTransform != null && currentRespawnPoint != null)
            {
                playerTransform.SetPositionAndRotation(
                    currentRespawnPoint.position,
                    currentRespawnPoint.rotation
                );
            }

            if (EstadisticasManager.Instance != null)
            {
                EstadisticasManager.Instance.RegistrarCheckpoint(
                    selectedCheckpoint
                );
            }

            Physics.SyncTransforms();
        }
    }

    public void ActivateCheckpoint(int index, Transform respawnPoint)
    {
        if (index <= currentCheckpointIndex)
            return;

        currentCheckpointIndex = index;
        currentRespawnPoint = respawnPoint;

        if (EstadisticasManager.Instance != null)
            EstadisticasManager.Instance.RegistrarCheckpoint(index);

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

        if (EstadisticasManager.Instance != null)
            EstadisticasManager.Instance.RegistrarIntento();

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