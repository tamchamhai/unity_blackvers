using UnityEngine;

namespace Blackvers.Inventory
{
    public class MinerInventory : InventoryAbstract
    {
        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadCapacity();
        }

        protected virtual void LoadCapacity()
        {
            if (this.maxCapacity > 0) return;
            // Default capacity for miner
            this.maxCapacity = 100f;
        }
    }
}
