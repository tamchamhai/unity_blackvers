using UnityEngine;
using System;

namespace Blackvers.UI.Inventory
{
    /// <summary>
    /// Controls the logic and user interactions for the top navigation system.
    /// Acts as the Controller in the MVC structure, binding view actions to the data flow.
    /// </summary>
    public class TopNavigationController : MasterMonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected TopNavigationUI view;

        [Header("Panel References")]
        [SerializeField] protected GameObject mainContentArea;
        [SerializeField] protected GameObject shipsPanel;
        [SerializeField] protected GameObject planetsPanel;

        // Action triggered when a main tab is clicked/selected
        public Action<MainTab> OnTabSelected;

        // Currently selected main tab
        private MainTab _currentTab = MainTab.Inventory;
        public MainTab CurrentTab => this._currentTab;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadViewComponent();
            this.LoadMainContentArea();
            this.LoadShipsPanel();
            this.LoadPlanetsPanel();
        }

        protected override void Start()
        {
            base.Start();
            this.InitializeNavigationSystem();
        }

        /// <summary>
        /// Dedicated component loading to find the TopNavigationUI View.
        /// </summary>
        protected virtual void LoadViewComponent()
        {
            if (this.view != null) return;
            this.view = this.GetComponent<TopNavigationUI>();
        }

        /// <summary>
        /// Dedicated component loading for mainContentArea.
        /// </summary>
        protected virtual void LoadMainContentArea()
        {
            if (this.mainContentArea != null) return;
            Transform parentTransform = this.transform.parent;
            if (parentTransform == null) return;

            Transform mainContent = parentTransform.Find("Main_Content_Area");
            if (mainContent != null)
            {
                this.mainContentArea = mainContent.gameObject;
            }
        }

        /// <summary>
        /// Dedicated component loading for shipsPanel.
        /// </summary>
        protected virtual void LoadShipsPanel()
        {
            if (this.shipsPanel != null) return;
            Transform parentTransform = this.transform.parent;
            if (parentTransform == null) return;

            Transform panel = parentTransform.Find("Ships_Panel");
            if (panel != null)
            {
                this.shipsPanel = panel.gameObject;
            }
        }

        /// <summary>
        /// Dedicated component loading for planetsPanel.
        /// </summary>
        protected virtual void LoadPlanetsPanel()
        {
            if (this.planetsPanel != null) return;
            Transform parentTransform = this.transform.parent;
            if (parentTransform == null) return;

            Transform panel = parentTransform.Find("Planets_Panel");
            if (panel != null)
            {
                this.planetsPanel = panel.gameObject;
            }
        }

        /// <summary>
        /// Initializes navigation event bindings and initial visual states.
        /// </summary>
        protected virtual void InitializeNavigationSystem()
        {
            if (this.view == null)
            {
                Debug.LogWarning(this.transform.name + ": TopNavigationUI View is missing!", this.gameObject);
                return;
            }

            this.RegisterTabClickListeners();
            this.view.UpdateTabVisuals(this._currentTab);
            this.UpdatePanelStates(this._currentTab);
        }

        /// <summary>
        /// Binds click events from the View to the Controller's handler logic.
        /// </summary>
        protected virtual void RegisterTabClickListeners()
        {
            if (this.view.TabInventoryButton != null)
            {
                this.view.TabInventoryButton.onClick.RemoveAllListeners();
                this.view.TabInventoryButton.onClick.AddListener(() => this.HandleTabClicked(MainTab.Inventory));
            }

            if (this.view.TabShipsButton != null)
            {
                this.view.TabShipsButton.onClick.RemoveAllListeners();
                this.view.TabShipsButton.onClick.AddListener(() => this.HandleTabClicked(MainTab.Ships));
            }

            if (this.view.TabPlanetsButton != null)
            {
                this.view.TabPlanetsButton.onClick.RemoveAllListeners();
                this.view.TabPlanetsButton.onClick.AddListener(() => this.HandleTabClicked(MainTab.Planets));
            }
        }

        /// <summary>
        /// Updates the active/inactive states of target main panels depending on the active MainTab.
        /// </summary>
        protected virtual void UpdatePanelStates(MainTab tab)
        {
            if (this.mainContentArea != null)
            {
                this.mainContentArea.SetActive(tab == MainTab.Inventory);
            }

            if (this.shipsPanel != null)
            {
                this.shipsPanel.SetActive(tab == MainTab.Ships);
            }

            if (this.planetsPanel != null)
            {
                this.planetsPanel.SetActive(tab == MainTab.Planets);
            }
        }

        /// <summary>
        /// Triggers navigation logic and visual updates when a tab is clicked.
        /// </summary>
        protected virtual void HandleTabClicked(MainTab selectedTab)
        {
            this._currentTab = selectedTab;
            
            if (this.view != null)
            {
                this.view.UpdateTabVisuals(selectedTab);
            }

            this.UpdatePanelStates(selectedTab);

            this.OnTabSelected?.Invoke(selectedTab);
        }

        /// <summary>
        /// Public method to force switch active tab programmatically.
        /// </summary>
        public virtual void SelectTab(MainTab tab)
        {
            this.HandleTabClicked(tab);
        }
    }
}
