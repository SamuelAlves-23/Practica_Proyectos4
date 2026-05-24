using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverController : MonoBehaviour
{
    [Header("UI References (optional — auto-found by name)")]
    public TMP_Text resultadoText;
    public TMP_Text huevosText;
    public TMP_Text intrusosText;

    private void Awake()
    {
        if (resultadoText == null)
            resultadoText = transform.Find("ResultadoText")?.GetComponent<TMP_Text>();
        if (huevosText == null)
            huevosText = transform.Find("ButtonContainer/EggLabel")?.GetComponent<TMP_Text>();
        if (intrusosText == null)
            intrusosText = transform.Find("ButtonContainer/IntruderLabel")?.GetComponent<TMP_Text>();
    }

    private void Start()
    {
        bool won = PlayerPrefs.GetInt("SOTG_LastResult_Won", 0) == 1;
        int eggsRemaining = PlayerPrefs.GetInt("SOTG_LastResult_EggsRemaining", 0);
        int initialEggs = PlayerPrefs.GetInt("SOTG_LastResult_InitialEggs", 0);
        int intrudersKilled = PlayerPrefs.GetInt("SOTG_LastResult_IntrudersKilled", 0);

        if (resultadoText != null)
        {
            resultadoText.text = "RESUMEN";
            resultadoText.color = Color.white;
        }

        if (huevosText != null)
            huevosText.text = $"Huevos restantes: {eggsRemaining}/{initialEggs}";

        if (intrusosText != null)
            intrusosText.text = $"Intrusos eliminados: {intrudersKilled}";
    }

    public void Reintentar()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void MenuPrincipal()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void Salir()
    {
        Application.Quit();
    }
}
