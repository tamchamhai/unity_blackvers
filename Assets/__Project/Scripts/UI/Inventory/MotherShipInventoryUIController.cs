using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Blackvers.Inventory;
using Blackvers.Data;
using TMPro;

namespace Blackvers.UI.Inventory
{
    /// <summary>
    /// Partial class for MotherShipInventoryUI containing tab navigation, object pooling, and UI refreshing logic.
    /// Manages the visual updates and runtime item rendering.
    /// </summary>
    public partial class MotherShipInventoryUI
    {
        /// <summary>
        /// Registers a single tab button and its text element to the tab system.
        /// </summary>
        protected virtual void RegisterTab(InventoryTab tab, Button button, TextMeshProUGUI text)
        {
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => this.SwitchTab(tab));
                this.tabButtons.Add(tab, button);
            }

            if (text != null)
            {
                this.tabTexts.Add(tab, text);
            }
        }

        /// <summary>
        /// Registers click listeners and builds internal references for the drag-and-drop tab components.
        /// </summary>
        protected virtual void InitializeTabSystem()
        {
            this.tabButtons.Clear();
            this.tabTexts.Clear();

            this.RegisterTab(InventoryTab.Ores, this.tabOresButton, this.tabOresText);
            this.RegisterTab(InventoryTab.Bars, this.tabBarsButton, this.tabBarsText);
            this.RegisterTab(InventoryTab.Items, this.tabItemsButton, this.tabItemsText);

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
            foreach (KeyValuePair<InventoryTab, Button> keyValuePair in this.tabButtons)
            {
                Image tabImage = keyValuePair.Value.GetComponent<Image>();
                TextMeshProUGUI tabTextComponent = this.tabTexts.ContainsKey(keyValuePair.Key) ? this.tabTexts[keyValuePair.Key] : null;

                if (keyValuePair.Key == this._currentTab)
                {
                    if (tabImage != null)
                    {
                        tabImage.color = this.activeTabColor;
                    }

                    if (tabTextComponent != null)
                    {
                        tabTextComponent.fontStyle = FontStyles.Bold;
                        tabTextComponent.color = this.activeTextColor;
                    }
                }
                else
                {
                    if (tabImage != null)
                    {
                        tabImage.color = this.inactiveTabColor;
                    }

                    if (tabTextComponent != null)
                    {
                        tabTextComponent.fontStyle = FontStyles.Normal;
                        tabTextComponent.color = this.inactiveTextColor;
                    }
                }
            }
        }

        /// <summary>
        /// Instantiates a new inventory item UI element and adds it to the pool.
        /// </summary>
        protected virtual InventoryItemUI CreateNewItemUI()
        {
            if (this.itemPrefab == null)
            {
                return null;
            }

            // Instantiate without parent first, set inactive, then set parent to avoid layout glitches!
            GameObject newGameObject = Instantiate(this.itemPrefab);
            newGameObject.SetActive(false);
            newGameObject.transform.SetParent(this.contentContainer, false);

            InventoryItemUI newItemUI = newGameObject.GetComponent<InventoryItemUI>();
            if (newItemUI != null)
            {
                this.itemUIPool.Add(newItemUI);
            }

            return newItemUI;
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

            return this.CreateNewItemUI();
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

        /// <summary>
        /// Clears all active UI elements by despawning them.
        /// </summary>
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
            if (MotherShipController.Instance == null || MotherShipController.Instance.Inventory == null)
            {
                return;
            }

            MotherShipInventory motherShipInventory = MotherShipController.Instance.Inventory;

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
            foreach (InventoryItem item in motherShipInventory.GetAllItems())
            {
                if (item.mineralData == null || item.amount <= 0)
                {
                    continue;
                }

                if (item.mineralData.Type != targetType)
                {
                    continue; // Filter by type
                }

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
                    this.CreateNewItemUI();
                }
            }

            // Update active elements and deactivate the rest
            for (int i = 0; i < this.itemUIPool.Count; i++)
            {
                InventoryItemUI itemUI = this.itemUIPool[i];
                if (itemUI == null)
                {
                    continue;
                }

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

            this.UpdateCapacityText(motherShipInventory);

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
                    Vector2 anchoredPosition = contentRect.anchoredPosition;
                    anchoredPosition.y = savedScrollY;
                    contentRect.anchoredPosition = anchoredPosition;
                }

                this._shouldResetScrollY = false;
            }
        }

        /// <summary>
        /// Updates the capacity text dynamically to reflect the current tab's limits.
        /// </summary>
        protected virtual void UpdateCapacityText(MotherShipInventory motherShipInventory)
        {
            if (this.capacityText == null)
            {
                return;
            }

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

            float current = motherShipInventory.GetCurrentCapacity(targetType);
            float maxCapacity = motherShipInventory.GetMaxCapacity(targetType);

            this.capacityText.text = $"{prefix} Capacity: {current:F0} / {maxCapacity:F0}";
        }
    }
}
