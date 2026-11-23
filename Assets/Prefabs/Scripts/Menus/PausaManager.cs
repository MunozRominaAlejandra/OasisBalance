using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausaManager : MonoBehaviour
{
    [Header("Panel de pausa")]
    [SerializeField] private GameObject panelPausa;

    [Header("Control de volumen")]
    [SerializeField] private Slider sliderVolumen;

    [Header("Estadísticas")]
    [SerializeField] private EstadisticasManager estadisticasManager;

    private void Start()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (panelPausa != null)
            panelPausa.SetActive(false);

        if (sliderVolumen != null)
        {
            sliderVolumen.minValue = 0f;
            sliderVolumen.maxValue = 1f;

            float volumenGuardado = PlayerPrefs.GetFloat(
                "VolumenMaster",
                AudioListener.volume
            );

            AudioListener.volume = volumenGuardado;
            sliderVolumen.SetValueWithoutNotify(volumenGuardado);
            sliderVolumen.onValueChanged.AddListener(CambiarVolumen);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            AlternarPausa();
    }

    public void AlternarPausa()
    {
        if (Time.timeScale > 0f)
            PausarJuego();
        else
            ReanudarJuego();
    }

    public void PausarJuego()
    {
        if (panelPausa != null)
            panelPausa.SetActive(true);

        if (estadisticasManager == null)
            estadisticasManager = FindAnyObjectByType<EstadisticasManager>();

        if (estadisticasManager != null)
            estadisticasManager.ActualizarTexto();

        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    public void ReanudarJuego()
    {
        if (panelPausa != null)
            panelPausa.SetActive(false);

        AudioListener.pause = false;
        Time.timeScale = 1f;
    }

    public void CambiarVolumen(float valor)
    {
        AudioListener.volume = valor;
        PlayerPrefs.SetFloat("VolumenMaster", valor);
        PlayerPrefs.Save();
    }

    public void VolverAlMenuPrincipal()
    {
        AudioListener.pause = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    private void OnDestroy()
    {
        AudioListener.pause = false;
        Time.timeScale = 1f;
    }
}
