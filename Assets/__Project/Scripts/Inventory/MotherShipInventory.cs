using UnityEngine;

namespace Blackvers.Inventory
{
    /// <summary>
    /// Inventory system specifically for the MotherShip.
    /// Manages very large capacities for Ores, Bars, and Crafted Items.
    /// </summary>
    public class MotherShipInventory : InventoryAbstract
    {
        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadCapacity();
        }

        protected virtual void LoadCapacity()
        {
            // Set large default capacities for MotherShip if they are not already set
            if (this.maxOreCapacity <= 1000f)
            {
                this.maxOreCapacity = 1000000f;
            }
            if (this.maxBarCapacity <= 500f)
            {
                this.maxBarCapacity = 1000000f;
            }
            if (this.maxItemCapacity <= 200f)
            {
                this.maxItemCapacity = 1000000f;
            }
            
            this.maxCapacity = this.maxOreCapacity; // legacy compatibility
        }

        /// <summary>
        /// Upgrades the capacity limits of all three inventories on the MotherShip.
        /// </summary>
        public virtual void UpgradeCapacity(float additionalCapacity)
        {
            this.maxOreCapacity += additionalCapacity;
            this.maxBarCapacity += additionalCapacity;
            this.maxItemCapacity += additionalCapacity;
            this.maxCapacity = this.maxOreCapacity;
            
            this.OnInventoryChanged?.Invoke();
        }
    }
}
