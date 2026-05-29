using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Blackvers.UI.Styles;

namespace Blackvers.UI.Inventory
{
    /// <summary>
    /// Manages the visual elements (View) of the left sidebar sub-navigation in the MotherShip Inventory UI.
    /// Handles component references, caching, and visual updates for sub-tabs (Ore, Fragments, Items, Soldiers).
    /// </summary>
    public class LeftSidebarUI : MasterMonoBehaviour
    {
        [Header("Sub-Tab Button References")]
        [SerializeField] protected Button subTabOreButton;
        [SerializeField] protected Button subTabFragmentsButton;
        [SerializeField] protected Button subTabItemsButton;
        [SerializeField] protected Button subTabSoldiersButton;

        [Header("Sub-Tab Text References")]
        [SerializeField] protected TextMeshProUGUI subTabOreText;
        [SerializeField] protected TextMeshProUGUI subTabFragmentsText;
        [SerializeField] protected TextMeshProUGUI subTabItemsText;
        [SerializeField] protected TextMeshProUGUI subTabSoldiersText;

        [Header("Visual Theme Settings")]
        [SerializeField] protected LeftSidebarStyle style;

        // Exposing buttons to the Controller for event binding
        public Button SubTabOreButton => this.subTabOreButton;
        public Button SubTabFragmentsButton => this.subTabFragmentsButton;
        public Button SubTabItemsButton => this.subTabItemsButton;
        public Button SubTabSoldiersButton => this.subTabSoldiersButton;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadSubTabOreComponent();
            this.LoadSubTabFragmentsComponent();
            this.LoadSubTabItemsComponent();
            this.LoadSubTabSoldiersComponent();
        }

        /// <summary>
        /// Dedicated component loading for the Ore Sub-Tab.
        /// </summary>
        protected virtual void LoadSubTabOreComponent()
        {
            if (this.subTabOreButton != null) return;
            Transform child = this.transform.Find("SubTab_Ore");
            if (child == null) return;

            this.subTabOreButton = child.GetComponent<Button>();
            if (this.subTabOreButton == null)
            {
                this.subTabOreButton = child.gameObject.AddComponent<Button>();
            }
            this.subTabOreText = child.GetComponentInChildren<TextMeshProUGUI>();
        }

        /// <summary>
        /// Dedicated component loading for the Fragments Sub-Tab.
        /// </summary>
        protected virtual void LoadSubTabFragmentsComponent()
        {
            if (this.subTabFragmentsButton != null) return;
            Transform child = this.transform.Find("SubTab_Fragments");
            if (child == null) return;

            this.subTabFragmentsButton = child.GetComponent<Button>();
            if (this.subTabFragmentsButton == null)
            {
                this.subTabFragmentsButton = child.gameObject.AddComponent<Button>();
            }
            this.subTabFragmentsText = child.GetComponentInChildren<TextMeshProUGUI>();
        }

        /// <summary>
        /// Dedicated component loading for the Items Sub-Tab.
        /// </summary>
        protected virtual void LoadSubTabItemsComponent()
        {
            if (this.subTabItemsButton != null) return;
            Transform child = this.transform.Find("SubTab_Items");
            if (child == null) return;

            this.subTabItemsButton = child.GetComponent<Button>();
            if (this.subTabItemsButton == null)
            {
                this.subTabItemsButton = child.gameObject.AddComponent<Button>();
            }
            this.subTabItemsText = child.GetComponentInChildren<TextMeshProUGUI>();
        }

        /// <summary>
        /// Dedicated component loading for the Soldiers Sub-Tab.
        /// </summary>
        protected virtual void LoadSubTabSoldiersComponent()
        {
            if (this.subTabSoldiersButton != null) return;
            Transform child = this.transform.Find("SubTab_Soldiers");
            if (child == null) return;

            this.subTabSoldiersButton = child.GetComponent<Button>();
            if (this.subTabSoldiersButton == null)
            {
                this.subTabSoldiersButton = child.gameObject.AddComponent<Button>();
            }
            this.subTabSoldiersText = child.GetComponentInChildren<TextMeshProUGUI>();
        }

        /// <summary>
        /// Updates the colors and font weights of the sub-tab buttons based on the active state.
        /// </summary>
        public virtual void UpdateSubTabVisuals(InventorySubTab activeSubTab)
        {
            this.SetButtonVisual(this.subTabOreButton, this.subTabOreText, activeSubTab == InventorySubTab.Ore);
            this.SetButtonVisual(this.subTabFragmentsButton, this.subTabFragmentsText, activeSubTab == InventorySubTab.Fragments);
            this.SetButtonVisual(this.subTabItemsButton, this.subTabItemsText, activeSubTab == InventorySubTab.Items);
            this.SetButtonVisual(this.subTabSoldiersButton, this.subTabSoldiersText, activeSubTab == InventorySubTab.Soldiers);
        }

        /// <summary>
        /// Helper to set the visual properties of a single sub-tab button.
        /// </summary>
        protected virtual void SetButtonVisual(Button button, TextMeshProUGUI text, bool isActive)
        {
            if (button == null) return;

            Image backgroundImage = button.GetComponent<Image>();
            if (backgroundImage != null && this.style != null)
            {
                backgroundImage.color = isActive ? this.style.ActiveSubTabColor : this.style.InactiveSubTabColor;
            }

            if (text == null) return;

            text.color = isActive ? this.style.ActiveTextColor : this.style.InactiveTextColor;
            text.fontStyle = isActive ? FontStyles.Bold : FontStyles.Normal;
        }
    }
}
