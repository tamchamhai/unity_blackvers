using UnityEngine;
using System;
using System.Collections.Generic;
using Blackvers.Data;
using System.Linq;

namespace Blackvers.Inventory
{
    /// <summary>
    /// Abstract base class for managing an inventory of minerals.
    /// </summary>
    public abstract class InventoryAbstract : MasterMonoBehaviour
    {
        [Header("Inventory Settings")]
        [SerializeField] protected float maxCapacity;
        [SerializeField] protected float currentCapacity;

        [Header("Inventory Data")]
        [SerializeField] protected List<InventoryItem> items = new List<InventoryItem>();

        public virtual float MaxCapacity => this.maxCapacity;
        public virtual float CurrentCapacity => this.currentCapacity;
        public virtual bool IsFull => this.currentCapacity >= this.maxCapacity;
        public Action OnInventoryChanged;

        protected override void Start()
        {
            base.Start();
            this.CalculateCurrentCapacity();
        }

        /// <summary>
        /// Adds a specified amount of a mineral to the inventory.
        /// Returns the actual amount added (limited by capacity).
        /// </summary>
        public virtual float AddMineral(MineralData mineral, float amount)
        {
            if (mineral == null || amount <= 0) return 0f;
            if (this.IsFull) return 0f;

            float availableSpace = this.maxCapacity - this.currentCapacity;
            float amountToAdd = Mathf.Min(amount, availableSpace);

            InventoryItem item = this.items.FirstOrDefault(i => i.mineralData == mineral);
            if (item != null)
            {
                item.amount += amountToAdd;
            }
            else
            {
                this.items.Add(new InventoryItem(mineral, amountToAdd));
            }

            this.currentCapacity += amountToAdd;
            this.OnInventoryChanged?.Invoke();
            return amountToAdd;
        }

        /// <summary>
        /// Removes a specified amount of a mineral from the inventory.
        /// Returns the actual amount removed.
        /// </summary>
        public virtual float RemoveMineral(MineralData mineral, float amount)
        {
            if (mineral == null || amount <= 0) return 0f;

            InventoryItem item = this.items.FirstOrDefault(i => i.mineralData == mineral);
            if (item == null) return 0f;

            float amountToRemove = Mathf.Min(amount, item.amount);
            item.amount -= amountToRemove;
            this.currentCapacity -= amountToRemove;

            if (item.amount <= 0)
            {
                this.items.Remove(item);
            }

            this.OnInventoryChanged?.Invoke();
            return amountToRemove;
        }

        public virtual float GetAmount(MineralData mineral)
        {
            if (mineral == null) return 0f;
            InventoryItem item = this.items.FirstOrDefault(i => i.mineralData == mineral);
            return item != null ? item.amount : 0f;
        }

        public virtual void Clear()
        {
            this.items.Clear();
            this.currentCapacity = 0f;
            this.OnInventoryChanged?.Invoke();
        }

        public virtual List<InventoryItem> GetAllItems()
        {
            return new List<InventoryItem>(this.items);
        }

        protected virtual void CalculateCurrentCapacity()
        {
            this.currentCapacity = 0f;
            foreach (InventoryItem item in this.items)
            {
                this.currentCapacity += item.amount;
            }
        }
    }
}
