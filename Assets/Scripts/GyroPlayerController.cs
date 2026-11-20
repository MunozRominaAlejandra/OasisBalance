
using UnityEngine;

public class GyroPlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float acceleration = 5f;

    [Header("Sensibilidad")]
    [SerializeField] private float tiltSensitivity = 2f;
    [SerializeField] private float maxTiltAngle = 25f;

    [Header("Filtro de temblor")]
    [SerializeField] private float smoothing = 8f;
    [SerializeField] private float deadZone = 0.04f;
    [SerializeField] private float movementThreshold = 0.18f;
    [SerializeField] private float persistenceTime = 0.12f;

    [Header("Control")]
    [SerializeField] private bool invertHorizontal = false;
    [SerializeField] private bool invertVertical = false;

    private Rigidbody rb;

    private Quaternion calibrationRotation;

    private Vector2 filteredInput;
    private Vector2 stableInput;
    private Vector2 currentVelocity;

    private float horizontalTimer;
    private float verticalTimer;

    private bool gyroAvailable;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Activar giroscopio
        gyroAvailable = SystemInfo.supportsGyroscope;

        if (gyroAvailable)
        {
            Input.gyro.enabled = true;

            // Esperar a que el sensor esté disponible y calibrar
            calibrationRotation = GetGyroRotation();
        }
        else
        {
            Debug.LogWarning("Este dispositivo no tiene giroscopio disponible.");
        }
    }

    void Update()
    {
        if (!gyroAvailable)
            return;

        // Leer inclinación relativa a la posición inicial
        Quaternion relativeRotation =
            Quaternion.Inverse(calibrationRotation) * GetGyroRotation();

        Vector3 angles = relativeRotation.eulerAngles;

        // Convertir ángulos a valores entre -180 y 180
        float pitch = NormalizeAngle(angles.x);
        float roll = NormalizeAngle(angles.z);

        // Normalizar inclinación
        float horizontal = Mathf.Clamp(
            roll / maxTiltAngle, -1f, 1f);

        float vertical = Mathf.Clamp(
            pitch / maxTiltAngle, -1f, 1f);

        if (invertHorizontal)
            horizontal *= -1f;

        if (invertVertical)
            vertical *= -1f;

        Vector2 rawInput = new Vector2(horizontal, vertical);

        // 1. Filtro de suavizado
        filteredInput = Vector2.Lerp(
            filteredInput,
            rawInput,
            smoothing * Time.deltaTime
        );

        // 2. Zona muerta
        filteredInput.x = ApplyDeadZone(filteredInput.x);
        filteredInput.y = ApplyDeadZone(filteredInput.y);

        // 3. Detección de intención + persistencia
        stableInput.x = DetectIntent(
            filteredInput.x,
            stableInput.x,
            ref horizontalTimer
        );

        stableInput.y = DetectIntent(
            filteredInput.y,
            stableInput.y,
            ref verticalTimer
        );
    }

    void FixedUpdate()
    {
        if (!gyroAvailable || rb == null)
            return;

        Vector3 direction = new Vector3(
            stableInput.x,
            0f,
            stableInput.y
        );

        // Evitar velocidad diagonal superior a la normal
        direction = Vector3.ClampMagnitude(direction, 1f);

        Vector3 targetVelocity = direction * moveSpeed;

        // Aceleración y desaceleración progresiva
        currentVelocity = Vector2.Lerp(
            currentVelocity,
            new Vector2(targetVelocity.x, targetVelocity.z),
            acceleration * Time.fixedDeltaTime
        );

        Vector3 movement = new Vector3(
            currentVelocity.x,
            0f,
            currentVelocity.y
        );

        rb.MovePosition(
            rb.position + movement * Time.fixedDeltaTime
        );
    }

    float DetectIntent(float input, float previous, ref float timer)
    {
        // Si el movimiento supera el umbral, debe mantenerse
        // durante cierto tiempo para ser aceptado
        if (Mathf.Abs(input) >= movementThreshold)
        {
            timer += Time.deltaTime;

            if (timer >= persistenceTime)
                return input;
        }
        else
        {
            timer = 0f;
        }

        // Si no hay intención clara, volver gradualmente a cero
        return Mathf.Lerp(previous, 0f, 5f * Time.deltaTime);
    }

    float ApplyDeadZone(float value)
    {
        if (Mathf.Abs(value) < deadZone)
            return 0f;

        return value;
    }

    float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }

    Quaternion GetGyroRotation()
    {
        // Conversión de coordenadas del sensor a Unity
        return new Quaternion(
            Input.gyro.attitude.x,
            Input.gyro.attitude.y,
            -Input.gyro.attitude.z,
            -Input.gyro.attitude.w
        );
    }

    // Permite recalibrar la posición neutral
    public void Recalibrate()
    {
        if (!gyroAvailable)
            return;

        calibrationRotation = GetGyroRotation();

        filteredInput = Vector2.zero;
        stableInput = Vector2.zero;
        currentVelocity = Vector2.zero;

        horizontalTimer = 0f;
        verticalTimer = 0f;
    }
}

