using UnityEngine;
using TMPro;
using SOTG.Mechanics.Game;
using SOTG.Mechanics.Intruder;

public class HUDController : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text criaturas;
    public TMP_Text intrusos;

    [Header("Game References")]
    public GameManager gameManager;
    public IntruderSpawner intruderSpawner;

    private void Start()
    {
        // Subscribe to game events
        if (gameManager != null)
        {
            gameManager.OnEggsChanged += UpdateEggCount;
        }

        UpdateEggCount(0);
        UpdateIntruderCount(0);
    }

    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.OnEggsChanged -= UpdateEggCount;
        }
    }

    private void Update()
    {
        // Update intruder count in real-time
        if (intruderSpawner != null)
        {
            UpdateIntruderCount(intruderSpawner.SpawnedIntruders.Count);
        }
    }

    private void UpdateEggCount(int count)
    {
        if (criaturas != null)
        {
            criaturas.text = "Huevos: " + count;
        }
    }

    private void UpdateIntruderCount(int count)
    {
        if (intrusos != null)
        {
            intrusos.text = "Intrusos: " + count;
        }
    }
}
