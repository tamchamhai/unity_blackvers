using UnityEngine;

namespace Blackvers.Data
{
    /// <summary>
    /// ScriptableObject data for minerals.
    /// </summary>
    [CreateAssetMenu(fileName = "MineralData", menuName = "Blackvers/Mineral Data")]
    public class MineralData : ScriptableObject
    {
        public string mineralName;
        public Sprite icon;
        public string description;
        public float baseValue;
    }
}
