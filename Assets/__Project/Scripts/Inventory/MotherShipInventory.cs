using UnityEngine;

namespace Blackvers.Inventory
{
    public class MotherShipInventory : InventoryAbstract
    {
        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadCapacity();
        }

        protected virtual void LoadCapacity()
        {
            if (this.maxCapacity > 0) return;
            // Default capacity for mother ship
            this.maxCapacity = 1000000f; 
        }

        // TODO: Implement mechanism to increase mother ship capacity
        public virtual void UpgradeCapacity(float additionalCapacity)
        {
            this.maxCapacity += additionalCapacity;
        }
    }
}
