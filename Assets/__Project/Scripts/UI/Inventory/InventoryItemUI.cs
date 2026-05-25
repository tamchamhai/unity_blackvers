using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Blackvers.Data;

namespace Blackvers.UI.Inventory
{
    /// <summary>
    /// Displays a single item (Ore, Bar, or Crafted Item) in the inventory list.
    /// </summary>
    public class InventoryItemUI : MasterMonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] protected Image mineralIcon;
        [SerializeField] protected TextMeshProUGUI mineralNameText;
        [SerializeField] protected TextMeshProUGUI mineralAmountText;

        public virtual void UpdateUI(ItemData data, float amount)
        {
            if (data == null) return;

            if (this.mineralIcon != null && data.Icon != null)
            {
                this.mineralIcon.sprite = data.Icon;
            }

            if (this.mineralNameText != null)
            {
                this.mineralNameText.text = data.ItemName;
            }

            if (this.mineralAmountText != null)
            {
                // Format amount to 1 decimal place or whole number
                this.mineralAmountText.text = amount.ToString("F1");
            }
        }
    }
}
