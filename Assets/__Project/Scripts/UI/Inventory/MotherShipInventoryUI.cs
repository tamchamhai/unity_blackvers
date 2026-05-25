using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Blackvers.Inventory;
using Blackvers.Data;
using TMPro;

namespace Blackvers.UI.Inventory
{
    /// <summary>
    /// Type of inventory tabs.
    /// </summary>
    public enum InventoryTab
    {
        Ores,
        Bars,
        Items
    }

    /// <summary>
    /// Manages the UI display of the MotherShip's inventory, with a tab system
    /// separating Ores, Metal Bars, and Crafted Items.
    /// Tracks separate capacity limits and individual full states for each panel.
    /// Uses a CanvasGroup to fully disable raycasting when closed.
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

        [Header("Object Pooling")]
        [SerializeField] protected List<InventoryItemUI> itemUIPool = new List<InventoryItemUI>();

        [Header("Tab System Settings (Drag and Drop in Inspector)")]
        [Tooltip("Button for the Ores tab.")]
        [SerializeField] protected Button tabOresButton;
        
        [Tooltip("Button for the Bars tab.")]
        [SerializeField] protected Button tabBarsButton;
        
        [Tooltip("Button for the Items tab.")]
        [SerializeField] protected Button tabItemsButton;

        [Tooltip("TextMeshPro text for the Ores tab.")]
        [SerializeField] protected TextMeshProUGUI tabOresText;

        [Tooltip("TextMeshPro text for the Bars tab.")]
        [SerializeField] protected TextMeshProUGUI tabBarsText;

        [Tooltip("TextMeshPro text for the Items tab.")]
        [SerializeField] protected TextMeshProUGUI tabItemsText;

        [Header("Tab Visual Settings")]
        [SerializeField] protected Color activeTabColor = new Color(0.12f, 0.38f, 0.18f, 1f); // Sleek Emerald
        [SerializeField] protected Color inactiveTabColor = new Color(0.18f, 0.18f, 0.18f, 0.9f); // Dark Gray
        [SerializeField] protected Color activeTextColor = new Color(0.85f, 1f, 0.85f, 1f);
        [SerializeField] protected Color inactiveTextColor = new Color(0.7f, 0.7f, 0.7f, 1f);

        // Runtime dictionary references for easy state management
        protected Dictionary<InventoryTab, Button> tabButtons = new Dictionary<InventoryTab, Button>();
        protected Dictionary<InventoryTab, TextMeshProUGUI> tabTexts = new Dictionary<InventoryTab, TextMeshProUGUI>();
        private InventoryTab _currentTab = InventoryTab.Ores;

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
            this.LoadTabComponents();
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

        /// <summary>
        /// Dedicated component loading function for auto-populating tab elements if present in the hierarchy.
        /// Finds the Container/TabsContainer path and maps the 1st, 2nd, and 3rd children directly.
        /// Automatically attaches a Button component if it is missing from the GameObject!
        /// </summary>
        protected virtual void LoadTabComponents()
        {
            Transform container = this.transform.Find("Container");
            if (container == null) return;

            Transform tabsContainer = container.Find("TabsContainer");
            if (tabsContainer == null) return;

            // Map tabs by children index in hierarchy order (0 = Ores, 1 = Bars, 2 = Items)
            if (tabsContainer.childCount >= 1)
            {
                Transform child0 = tabsContainer.GetChild(0);
                if (child0 != null)
                {
                    this.tabOresButton = child0.GetComponent<Button>();
                    if (this.tabOresButton == null)
                    {
                        this.tabOresButton = child0.gameObject.AddComponent<Button>();
                    }
                    this.tabOresText = child0.GetComponentInChildren<TextMeshProUGUI>();
                }
            }

            if (tabsContainer.childCount >= 2)
            {
                Transform child1 = tabsContainer.GetChild(1);
                if (child1 != null)
                {
                    this.tabBarsButton = child1.GetComponent<Button>();
                    if (this.tabBarsButton == null)
                    {
                        this.tabBarsButton = child1.gameObject.AddComponent<Button>();
                    }
                    this.tabBarsText = child1.GetComponentInChildren<TextMeshProUGUI>();
                }
            }

            if (tabsContainer.childCount >= 3)
            {
                Transform child2 = tabsContainer.GetChild(2);
                if (child2 != null)
                {
                    this.tabItemsButton = child2.GetComponent<Button>();
                    if (this.tabItemsButton == null)
                    {
                        this.tabItemsButton = child2.gameObject.AddComponent<Button>();
                    }
                    this.tabItemsText = child2.GetComponentInChildren<TextMeshProUGUI>();
                }
            }
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
            if (MotherShipController.Instance == null) return;

            if (MotherShipController.Instance.Inventory != null)
            {
                MotherShipController.Instance.Inventory.OnInventoryChanged -= this.RefreshUI;
            }

            MotherShipController.Instance.OnMotherShipClicked -= this.ToggleUI;
        }

