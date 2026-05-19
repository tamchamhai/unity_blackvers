using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Blackvers.Data;

namespace Blackvers.UI.Inventory
{
    public class InventoryItemUI : MasterMonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] protected Image mineralIcon;
        [SerializeField] protected TextMeshProUGUI mineralNameText;
        [SerializeField] protected TextMeshProUGUI mineralAmountText;

        public virtual void UpdateUI(MineralData data, float amount)
        {
            if (data == null) return;

            if (this.mineralIcon != null && data.icon != null)
            {
                this.mineralIcon.sprite = data.icon;
            }

            if (this.mineralNameText != null)
            {
                this.mineralNameText.text = data.mineralName;
            }

            if (this.mineralAmountText != null)
            {
                // Format amount to 1 decimal place or whole number
                this.mineralAmountText.text = amount.ToString("F1");
            }
        }
    }
}
