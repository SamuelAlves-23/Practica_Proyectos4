using UnityEngine;
using SOTG.Mechanics.Intruder;

/// <summary>
/// Attached to the player's attack HitBox.
/// Activated/deactivated by animation events (AttackAnimEvent / UnAttackAnimEvent).
/// Detects intruders when active and kills them on contact.
/// </summary>
public class HitBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Intruder intruder = other.GetComponentInParent<Intruder>();
        if (intruder != null)
        {
            intruder.Kill();
        }
    }
}
