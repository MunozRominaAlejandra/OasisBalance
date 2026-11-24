using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditosController : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private RectTransform contenedorCreditos;
    [SerializeField] private GameObject panelGraciasPorJugar;
    [SerializeField] private Button botonReiniciar;
    [SerializeField] private Button botonSalir;

    [Header("Escena de destino")]
    [SerializeField] private string nombreEscenaMenu = "Menu";

    [Header("Configuración de desplazamiento")]
    [SerializeField] private float velocidadSubida = 60f;
    [SerializeField] private float posicionFinalY = 1200f;
    [SerializeField] private bool ocultarCreditosAlTerminar = true;

    private bool creditosTerminados = false;

    private void Start()
    {
        if (panelGraciasPorJugar != null)
            panelGraciasPorJugar.SetActive(false);

        if (botonReiniciar != null)
            botonReiniciar.onClick.AddListener(IrAlMenuPrincipal);

        if (botonSalir != null)
            botonSalir.onClick.AddListener(SalirJuego);
    }

    private void Update()
    {
        if (creditosTerminados || contenedorCreditos == null)
            return;

        Vector2 pos = contenedorCreditos.anchoredPosition;
        pos.y += velocidadSubida * Time.deltaTime;
        contenedorCreditos.anchoredPosition = pos;

        if (pos.y >= posicionFinalY)
        {
            TerminarCreditos();
        }
    }

    private void TerminarCreditos()
    {
        creditosTerminados = true;

        if (ocultarCreditosAlTerminar && contenedorCreditos != null)
            contenedorCreditos.gameObject.SetActive(false);

        if (panelGraciasPorJugar != null)
            panelGraciasPorJugar.SetActive(true);
    }

    public void IrAlMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscenaMenu);
    }

    public void SalirJuego()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}