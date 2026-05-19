using UnityEngine;
using System.Collections.Generic;
using Blackvers.Inventory;
using TMPro;

namespace Blackvers.UI.Inventory
{
    public class MotherShipInventoryUI : MasterMonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected GameObject mainPanel;
        [SerializeField] protected Transform contentContainer;
        [SerializeField] protected GameObject itemPrefab;
        [SerializeField] protected TextMeshProUGUI capacityText;

        [Header("Runtime")]
        [SerializeField] protected List<InventoryItemUI> activeItemUIs = new List<InventoryItemUI>();

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadMainPanel();
            this.LoadContentContainer();
            this.LoadCapacityText();
            this.LoadItemPrefab();
        }

        protected virtual void LoadMainPanel()
        {
            if (this.mainPanel != null) return;
            Transform container = this.transform.Find("Container");
            if (container != null)
            {
                this.mainPanel = container.gameObject;
            }
        }

        protected virtual void LoadContentContainer()
        {
            if (this.contentContainer != null) return;
            Transform container = this.transform.Find("Container");
            if (container != null)
            {
                Transform scrollView = container.Find("Scroll View");
                if (scrollView != null)
                {
                    Transform viewport = scrollView.Find("Viewport");
                    if (viewport != null)
                    {
                        this.contentContainer = viewport.Find("Content");
                    }
                    else 
                    {
                        // Sometime Viewport is missing if it was modified
                        this.contentContainer = scrollView.Find("Content");
                    }
                }
            }
        }

        protected virtual void LoadCapacityText()
        {
            if (this.capacityText != null) return;
            Transform container = this.transform.Find("Container");
            if (container != null)
            {
                Transform capacity = container.Find("Capacity");
                if (capacity != null)
                {
                    this.capacityText = capacity.GetComponent<TextMeshProUGUI>();
                }
            }
        }

        protected virtual void LoadItemPrefab()
        {
            if (this.itemPrefab != null) return;
#if UNITY_EDITOR
            string prefabPath = "Assets/__Project/Prefabs/UI/Inventory/MineralItem_UI.prefab";
            this.itemPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
#endif
        }

        protected override void Start()
        {
            base.Start();
            this.SubscribeToEvents();
            this.RefreshUI(); // Initial refresh
            this.CloseUI(); // Hide by default
        }

        protected virtual void OnDestroy()
        {
            this.UnsubscribeFromEvents();
        }

        protected virtual void SubscribeToEvents()
        {
            if (MotherShipController.Instance == null) return;
            
            if (MotherShipController.Instance.Inventory != null)
            {
                MotherShipController.Instance.Inventory.OnInventoryChanged += this.RefreshUI;
            }
            
            MotherShipController.Instance.OnMotherShipClicked += this.ToggleUI;
        }

        protected virtual void UnsubscribeFromEvents()
        {
            if (MotherShipController.Instance == null) return;
            
            if (MotherShipController.Instance.Inventory != null)
            {
                MotherShipController.Instance.Inventory.OnInventoryChanged -= this.RefreshUI;
            }
            
            MotherShipController.Instance.OnMotherShipClicked -= this.ToggleUI;
        }

        public virtual void ToggleUI()
        {
            if (this.mainPanel != null)
            {
                this.mainPanel.SetActive(!this.mainPanel.activeSelf);
                if (this.mainPanel.activeSelf) this.RefreshUI();
            }
        }

        public virtual void CloseUI()
        {
            if (this.mainPanel != null)
            {
                this.mainPanel.SetActive(false);
            }
        }

        protected virtual void ClearUI()
        {
            foreach (var itemUI in this.activeItemUIs)
            {
                if (itemUI != null)
                {
                    Destroy(itemUI.gameObject);
                }
            }
            this.activeItemUIs.Clear();
        }

        public virtual void RefreshUI()
        {
            this.ClearUI();

            if (MotherShipController.Instance == null || MotherShipController.Instance.Inventory == null) return;

            MotherShipInventory inventory = MotherShipController.Instance.Inventory;
            var items = inventory.GetAllItems();

            foreach (var item in items)
            {
                if (item.mineralData == null || item.amount <= 0) continue;

                GameObject newObj = Instantiate(this.itemPrefab, this.contentContainer);
                newObj.SetActive(true);
                
                InventoryItemUI itemUI = newObj.GetComponent<InventoryItemUI>();
                if (itemUI != null)
                {
                    itemUI.UpdateUI(item.mineralData, item.amount);
                    this.activeItemUIs.Add(itemUI);
                }
            }

            this.UpdateCapacityText(inventory);
        }

        protected virtual void UpdateCapacityText(MotherShipInventory inventory)
        {
            if (this.capacityText == null) return;
            this.capacityText.text = $"Capacity: {inventory.CurrentCapacity:F0} / {inventory.MaxCapacity:F0}";
        }
    }
}
