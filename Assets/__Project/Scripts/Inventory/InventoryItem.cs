using UnityEngine;
using Blackvers.Data;
using System;

namespace Blackvers.Inventory
{
    [Serializable]
    public class InventoryItem
    {
        public MineralData mineralData;
        public float amount;

        public InventoryItem(MineralData mineralData, float amount)
        {
            this.mineralData = mineralData;
            this.amount = amount;
        }
    }
}
