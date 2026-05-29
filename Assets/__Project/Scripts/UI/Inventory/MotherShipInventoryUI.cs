using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Blackvers.Inventory;
using Blackvers.Data;
using TMPro;

namespace Blackvers.UI.Inventory
{
    /// <summary>
    /// Manages the UI display of the MotherShip's inventory, with a tab system
    /// separating Ores, Metal Bars, and Crafted Items.
    /// Tracks separate capacity limits and individual full states for each panel.
    /// Uses a CanvasGroup to fully disable raycasting when closed.
    /// This is a partial class; component loading is in MotherShipInventoryUIComponents.cs,
    /// and tab/pooling/refresh logic is in MotherShipInventoryUIController.cs.
    /// </summary>
    public partial class MotherShipInventoryUI : MasterMonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected GameObject mainPanel;
        [SerializeField] protected Transform contentContainer;
        [SerializeField] protected GameObject itemPrefab;
        [SerializeField] protected TextMeshProUGUI capacityText;

        [Header("Navigation & Sidebar")]
        [SerializeField] protected TopNavigationController topNavigation;
        [SerializeField] protected LeftSidebarController leftSidebar;

        [Header("Runtime")]
        [SerializeField] protected List<InventoryItemUI> activeItemUIs = new List<InventoryItemUI>();

        [Header("Object Pooling")]
        [SerializeField] protected List<InventoryItemUI> itemUIPool = new List<InventoryItemUI>();

        // Controls whether the entire inventory area blocks raycasts (input).
        private CanvasGroup _canvasGroup;

        // Flag to reset vertical scroll position to 0 on open/close or tab switches
        protected bool _shouldResetScrollY = false;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadCanvasGroup();
            this.LoadMainPanel();
            this.LoadContentContainer();
            this.LoadCapacityText();
            this.LoadItemPrefab();
            this.LoadTopNavigation();
            this.LoadLeftSidebar();
        }

        protected override void Start()
        {
            base.Start();
            this.InitializeTabSystem();
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
            if (this.topNavigation != null)
            {
                this.topNavigation.OnTabSelected -= this.HandleMainTabSelected;
            }

            if (this.leftSidebar != null)
            {
                this.leftSidebar.OnSubTabSelected -= this.HandleSubTabSelected;
            }

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
            if (isOpening)
            {
                this._shouldResetScrollY = true;
                this.RefreshUI();
            }
        }

        public virtual void CloseUI()
        {
            this.SetInventoryVisible(false);
        }

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

            if (!isVisible)
            {
                this._shouldResetScrollY = true;

                // Reset ScrollRect position to 1f (top) physically when closing
                if (this.contentContainer != null)
                {
                    ScrollRect scrollRect = this.contentContainer.GetComponentInParent<ScrollRect>();
                    if (scrollRect != null)
                    {
                        scrollRect.verticalNormalizedPosition = 1f;
                    }
                }
            }
        }
    }
}
