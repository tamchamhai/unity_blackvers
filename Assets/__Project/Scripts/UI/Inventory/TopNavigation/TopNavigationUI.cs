using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Blackvers.UI.Inventory
{
    /// <summary>
    /// Manages the visual elements (View) of the top navigation tab bar in the MotherShip UI.
    /// Handles component references, caching, and visual updates for the tabs.
    /// </summary>
    public class TopNavigationUI : MasterMonoBehaviour
    {
        [Header("Tab Button References")]
        [SerializeField] protected Button tabInventoryButton;
        [SerializeField] protected Button tabShipsButton;
        [SerializeField] protected Button tabPlanetsButton;

        [Header("Tab Text References")]
        [SerializeField] protected TextMeshProUGUI tabInventoryText;
        [SerializeField] protected TextMeshProUGUI tabShipsText;
        [SerializeField] protected TextMeshProUGUI tabPlanetsText;

        [Header("Visual Theme Settings")]
        [SerializeField] protected Color activeTabColor = new Color(0.12f, 0.38f, 0.18f, 1f); // Sleek Emerald
        [SerializeField] protected Color inactiveTabColor = new Color(0.18f, 0.18f, 0.18f, 0.9f); // Dark Gray
        [SerializeField] protected Color activeTextColor = new Color(0.85f, 1f, 0.85f, 1f);
        [SerializeField] protected Color inactiveTextColor = new Color(0.7f, 0.7f, 0.7f, 1f);

        // Exposing buttons to the Controller for event binding
        public Button TabInventoryButton => this.tabInventoryButton;
        public Button TabShipsButton => this.tabShipsButton;
        public Button TabPlanetsButton => this.tabPlanetsButton;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadTabInventoryComponent();
            this.LoadTabShipsComponent();
            this.LoadTabPlanetsComponent();
        }

        /// <summary>
        /// Dedicated component loading for the Inventory Tab GameObject.
        /// </summary>
        protected virtual void LoadTabInventoryComponent()
        {
            if (this.tabInventoryButton != null) return;
            Transform child = this.transform.Find("Tab_Inventory");
            if (child == null) return;

            this.tabInventoryButton = child.GetComponent<Button>();
            if (this.tabInventoryButton == null)
            {
                this.tabInventoryButton = child.gameObject.AddComponent<Button>();
            }
            this.tabInventoryText = child.GetComponentInChildren<TextMeshProUGUI>();
        }

        /// <summary>
        /// Dedicated component loading for the Ships Tab GameObject.
        /// </summary>
        protected virtual void LoadTabShipsComponent()
        {
            if (this.tabShipsButton != null) return;
            Transform child = this.transform.Find("Tab_Ships");
            if (child == null) return;

            this.tabShipsButton = child.GetComponent<Button>();
            if (this.tabShipsButton == null)
            {
                this.tabShipsButton = child.gameObject.AddComponent<Button>();
            }
            this.tabShipsText = child.GetComponentInChildren<TextMeshProUGUI>();
        }

        /// <summary>
        /// Dedicated component loading for the Planets Tab GameObject.
        /// </summary>
        protected virtual void LoadTabPlanetsComponent()
        {
            if (this.tabPlanetsButton != null) return;
            Transform child = this.transform.Find("Tab_Planets");
            if (child == null) return;

            this.tabPlanetsButton = child.GetComponent<Button>();
            if (this.tabPlanetsButton == null)
            {
                this.tabPlanetsButton = child.gameObject.AddComponent<Button>();
            }
            this.tabPlanetsText = child.GetComponentInChildren<TextMeshProUGUI>();
        }

        /// <summary>
        /// Updates the colors and font weights of the tab buttons based on the active state.
        /// </summary>
        public virtual void UpdateTabVisuals(MainTab activeTab)
        {
            this.SetButtonVisual(this.tabInventoryButton, this.tabInventoryText, activeTab == MainTab.Inventory);
            this.SetButtonVisual(this.tabShipsButton, this.tabShipsText, activeTab == MainTab.Ships);
            this.SetButtonVisual(this.tabPlanetsButton, this.tabPlanetsText, activeTab == MainTab.Planets);
        }

        /// <summary>
        /// Helper to set the visual properties of a single tab button.
        /// </summary>
        protected virtual void SetButtonVisual(Button button, TextMeshProUGUI text, bool isActive)
        {
            if (button == null) return;

            Image backgroundImage = button.GetComponent<Image>();
            if (backgroundImage != null)
            {
                backgroundImage.color = isActive ? this.activeTabColor : this.inactiveTabColor;
            }

            if (text == null) return;

            text.color = isActive ? this.activeTextColor : this.inactiveTextColor;
            text.fontStyle = isActive ? FontStyles.Bold : FontStyles.Normal;
        }
    }
}
