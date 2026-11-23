using UnityEngine;
using TMPro;

public class EstadisticasManager : MonoBehaviour
{
    public static EstadisticasManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI estadisticasText;

    private int intentos;
    private int ultimoCheckpoint = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        ActualizarTexto();
    }

    public void RegistrarIntento()
    {
        intentos++;
        ActualizarTexto();
    }

    public void RegistrarCheckpoint(int index)
    {
        if (index <= ultimoCheckpoint)
            return;

        ultimoCheckpoint = index;
        ActualizarTexto();
    }

    public void ActualizarTexto()
    {
        if (estadisticasText == null)
            return;

        string checkpoint = ultimoCheckpoint < 0
            ? "Inicio"
            : (ultimoCheckpoint + 1).ToString();

        estadisticasText.text =
            "Intentos: " + intentos +
            "\nÚltimo checkpoint: " + checkpoint;
    }
}
