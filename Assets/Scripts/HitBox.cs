using UnityEngine;
using SOTG.Mechanics.Intruder;

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
