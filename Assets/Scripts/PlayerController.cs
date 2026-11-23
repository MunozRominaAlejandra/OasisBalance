
using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float acceleration = 2.5f;
    [SerializeField] private float deceleration = 8f;

    [Header("Giroscopio")]
    [SerializeField] private float turnSensitivity = 1f;
    [SerializeField] private float accelerometerTurnSpeed = 90f;
    [SerializeField] private bool invertGyroscope = false;

    [Header("Sensibilidad")]
    [SerializeField] private float maxTiltAngle = 45f;
    [SerializeField] private bool invertVertical = false;

    [Header("Filtro de temblor")]
    [SerializeField] private float filterTime = 0.28f;
    [SerializeField] private float deadZone = 0.14f;
    [SerializeField] private float responseCurve = 1.5f;

    [Header("Detección de intención")]
    [SerializeField] private float activationThreshold = 0.25f;
    [SerializeField] private float releaseThreshold = 0.16f;
    [SerializeField] private float persistenceTime = 0.25f;

    [Header("Calibración inicial")]
    [SerializeField] private float calibrationCountdown = 3f;
    [SerializeField] private TextMeshProUGUI calibrationText;

    private Rigidbody rb;
    private bool canMove;
    private bool hasGyroscope;
    private Coroutine calibrationCoroutine;

    private Vector2 calibrationAngles;
    private Vector2 rawInput;
    private Vector2 filteredInput;
    private Vector2 stableInput;
    private Vector2 currentVelocity;

    private float verticalTimer;
    private float turnInput;

    private bool verticalActive;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        hasGyroscope = SystemInfo.supportsGyroscope;

        if (hasGyroscope)
        {
            Input.gyro.enabled = true;
        }
        else
        {
            Debug.LogWarning(
                "Este dispositivo no tiene giroscopio. " +
                "Se usará el acelerómetro para girar."
            );
        }
    }

    private void Start()
    {
        canMove = false;
        StartCalibration();

        Debug.Log("Controlador accesible iniciado.");
    }

    private void Update()
    {
        if (!canMove)
            return;

        ReadAccelerometer();
        FilterInput();
        DetectIntent();
        CalculateMovement();
    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        MovePlayer();
    }

    public void StartCalibration()
    {
        if (calibrationCoroutine != null)
            return;

        canMove = false;
        calibrationCoroutine = StartCoroutine(CalibrateBeforeGame());
    }

    private IEnumerator CalibrateBeforeGame()
    {
        ResetMovementState();

        int remainingSeconds = Mathf.CeilToInt(calibrationCountdown);

        while (remainingSeconds > 0)
        {
            SetCalibrationText("Mantén el dispositivo quieto\n" + remainingSeconds);
            yield return new WaitForSeconds(1f);
            remainingSeconds--;
        }

        Recalibrate();
        canMove = true;
        SetCalibrationText("¡Comienza!");

        yield return new WaitForSeconds(1f);
        SetCalibrationText(string.Empty);
        calibrationCoroutine = null;
    }

    private void SetCalibrationText(string message)
    {
        if (calibrationText != null)
            calibrationText.text = message;
    }

    private void ReadAccelerometer()
    {
        Vector2 angles = GetAccelerometerAngles();

        float pitch = Mathf.DeltaAngle(
            calibrationAngles.x,
            angles.x
        );

        float roll = Mathf.DeltaAngle(
            calibrationAngles.y,
            angles.y
        );

        float vertical = Mathf.Clamp(
            pitch / maxTiltAngle, -1f, 1f
        );

        turnInput = Mathf.Clamp(
            Mathf.DeltaAngle(calibrationAngles.y, angles.y)
                / maxTiltAngle,
            -1f,
            1f
        );

        if (invertVertical)
            vertical *= -1f;

        rawInput = new Vector2(0f, vertical);
    }

    private Vector2 GetAccelerometerAngles()
    {
        Vector3 input = Input.acceleration;

        float pitch = Mathf.Atan2(
            -input.z,
            input.y
        ) * Mathf.Rad2Deg;

        float roll = Mathf.Atan2(
            input.x,
            input.y
        ) * Mathf.Rad2Deg;

        return new Vector2(pitch, roll);
    }

    private void FilterInput()
    {
        float alpha = 1f - Mathf.Exp(
            -Time.deltaTime / Mathf.Max(0.01f, filterTime)
        );

        filteredInput = Vector2.Lerp(
            filteredInput,
            rawInput,
            alpha
        );
    }

    private void DetectIntent()
    {
        verticalActive = DetectAxis(
            filteredInput.y,
            verticalActive,
            ref verticalTimer
        );
    }

    private bool DetectAxis(
        float input,
        bool active,
        ref float timer)
    {
        float magnitude = Mathf.Abs(input);

        if (active)
        {
            if (magnitude <= releaseThreshold)
            {
                timer = 0f;
                return false;
            }

            return true;
        }

        if (magnitude >= activationThreshold)
        {
            timer += Time.deltaTime;

            if (timer >= persistenceTime)
            {
                timer = 0f;
                return true;
            }
        }
        else
        {
            timer = 0f;
        }

        return false;
    }

    private void CalculateMovement()
    {
        float vertical = verticalActive
            ? ApplyResponseCurve(filteredInput.y)
            : 0f;

        stableInput = Vector2.ClampMagnitude(
            new Vector2(0f, vertical),
            1f
        );
    }

    private float ApplyResponseCurve(float value)
    {
        float magnitude = Mathf.Abs(value);

        float adjusted = Mathf.InverseLerp(
            deadZone,
            1f,
            magnitude
        );

        adjusted = Mathf.Pow(adjusted, responseCurve);

        return Mathf.Sign(value) * adjusted;
    }

    private void MovePlayer()
    {
        // La inclinación determina la velocidad deseada.
        Vector2 targetVelocity = stableInput * moveSpeed;

        // Si hay entrada, acelera suavemente.
        // Si no hay entrada, frena con mayor intensidad.
        float rate = stableInput.sqrMagnitude > 0.01f
            ? acceleration
            : deceleration;

        currentVelocity = Vector2.MoveTowards(
            currentVelocity,
            targetVelocity,
            rate * Time.fixedDeltaTime
        );

        Vector3 movement = new Vector3(
            0f,
            0f,
            currentVelocity.y
        );

        if (Mathf.Abs(currentVelocity.y) > 0.01f)
        {
            float turn = hasGyroscope
                ? Input.gyro.rotationRateUnbiased.y * turnSensitivity
                : turnInput * accelerometerTurnSpeed;

            if (invertGyroscope)
                turn *= -1f;

            Quaternion rotation = Quaternion.Euler(
                0f,
                turn * Time.fixedDeltaTime,
                0f
            );

            rb.MoveRotation(rb.rotation * rotation);
        }

        movement = rb.rotation * movement;

        rb.MovePosition(
            rb.position + movement * Time.fixedDeltaTime
        );
    }

    public void Recalibrate()
    {
        calibrationAngles = GetAccelerometerAngles();

        rawInput = Vector2.zero;
        filteredInput = Vector2.zero;
        stableInput = Vector2.zero;
        currentVelocity = Vector2.zero;
        turnInput = 0f;

        verticalTimer = 0f;

        verticalActive = false;

        Debug.Log("Acelerómetro recalibrado.");
    }

    public void ResetMovementState()
    {
        rawInput = Vector2.zero;
        filteredInput = Vector2.zero;
        stableInput = Vector2.zero;
        currentVelocity = Vector2.zero;
        turnInput = 0f;

        verticalTimer = 0f;

        verticalActive = false;
    }
}
