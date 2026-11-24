using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform player;

    [Header("Posición")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 4f, -7f);

    [Header("Seguimiento")]
    [SerializeField] private float smoothSpeed = 5f;

    [Header("Orientación")]
    [SerializeField] private float lookHeight = 1.2f;
    [SerializeField] private bool followPlayerRotation = true;
    [Range(0f, 1f)]
    [SerializeField] private float rotationFollowAmount = 0.35f;
    [SerializeField] private float rotationSpeed = 5f;

    private void LateUpdate()
    {
        if (player == null) return;

        // Calcular posición de seguimiento
        Vector3 desiredOffset = offset;

        if (followPlayerRotation)
        {
            float playerYaw = player.eulerAngles.y;
            float limitedYaw = playerYaw * rotationFollowAmount;

            desiredOffset = Quaternion.Euler(
                0f,
                limitedYaw,
                0f
            ) * offset;
        }

        Vector3 desiredPosition = player.position + desiredOffset;

        // Suavizar movimiento
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        // Punto al que mira la cámara
        Vector3 lookTarget = player.position +
                             Vector3.up * lookHeight;

        Vector3 direction = lookTarget - transform.position;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion desiredRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                desiredRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
