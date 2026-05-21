using UnityEngine;
using System.Collections.Generic;
using Blackvers.Inventory;
using TMPro;

namespace Blackvers.UI.Inventory
{
    /// <summary>
    /// Manages the UI display of the MotherShip's inventory, including item list,
    /// capacity text, and open/close state. Uses a CanvasGroup to fully disable
    /// raycasting when closed, preventing invisible panels from blocking world input.
    /// </summary>
    public class MotherShipInventoryUI : MasterMonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected GameObject mainPanel;
        [SerializeField] protected Transform contentContainer;
        [SerializeField] protected GameObject itemPrefab;
        [SerializeField] protected TextMeshProUGUI capacityText;

        [Header("Runtime")]
        [SerializeField] protected List<InventoryItemUI> activeItemUIs = new List<InventoryItemUI>();

        // Controls whether the entire inventory area blocks raycasts (input).
        // Setting blocksRaycasts = false when closed prevents the invisible parent
        // container from intercepting touches and blocking camera pan/zoom.
        private CanvasGroup _canvasGroup;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadCanvasGroup();
            this.LoadMainPanel();
            this.LoadContentContainer();
            this.LoadCapacityText();
            this.LoadItemPrefab();
        }

        protected virtual void LoadCanvasGroup()
        {
            this._canvasGroup = this.GetComponent<CanvasGroup>();
            if (this._canvasGroup == null)
            {
                this._canvasGroup = this.gameObject.AddComponent<CanvasGroup>();
            }
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
            if (container == null) return;

            Transform scrollView = container.Find("Scroll View");
            if (scrollView == null) return;

            Transform viewport = scrollView.Find("Viewport");
            this.contentContainer = viewport != null
                ? viewport.Find("Content")
                : scrollView.Find("Content"); // Fallback if Viewport was removed
        }

        protected virtual void LoadCapacityText()
        {
            if (this.capacityText != null) return;
            Transform container = this.transform.Find("Container");
            if (container == null) return;

            Transform capacity = container.Find("Capacity");
            if (capacity != null)
            {
                this.capacityText = capacity.GetComponent<TextMeshProUGUI>();
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
            this.RefreshUI();
            this.CloseUI();
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
            if (this.mainPanel == null) return;

            bool isOpening = !this.mainPanel.activeSelf;
            this.SetInventoryVisible(isOpening);
            if (isOpening) this.RefreshUI();
        }

        public virtual void CloseUI()
        {
            this.SetInventoryVisible(false);
        }

        /// <summary>
        /// Sets the visibility and raycast state of the inventory panel.
        /// When hidden, raycasting is disabled on the CanvasGroup so that the invisible
        /// parent container does not intercept world-space touch inputs (camera pan/zoom).
        /// </summary>
        protected virtual void SetInventoryVisible(bool isVisible)
        {
            if (this.mainPanel != null)
            {
                this.mainPanel.SetActive(isVisible);
            }

            if (this._canvasGroup != null)
            {
                this._canvasGroup.blocksRaycasts = isVisible;
                this._canvasGroup.interactable   = isVisible;
            }
        }

        protected virtual void ClearUI()
        {
            foreach (InventoryItemUI itemUI in this.activeItemUIs)
            {
                if (itemUI != null) Destroy(itemUI.gameObject);
            }
            this.activeItemUIs.Clear();
        }

        public virtual void RefreshUI()
        {
            this.ClearUI();

            if (MotherShipController.Instance == null || MotherShipController.Instance.Inventory == null) return;

            MotherShipInventory inventory = MotherShipController.Instance.Inventory;

            foreach (InventoryItem item in inventory.GetAllItems())
            {
                if (item.mineralData == null || item.amount <= 0) continue;

                GameObject newObj = Instantiate(this.itemPrefab, this.contentContainer);
                newObj.SetActive(true);

                InventoryItemUI itemUI = newObj.GetComponent<InventoryItemUI>();
                if (itemUI == null) continue;

                itemUI.UpdateUI(item.mineralData, item.amount);
                this.activeItemUIs.Add(itemUI);
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
