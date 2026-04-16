using UnityEngine;
using System.Collections.Generic;
using Blackvers.Commons;

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
        public PlanetSize size;
        public int priority;
        public List<MineralData> minerals = new List<MineralData>();
    }
}
