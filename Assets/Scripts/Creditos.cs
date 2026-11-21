using UnityEngine;
using UnityEngine.SceneManagement;

public class Creditos : MonoBehaviour
{
    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Nivel1");
    }

    public void SalirDelJuego()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
