using UnityEngine;
using UnityEngine.SceneManagement;

public class NivelManager : MonoBehaviour
{
    public static int CheckpointSeleccionado { get; private set; } = -1;

    public static void SeleccionarInicio()
    {
        CheckpointSeleccionado = -1;
    }

    public void CargarCheckpoint(int checkpointIndex)
    {
        if (checkpointIndex < 0 || checkpointIndex > 5)
            return;

        CheckpointSeleccionado = checkpointIndex;
        SceneManager.LoadScene("Nivel1");
    }
}
