
using UnityEngine;

public class SensorDiagnostic : MonoBehaviour
{
    private float timer;

    void Start()
    {
        Input.gyro.enabled = true;

        Debug.Log("=== DIAGNÓSTICO DE SENSORES ===");
        Debug.Log("Plataforma: " + Application.platform);
        Debug.Log("Editor: " + Application.isEditor);
        Debug.Log("Giroscopio local: " +
                  SystemInfo.supportsGyroscope);
        Debug.Log("Acelerómetro local: " +
                  SystemInfo.supportsAccelerometer);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 0.5f)
        {
            timer = 0f;

            Debug.Log("ACELERÓMETRO: " +
                      Input.acceleration);

            Debug.Log("GIROSCOPIO: " +
                      Input.gyro.rotationRate);

            Debug.Log("ACTITUD: " +
                      Input.gyro.attitude);
        }
    }
}