        /// <summary>
        /// Registers click listeners and builds internal references for the drag-and-drop tab components.
        /// </summary>
        protected virtual void InitializeTabSystem()
        {
            this.tabButtons.Clear();
            this.tabTexts.Clear();

            // Register Ores Tab
            if (this.tabOresButton != null)
            {
                this.tabOresButton.onClick.RemoveAllListeners();
                this.tabOresButton.onClick.AddListener(() => this.SwitchTab(InventoryTab.Ores));
                this.tabButtons.Add(InventoryTab.Ores, this.tabOresButton);
            }
            if (this.tabOresText != null)
            {
                this.tabTexts.Add(InventoryTab.Ores, this.tabOresText);
            }

            // Register Bars Tab
            if (this.tabBarsButton != null)
            {
                this.tabBarsButton.onClick.RemoveAllListeners();
                this.tabBarsButton.onClick.AddListener(() => this.SwitchTab(InventoryTab.Bars));
                this.tabButtons.Add(InventoryTab.Bars, this.tabBarsButton);
            }
            if (this.tabBarsText != null)
            {
                this.tabTexts.Add(InventoryTab.Bars, this.tabBarsText);
            }

            // Register Items Tab
            if (this.tabItemsButton != null)
            {
                this.tabItemsButton.onClick.RemoveAllListeners();
                this.tabItemsButton.onClick.AddListener(() => this.SwitchTab(InventoryTab.Items));
                this.tabButtons.Add(InventoryTab.Items, this.tabItemsButton);
            }
            if (this.tabItemsText != null)
            {
                this.tabTexts.Add(InventoryTab.Items, this.tabItemsText);
            }

            this.UpdateTabVisuals();
        }

        /// <summary>
        /// Switches the currently displayed inventory tab.
        /// </summary>
        public virtual void SwitchTab(InventoryTab newTab)
        {
            this._currentTab = newTab;
            this.UpdateTabVisuals();
            this._shouldResetScrollY = true;
            this.RefreshUI();
        }

