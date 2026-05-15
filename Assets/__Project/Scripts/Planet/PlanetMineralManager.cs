using UnityEngine;
using System.Collections.Generic;
using Blackvers.Data;
using Blackvers.Inventory;

namespace Blackvers.Planet
{
    [System.Serializable]
    public class MineralRuntimeData
    {
        public MineralData mineralData;
        public float currentAmount;
        public float mineRate;

        public MineralRuntimeData(MineralData data, float rate)
        {
            this.mineralData = data;
            this.mineRate = rate;
            this.currentAmount = 0f;
        }
    }

    public class PlanetMineralManager : MasterMonoBehaviour
    {
        [Header("References")]
        public PlanetController planetController;

        [Header("Runtime Data")]
        [SerializeField] protected List<MineralRuntimeData> activeMinerals = new List<MineralRuntimeData>();

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadPlanetController();
        }

        protected virtual void LoadPlanetController()
        {
            if (this.planetController != null) return;
            this.planetController = this.transform.parent.GetComponent<PlanetController>();
        }

        public virtual void Initialize()
        {
            this.activeMinerals.Clear();
            if (this.planetController == null || this.planetController.planetData == null) return;

            foreach (var pMineral in this.planetController.planetData.minerals)
            {
                if (pMineral.mineralData == null) continue;
                this.activeMinerals.Add(new MineralRuntimeData(pMineral.mineralData, pMineral.mineRate));
            }
        }

        protected override void Update()
        {
            base.Update();
            this.GenerateMinerals();
        }

        protected virtual void GenerateMinerals()
        {
            float dt = Time.deltaTime;
            foreach (var minData in this.activeMinerals)
            {
                minData.currentAmount += minData.mineRate * dt;
            }
        }

        /// <summary>
        /// Miner calls this to collect minerals.
        /// Iterates through available minerals and fills the remaining capacity.
        /// Returns a list of collected minerals.
        /// </summary>
        public virtual List<InventoryItem> CollectMineral(float availableCapacity)
        {
            List<InventoryItem> collected = new List<InventoryItem>();
            float remainingCapacity = availableCapacity;

            if (remainingCapacity <= 0) return collected;

            // Take sequentially based on what's available
            foreach (var minData in this.activeMinerals)
            {
                if (remainingCapacity <= 0) break;
                if (minData.currentAmount <= 0) continue;

                float amountToTake = Mathf.Min(remainingCapacity, minData.currentAmount);
                minData.currentAmount -= amountToTake;
                remainingCapacity -= amountToTake;

                collected.Add(new InventoryItem(minData.mineralData, amountToTake));
            }

            return collected;
        }

        // TODO: Implement mechanism to upgrade mine rate
        public virtual void UpgradeMineRate()
        {
            // Placeholder for future upgrade logic
        }
    }
}
