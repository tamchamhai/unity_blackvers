using UnityEngine;
using System.Collections.Generic;

namespace Blackvers.Data
{
    [System.Serializable]
    public class PlanetMineral
    {
        public MineralData mineralData;
        public float mineRate;
    }

    /// <summary>
    /// ScriptableObject data for planets.
    /// </summary>
    [CreateAssetMenu(fileName = "PlanetData", menuName = "Blackvers/Planet Data")]
    public class PlanetData : ScriptableObject
    {
        public string planetName;
        public Sprite planetSprite;
        public float radius = 0.5f;
        public int priority;
        public List<PlanetMineral> minerals = new List<PlanetMineral>();
    }
}
