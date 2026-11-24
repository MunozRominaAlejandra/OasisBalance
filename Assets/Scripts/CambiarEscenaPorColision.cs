using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscenaPorColision : MonoBehaviour
{
    [Header("Configuración de escena")]
    [SerializeField] private string nombreSiguienteEscena = "";
    [SerializeField] private bool usarSiguientePorBuildIndex = true;
    [SerializeField] private float retrasoSegundos = 0.5f;

    [Header("Filtro del jugador")]
    [SerializeField] private string tagJugador = "Player";

    private bool transicionIniciada = false;

    private void OnTriggerEnter(Collider other)
    {
        if (transicionIniciada)
            return;

        if (!other.CompareTag(tagJugador))
            return;

        transicionIniciada = true;
        StartCoroutine(CargarEscenaConRetraso());
    }

    private IEnumerator CargarEscenaConRetraso()
    {
        yield return new WaitForSeconds(retrasoSegundos);

        Time.timeScale = 1f;

        if (usarSiguientePorBuildIndex)
        {
            int siguienteIndice = SceneManager.GetActiveScene().buildIndex + 1;
            if (siguienteIndice < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(siguienteIndice);
            }
            else
            {
                Debug.LogWarning("No hay más escenas en Build Settings después del índice actual.");
            }
        }
        else if (!string.IsNullOrWhiteSpace(nombreSiguienteEscena))
        {
            SceneManager.LoadScene(nombreSiguienteEscena);
        }
        else
        {
            Debug.LogWarning("Indicá un nombre de escena válido o activá 'Usar Siguiente Por Build Index'.");
        }
    }
}