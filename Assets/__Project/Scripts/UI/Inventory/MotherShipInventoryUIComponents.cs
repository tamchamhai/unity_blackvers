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
        /// Dedicated component loading for TopNavigationController.
        /// </summary>
        protected virtual void LoadTopNavigation()
        {
            if (this.topNavigation != null) return;
            Transform container = this.transform.Find("Container");
            if (container == null) return;

            Transform topNav = container.Find("Top_Navigation");
            if (topNav != null)
            {
                this.topNavigation = topNav.GetComponent<TopNavigationController>();
            }
        }

        /// <summary>
        /// Dedicated component loading for LeftSidebarController.
        /// </summary>
        protected virtual void LoadLeftSidebar()
        {
            if (this.leftSidebar != null) return;
            Transform container = this.transform.Find("Container");
            if (container == null) return;

            Transform mainContent = container.Find("Main_Content_Area");
            if (mainContent == null) return;

            Transform sidebar = mainContent.Find("Left_Sidebar");
            if (sidebar != null)
            {
                this.leftSidebar = sidebar.GetComponent<LeftSidebarController>();
            }
        }
    }
}
