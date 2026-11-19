using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject menuPrincipal;
    [SerializeField] private GameObject menuOpciones;
    [SerializeField] private GameObject menuInstrucciones;

    [Header("Control de volumen")]
    [SerializeField] private Slider sliderVolumen;

    private void Start()
    {
        // Mostrar el menú principal al iniciar
        MenuPrincipal();

        // Configurar volumen
        sliderVolumen.minValue = 0f;
        sliderVolumen.maxValue = 1f;

        // Recuperar el volumen guardado
        float volumenGuardado = PlayerPrefs.GetFloat(
            "VolumenMaster", AudioListener.volume
        );

        AudioListener.volume = volumenGuardado;
        sliderVolumen.SetValueWithoutNotify(volumenGuardado);

        // Escuchar cambios del slider
        sliderVolumen.onValueChanged.AddListener(CambiarVolumen);
    }
    #region BOTONES Y PANELES DE NAVEGACIÓN
    // PANEL PRINCIPAL
    public void MenuPrincipal()
    {
        menuPrincipal.SetActive(true);
        menuOpciones.SetActive(false);
        menuInstrucciones.SetActive(false);
    }

    // PANEL DE OPCIONES
    public void MenuOpciones()
    {
        menuPrincipal.SetActive(false);
        menuOpciones.SetActive(true);
        menuInstrucciones.SetActive(false);
    }

    // PANEL DE INSTRUCCIONES
    public void Instrucciones()
    {
        menuPrincipal.SetActive(false);
        menuOpciones.SetActive(false);
        menuInstrucciones.SetActive(true);
    }

    // BOTÓN JUGAR
    public void Jugar()
    {
        SceneManager.LoadScene("Nivel1");
    }
    #endregion

    #region CONTROL DE VOLUMEN
    public void CambiarVolumen(float valor)
    {
        AudioListener.volume = valor;

        PlayerPrefs.SetFloat("VolumenMaster", valor);
        PlayerPrefs.Save();
    }
    #endregion
}
