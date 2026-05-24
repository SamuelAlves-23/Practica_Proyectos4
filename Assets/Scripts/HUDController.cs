using UnityEngine;
using TMPro;
using SOTG.Mechanics.Game;

public class HUDController : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text criaturasLabel;
    public TMP_Text criaturasValue;
    public TMP_Text intrusosLabel;
    public TMP_Text intrusosValue;

    private GameManager _gameManager;

    private void Awake()
    {
        if (criaturasLabel == null || criaturasValue == null || intrusosLabel == null || intrusosValue == null)
        {
            FindUICanvasInChildren();
        }

        if (criaturasValue == null || intrusosValue == null)
        {
            CreateHUDCanvas();
        }
    }

    private void FindUICanvasInChildren()
    {
        Transform uiTransform = transform.Find("UI");
        if (uiTransform == null) return;

        Transform criaturasLabelT = uiTransform.Find("Criaturas");
        if (criaturasLabelT != null) criaturasLabel = criaturasLabelT.GetComponent<TMP_Text>();

        Transform criaturasValueT = uiTransform.Find("Criaturas (1)");
        if (criaturasValueT != null) criaturasValue = criaturasValueT.GetComponent<TMP_Text>();

        Transform intrusosLabelT = uiTransform.Find("Intrusos");
        if (intrusosLabelT != null) intrusosLabel = intrusosLabelT.GetComponent<TMP_Text>();

        Transform intrusosValueT = uiTransform.Find("Intrusos (1)");
        if (intrusosValueT != null) intrusosValue = intrusosValueT.GetComponent<TMP_Text>();
    }

    private void Start()
    {
        _gameManager = FindFirstObjectByType<GameManager>();

        if (_gameManager != null)
        {
            _gameManager.OnEggsChanged += UpdateEggCount;
            _gameManager.OnIntrudersChanged += UpdateIntruderCount;

            UpdateEggCount(_gameManager.EggsRemaining);
            UpdateIntruderCount(_gameManager.IntrudersRemaining);
        }
    }

    private void CreateHUDCanvas()
    {
        GameObject canvasGO = new GameObject("HUD Canvas", typeof(Canvas), typeof(UnityEngine.UI.CanvasScaler), typeof(UnityEngine.UI.GraphicRaycaster));
        canvasGO.transform.SetParent(transform, false);

        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        UnityEngine.UI.CanvasScaler scaler = canvasGO.GetComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        GameObject criaturasGO = CreateText(canvasGO, "Criaturas", "Criaturas: 0", new Vector2(-750, 480));
        criaturasValue = criaturasGO.GetComponent<TMP_Text>();

        GameObject intrusosGO = CreateText(canvasGO, "Intrusos", "Intrusos: 0", new Vector2(750, 480));
        intrusosValue = intrusosGO.GetComponent<TMP_Text>();
    }

    private GameObject CreateText(GameObject parent, string name, string text, Vector2 anchoredPosition)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent.transform, false);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(300, 60);
        rt.anchoredPosition = anchoredPosition;

        TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 36;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;
        tmp.outlineWidth = 0.2f;
        tmp.outlineColor = Color.black;

        return go;
    }

    private void OnDestroy()
    {
        if (_gameManager != null)
        {
            _gameManager.OnEggsChanged -= UpdateEggCount;
            _gameManager.OnIntrudersChanged -= UpdateIntruderCount;
        }
    }

    private void UpdateEggCount(int count)
    {
        if (criaturasValue != null)
        {
            criaturasValue.text = count.ToString();
        }
    }

    private void UpdateIntruderCount(int count)
    {
        if (intrusosValue != null)
        {
            intrusosValue.text = count.ToString();
        }
    }
}
