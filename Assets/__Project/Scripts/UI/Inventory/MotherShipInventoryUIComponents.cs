using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Blackvers.UI.Inventory
{
    /// <summary>
    /// Partial class for MotherShipInventoryUI that handles component loading and reference caching.
    /// Follows the custom MasterBehaviour loading pattern.
    /// </summary>
    public partial class MotherShipInventoryUI
    {
        /// <summary>
        /// Dedicated component loading function for CanvasGroup.
        /// </summary>
        protected virtual void LoadCanvasGroup()
        {
            this._canvasGroup = this.GetComponent<CanvasGroup>();
            if (this._canvasGroup == null)
            {
                this._canvasGroup = this.gameObject.AddComponent<CanvasGroup>();
            }
        }

        /// <summary>
        /// Dedicated component loading function for MainPanel GameObject.
        /// </summary>
        protected virtual void LoadMainPanel()
        {
            if (this.mainPanel != null) return;
            Transform container = this.transform.Find("Container");
            if (container != null)
            {
                this.mainPanel = container.gameObject;
            }
        }

        /// <summary>
        /// Dedicated component loading function for ContentContainer Transform in ScrollView.
        /// </summary>
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

        /// <summary>
        /// Dedicated component loading function for Capacity TextMeshProUGUI text display.
        /// </summary>
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

        /// <summary>
        /// Dedicated component loading function for loading the item Prefab from assets folder in Editor.
        /// </summary>
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
        /// Automatically attaches a Button component if it is missing from the GameObject.
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
    }
}
