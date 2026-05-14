using UnityEngine;

namespace Blackvers.Data
{
    /// <summary>
    /// ScriptableObject data for Miner Ships.
    /// </summary>
    [CreateAssetMenu(fileName = "MinerShipData", menuName = "Blackvers/Ships/Miner Ship Data")]
    public class MinerShipData : ScriptableObject
    {
        public string shipName;
        public Sprite shipSprite;
        public float baseSpeed = 10f;
        public int levelRequired = 1;
        public float miningSpeed = 1f;
        public int capacity = 100;
    }
}
