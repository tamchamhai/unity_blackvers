using UnityEngine;
using System.Collections.Generic;

namespace Blackvers.Data
{
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
        public List<MineralData> minerals = new List<MineralData>();
    }
}
