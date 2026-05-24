using UnityEngine;
using System.Collections.Generic;

namespace SOTG.Mechanics.Intruder
{
    public class Cave : MonoBehaviour
    {
        public static List<Cave> AllCaves { get; private set; } = new List<Cave>();

        private void OnEnable()
        {
            if (!AllCaves.Contains(this))
            {
                AllCaves.Add(this);
            }
        }

        private void OnDisable()
        {
            AllCaves.Remove(this);
        }

        private void OnDestroy()
        {
            AllCaves.Remove(this);
        }

        public static Transform GetNearestCave(Vector3 intruderPosition)
        {
            if (AllCaves.Count == 0) return null;

            Cave nearestCave = null;
            float nearestDistance = float.MaxValue;

            foreach (Cave cave in AllCaves)
            {
                if (cave == null) continue;

                float distance = Vector3.Distance(intruderPosition, cave.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestCave = cave;
                }
            }

            return nearestCave != null ? nearestCave.transform : null;
        }
    }
}