using UnityEngine;
using System.Linq;
using SOTG.Mechanics.Intruder;

/// <summary>
/// Horizontal compass at the top of the screen that marks the direction
/// to the nearest egg-carrying intruder. Skyrim-style.
/// Uses the PLAYER's facing direction (not camera), with anchor-based
/// marker positioning for pixel-perfect alignment at any resolution.
/// </summary>
public class CompassController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform _compassBarRect;
    [SerializeField] private RectTransform _marker;

    [Header("Settings")]
    [SerializeField] private float _maxAngle = 90f;

    private Transform _player;

    private void Awake()
    {
        if (_compassBarRect == null)
            _compassBarRect = GetComponent<RectTransform>();

        if (_marker == null)
            _marker = transform.Find("CompassMarker")?.GetComponent<RectTransform>();
    }

    private void Start()
    {
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
            _player = playerGO.transform;
    }

    private void Update()
    {
        if (_player == null || _marker == null)
            return;

        // Find all intruders currently carrying an egg
        var kidnappers = Intruder.AllIntruders.Where(i => i.HasKidnapped).ToList();

        if (kidnappers.Count == 0)
        {
            if (_marker.gameObject.activeSelf)
                _marker.gameObject.SetActive(false);
            return;
        }

        if (!_marker.gameObject.activeSelf)
            _marker.gameObject.SetActive(true);

        // Closest egg-carrying intruder
        Vector3 playerPos = _player.position;
        Intruder closest = kidnappers
            .OrderBy(i => Vector3.Distance(playerPos, i.transform.position))
            .First();

        // Horizontal direction from player to target
        Vector3 toTarget = closest.transform.position - playerPos;
        toTarget.y = 0;

        // Use PLAYER forward for the compass direction
        Vector3 playerForward = _player.forward;
        playerForward.y = 0;

        // Signed angle: positive = right, negative = left
        float angle = Vector3.SignedAngle(playerForward.normalized, toTarget.normalized, Vector3.up);

        // Normalize angle to 0-1 for anchor-based positioning
        float t = Mathf.InverseLerp(-_maxAngle, _maxAngle, Mathf.Clamp(angle, -_maxAngle, _maxAngle));

        // Use anchor-based positioning (no pixel-width calculation needed)
        _marker.anchorMin = new Vector2(t, _marker.anchorMin.y);
        _marker.anchorMax = new Vector2(t, _marker.anchorMax.y);
    }
}
