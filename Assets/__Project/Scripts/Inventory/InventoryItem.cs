using UnityEngine;
using Blackvers.Data;
using System;

namespace Blackvers.Inventory
{
    /// <summary>
    /// Represents an item and its quantity in an inventory.
    /// Supports Ores, Bars, and Crafted Items via the ItemData base class.
    /// </summary>
    [Serializable]
    public class InventoryItem
    {
        [Tooltip("The reference to the item data (can be Ore, Bar, or Crafted Item).")]
        public ItemData mineralData;
        
        [Tooltip("The quantity of this item.")]
        public float amount;

        public InventoryItem(ItemData itemData, float amount)
        {
            this.mineralData = itemData;
            this.amount = amount;
        }
    }
}
