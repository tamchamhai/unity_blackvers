using UnityEngine;

namespace Blackvers.Data
{
    /// <summary>
    /// ScriptableObject data for minerals (ores). Inherits from ItemData to support generic inventory displays.
    /// </summary>
    [CreateAssetMenu(fileName = "MineralData", menuName = "Blackvers/Mineral Data")]
    public class MineralData : ItemData
    {
        [Header("Legacy Mineral Data Fields")]
        public string mineralName;
        public Sprite icon;
        public string description;
        public float baseValue;

        protected virtual void OnEnable()
        {
            this.type = ItemType.Ore;
            
            // Sync values from legacy serialized fields if they are populated
            if (!string.IsNullOrEmpty(this.mineralName) && string.IsNullOrEmpty(this.itemName))
            {
                this.itemName = this.mineralName;
            }
            if (this.icon != null && this.itemIcon == null)
            {
                this.itemIcon = this.icon;
            }
            if (!string.IsNullOrEmpty(this.description) && string.IsNullOrEmpty(this.itemDescription))
            {
                this.itemDescription = this.description;
            }
            if (this.baseValue > 0 && this.value <= 0)
            {
                this.value = this.baseValue;
            }
        }
        
        // Also override properties to keep them in sync
        public override string ItemName
        {
            get => string.IsNullOrEmpty(this.mineralName) ? base.ItemName : this.mineralName;
            set
            {
                base.ItemName = value;
                this.mineralName = value;
            }
        }

        public override Sprite Icon
        {
            get => this.icon == null ? base.Icon : this.icon;
            set
            {
                base.Icon = value;
                this.icon = value;
            }
        }

        public override string Description
        {
            get => string.IsNullOrEmpty(this.description) ? base.Description : this.description;
            set
            {
                base.Description = value;
                this.description = value;
            }
        }

        public override float BaseValue
        {
            get => this.baseValue <= 0 ? base.BaseValue : this.baseValue;
            set
            {
                base.BaseValue = value;
                this.baseValue = value;
            }
        }
    }
}