        /// <summary>
        /// Highlights the active tab button using customizable visual variables from the Inspector.
        /// </summary>
        protected virtual void UpdateTabVisuals()
        {
            foreach (var kvp in this.tabButtons)
            {
                Image img = kvp.Value.GetComponent<Image>();
                TextMeshProUGUI txt = this.tabTexts.ContainsKey(kvp.Key) ? this.tabTexts[kvp.Key] : null;

                if (kvp.Key == this._currentTab)
                {
                    if (img != null) img.color = this.activeTabColor;
                    if (txt != null)
                    {
                        txt.fontStyle = FontStyles.Bold;
                        txt.color = this.activeTextColor;
                    }
                }
                else
                {
                    if (img != null) img.color = this.inactiveTabColor;
                    if (txt != null)
                    {
                        txt.fontStyle = FontStyles.Normal;
                        txt.color = this.inactiveTextColor;
                    }
                }
            }
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

        /// <summary>
        /// Retrieves an inactive UI element from the pool or instantiates a new one if none are available.
        /// Matches the base Spawner.cs design pattern.
        /// </summary>
        protected virtual InventoryItemUI GetOrCreateItemUI()
        {
            foreach (InventoryItemUI itemUI in this.itemUIPool)
            {
                if (itemUI != null && !itemUI.gameObject.activeSelf)
                {
                    return itemUI;
                }
            }

            if (this.itemPrefab == null) return null;

            // Instantiate without parent first, set inactive, then set parent to avoid layout glitches!
            GameObject newObj = Instantiate(this.itemPrefab);
            newObj.SetActive(false);
            newObj.transform.SetParent(this.contentContainer, false);

            InventoryItemUI newItemUI = newObj.GetComponent<InventoryItemUI>();
            if (newItemUI != null)
            {
                this.itemUIPool.Add(newItemUI);
            }

            return newItemUI;
        }

        /// <summary>
        /// Deactivates all UI elements in the pool, returning them for future reuse.
        /// </summary>
        protected virtual void DespawnAll()
        {
            foreach (InventoryItemUI itemUI in this.itemUIPool)
            {
                if (itemUI != null)
                {
                    itemUI.gameObject.SetActive(false);
                }
            }

            this.activeItemUIs.Clear();
        }

        protected virtual void ClearUI()
        {
            this.DespawnAll();
        }

        /// <summary>
        /// Refreshes the items list showing only elements corresponding to the active tab type.
        /// Uses advanced Object Pooling to reuse existing UI elements without deactivating active ones,
        /// ensuring the ScrollRect height never collapses to 0 and preventing scroll position reset.
        /// </summary>
        public virtual void RefreshUI()
        {
            if (MotherShipController.Instance == null || MotherShipController.Instance.Inventory == null) return;

            MotherShipInventory inventory = MotherShipController.Instance.Inventory;

            // Save the current vertical scroll position in absolute pixels
            RectTransform contentRect = this.contentContainer != null ? this.contentContainer.GetComponent<RectTransform>() : null;
            float savedScrollY = 0f;
            if (contentRect != null && !this._shouldResetScrollY)
            {
                savedScrollY = contentRect.anchoredPosition.y;
            }

            // Determine target ItemType filter based on current tab
            ItemType targetType = ItemType.Ore;
            if (this._currentTab == InventoryTab.Bars)
            {
                targetType = ItemType.Bar;
            }
            else if (this._currentTab == InventoryTab.Items)
            {
                targetType = ItemType.Item;
            }

            // Get filtered list of items to display
            List<InventoryItem> itemsToDisplay = new List<InventoryItem>();
            foreach (InventoryItem item in inventory.GetAllItems())
            {
                if (item.mineralData == null || item.amount <= 0) continue;
                if (item.mineralData.Type != targetType) continue; // Filter by type
                itemsToDisplay.Add(item);
            }

            this.activeItemUIs.Clear();

            int requiredCount = itemsToDisplay.Count;
            int poolCount = this.itemUIPool.Count;

            // Ensure we have enough instantiated elements in the pool
            if (poolCount < requiredCount)
            {
                int deficit = requiredCount - poolCount;
                for (int i = 0; i < deficit; i++)
                {
                    if (this.itemPrefab == null) break;

                    // Instantiate without parent first, set inactive, then set parent to avoid layout glitches!
                    GameObject newObj = Instantiate(this.itemPrefab);
                    newObj.SetActive(false);
                    newObj.transform.SetParent(this.contentContainer, false);

                    InventoryItemUI newItemUI = newObj.GetComponent<InventoryItemUI>();
                    if (newItemUI != null)
                    {
                        this.itemUIPool.Add(newItemUI);
                    }
                }
            }

            // Update active elements and deactivate the rest
            for (int i = 0; i < this.itemUIPool.Count; i++)
            {
                InventoryItemUI itemUI = this.itemUIPool[i];
                if (itemUI == null) continue;

                if (i < requiredCount)
                {
                    // For required items, update and make sure they are active
                    InventoryItem item = itemsToDisplay[i];

                    // Crucial: Update the UI data FIRST while the item is potentially inactive
                    // to prevent intermediate layout passes with dirty/default values.
                    itemUI.UpdateUI(item.mineralData, item.amount);

                    // Only SetActive(true) if currently inactive to avoid triggering layout dirty flags
                    if (!itemUI.gameObject.activeSelf)
                    {
                        itemUI.gameObject.SetActive(true);
                    }

                    this.activeItemUIs.Add(itemUI);
                }
                else
                {
                    // For extra items in the pool, make sure they are inactive
                    if (itemUI.gameObject.activeSelf)
                    {
                        itemUI.gameObject.SetActive(false);
                    }
                }
            }

            this.UpdateCapacityText(inventory);

            // Force immediate layout update and restore absolute scroll position
            if (contentRect != null)
            {
                // Force Canvas update first
                Canvas.ForceUpdateCanvases();

                // Force layout rebuilder to immediately compute sizes of the content container
                LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);

                ScrollRect scrollRect = this.contentContainer.GetComponentInParent<ScrollRect>();
                if (this._shouldResetScrollY)
                {
                    if (scrollRect != null)
                    {
                        scrollRect.verticalNormalizedPosition = 1f;
                    }
                }
                else
                {
                    // Restore the anchored position
                    Vector2 pos = contentRect.anchoredPosition;
                    pos.y = savedScrollY;
                    contentRect.anchoredPosition = pos;
                }

                this._shouldResetScrollY = false;
            }
        }

        /// <summary>
        /// Updates the capacity text dynamically to reflect the current tab's limits.
        /// </summary>
        protected virtual void UpdateCapacityText(MotherShipInventory inventory)
        {
            if (this.capacityText == null) return;
            
            ItemType targetType = ItemType.Ore;
            string prefix = "Ores";
            
            if (this._currentTab == InventoryTab.Bars)
            {
                targetType = ItemType.Bar;
                prefix = "Bars";
            }
            else if (this._currentTab == InventoryTab.Items)
            {
                targetType = ItemType.Item;
                prefix = "Items";
            }

            float current = inventory.GetCurrentCapacity(targetType);
            float max = inventory.GetMaxCapacity(targetType);
            
            this.capacityText.text = $"{prefix} Capacity: {current:F0} / {max:F0}";
        }
    }
}
