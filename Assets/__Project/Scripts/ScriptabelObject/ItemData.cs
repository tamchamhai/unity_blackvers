using UnityEngine;

namespace Blackvers.Data
{
    /// <summary>
    /// Type of items in the game.
    /// </summary>
    public enum ItemType
    {
        Ore,
        Bar,
        Item
    }

    /// <summary>
    /// Abstract base ScriptableObject for all types of items (Ores, Metal Bars, Crafted Items).
    /// </summary>
    [CreateAssetMenu(fileName = "ItemData", menuName = "Blackvers/Item Data")]
    public class ItemData : ScriptableObject
    {
        [Header("General Settings")]
        [Tooltip("The readable name of the item.")]
        [SerializeField] protected string itemName;
        
        [Tooltip("The visual icon representing the item.")]
        [SerializeField] protected Sprite itemIcon;
        
        [Tooltip("A brief description of the item.")]
        [SerializeField] protected string itemDescription;
        
        [Tooltip("The value of the item.")]
        [SerializeField] protected float value;

        [Tooltip("The category/type of this item.")]
        [SerializeField] protected ItemType type = ItemType.Ore;

        public virtual string ItemName
        {
            get => this.itemName;
            set => this.itemName = value;
        }

        public virtual Sprite Icon
        {
            get => this.itemIcon;
            set => this.itemIcon = value;
        }

        public virtual string Description
        {
            get => this.itemDescription;
            set => this.itemDescription = value;
        }

        public virtual float BaseValue
        {
            get => this.value;
            set => this.value = value;
        }

        public virtual ItemType Type
        {
            get => this.type;
            set => this.type = value;
        }
    }
}
