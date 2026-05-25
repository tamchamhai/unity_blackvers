using UnityEngine;
using System;
using System.Collections.Generic;
using Blackvers.Data;
using System.Linq;

namespace Blackvers.Inventory
{
    /// <summary>
    /// Abstract base class for managing split inventories of Ores, Bars, and Crafted Items.
    /// Tracks separate capacity limits and individual full states for each item type.
    /// </summary>
    public abstract class InventoryAbstract : MasterMonoBehaviour
    {
        [Header("Inventory Limit Settings")]
        [Tooltip("Max capacity for Ores (quặng).")]
        [SerializeField] protected float maxOreCapacity = 1000f;
        
        [Tooltip("Max capacity for Metal Bars (thanh kim loại).")]
        [SerializeField] protected float maxBarCapacity = 500f;
        
        [Tooltip("Max capacity for Crafted Items (vật phẩm chế tạo).")]
        [SerializeField] protected float maxItemCapacity = 200f;

        [Header("Runtime Capacities")]
        [SerializeField] protected float currentOreCapacity;
        [SerializeField] protected float currentBarCapacity;
        [SerializeField] protected float currentItemCapacity;

        // Legacy compatibility fields
        [SerializeField] protected float maxCapacity;
        [SerializeField] protected float currentCapacity;

        [Header("Inventory Data")]
        [SerializeField] protected List<InventoryItem> items = new List<InventoryItem>();

        // Legacy compatibility properties
        public virtual float MaxCapacity => this.maxOreCapacity;
        public virtual float CurrentCapacity => this.currentOreCapacity;
        public virtual bool IsFull => this.IsInventoryFull(ItemType.Ore);
        
        public Action OnInventoryChanged;

        protected override void Start()
        {
            base.Start();
            this.CalculateCurrentCapacities();
        }

        /// <summary>
        /// Adds a specified amount of an item to the inventory.
        /// Returns the actual amount added (limited by the type's specific capacity).
        /// </summary>
        public virtual float AddItem(ItemData itemData, float amount)
        {
            if (itemData == null || amount <= 0) return 0f;
            if (this.IsInventoryFull(itemData.Type)) return 0f;

            float availableSpace = this.GetMaxCapacity(itemData.Type) - this.GetCurrentCapacity(itemData.Type);
            float amountToAdd = Mathf.Min(amount, availableSpace);

            InventoryItem item = this.items.FirstOrDefault(i => i.mineralData == itemData);
            if (item != null)
            {
                item.amount += amountToAdd;
            }
            else
            {
                this.items.Add(new InventoryItem(itemData, amountToAdd));
            }

            this.CalculateCurrentCapacities();
            this.OnInventoryChanged?.Invoke();
            return amountToAdd;
        }

        /// <summary>
        /// Legacy compatibility wrapper for adding items.
        /// </summary>
        public virtual float AddMineral(ItemData mineral, float amount)
        {
            return this.AddItem(mineral, amount);
        }

        /// <summary>
        /// Removes a specified amount of an item from the inventory.
        /// Returns the actual amount removed.
        /// </summary>
        public virtual float RemoveItem(ItemData itemData, float amount)
        {
            if (itemData == null || amount <= 0) return 0f;

            InventoryItem item = this.items.FirstOrDefault(i => i.mineralData == itemData);
            if (item == null) return 0f;

            float amountToRemove = Mathf.Min(amount, item.amount);
            item.amount -= amountToRemove;

            if (item.amount <= 0)
            {
                this.items.Remove(item);
            }

            this.CalculateCurrentCapacities();
            this.OnInventoryChanged?.Invoke();
            return amountToRemove;
        }

        /// <summary>
        /// Legacy compatibility wrapper for removing items.
        /// </summary>
        public virtual float RemoveMineral(ItemData mineral, float amount)
        {
            return this.RemoveItem(mineral, amount);
        }

        public virtual float GetAmount(ItemData itemData)
        {
            if (itemData == null) return 0f;
            InventoryItem item = this.items.FirstOrDefault(i => i.mineralData == itemData);
            return item != null ? item.amount : 0f;
        }

        public virtual float GetMaxCapacity(ItemType type)
        {
            switch (type)
            {
                case ItemType.Ore:  return this.maxOreCapacity;
                case ItemType.Bar:  return this.maxBarCapacity;
                case ItemType.Item: return this.maxItemCapacity;
                default:            return 0f;
            }
        }

        public virtual float GetCurrentCapacity(ItemType type)
        {
            switch (type)
            {
                case ItemType.Ore:  return this.currentOreCapacity;
                case ItemType.Bar:  return this.currentBarCapacity;
                case ItemType.Item: return this.currentItemCapacity;
                default:            return 0f;
            }
        }

        public virtual bool IsInventoryFull(ItemType type)
        {
            return this.GetCurrentCapacity(type) >= this.GetMaxCapacity(type);
        }

        public virtual void Clear()
        {
            this.items.Clear();
            this.currentOreCapacity = 0f;
            this.currentBarCapacity = 0f;
            this.currentItemCapacity = 0f;
            this.currentCapacity = 0f;
            this.OnInventoryChanged?.Invoke();
        }

        public virtual List<InventoryItem> GetAllItems()
        {
            return new List<InventoryItem>(this.items);
        }

        protected virtual void CalculateCurrentCapacities()
        {
            this.currentOreCapacity = 0f;
            this.currentBarCapacity = 0f;
            this.currentItemCapacity = 0f;

            foreach (InventoryItem item in this.items)
            {
                if (item.mineralData == null) continue;
                switch (item.mineralData.Type)
                {
                    case ItemType.Ore:
                        this.currentOreCapacity += item.amount;
                        break;
                    case ItemType.Bar:
                        this.currentBarCapacity += item.amount;
                        break;
                    case ItemType.Item:
                        this.currentItemCapacity += item.amount;
                        break;
                }
            }

            this.currentCapacity = this.currentOreCapacity; // legacy sync
            this.maxCapacity = this.maxOreCapacity;         // legacy sync
        }

        protected virtual void CalculateCurrentCapacity()
        {
            this.CalculateCurrentCapacities();
        }
    }
}
