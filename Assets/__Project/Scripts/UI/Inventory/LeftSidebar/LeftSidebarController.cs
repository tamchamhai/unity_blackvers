using UnityEngine;
using System;

namespace Blackvers.UI.Inventory
{
    /// <summary>
    /// Controls the logic and user interactions for the left sidebar sub-navigation system.
    /// Acts as the Controller in the MVC structure, binding view actions to the data flow.
    /// </summary>
    public class LeftSidebarController : MasterMonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected LeftSidebarUI view;

        // Action triggered when a sub-tab is clicked/selected
        public Action<InventorySubTab> OnSubTabSelected;

        // Currently selected sub-tab
        private InventorySubTab _currentSubTab = InventorySubTab.Ore;
        public InventorySubTab CurrentSubTab => this._currentSubTab;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadViewComponent();
        }

        protected override void Start()
        {
            base.Start();
            this.InitializeSidebarSystem();
        }

        /// <summary>
        /// Dedicated component loading to find the LeftSidebarUI View.
        /// </summary>
        protected virtual void LoadViewComponent()
        {
            if (this.view != null) return;
            this.view = this.GetComponent<LeftSidebarUI>();
        }

        /// <summary>
        /// Initializes sidebar event bindings and initial visual states.
        /// </summary>
        protected virtual void InitializeSidebarSystem()
        {
            if (this.view == null)
            {
                Debug.LogWarning(this.transform.name + ": LeftSidebarUI View is missing!", this.gameObject);
                return;
            }

            this.RegisterSubTabClickListeners();
            this.view.UpdateSubTabVisuals(this._currentSubTab);
        }

        /// <summary>
        /// Binds click events from the View to the Controller's handler logic.
        /// </summary>
        protected virtual void RegisterSubTabClickListeners()
        {
            if (this.view.SubTabOreButton != null)
            {
                this.view.SubTabOreButton.onClick.RemoveAllListeners();
                this.view.SubTabOreButton.onClick.AddListener(() => this.HandleSubTabClicked(InventorySubTab.Ore));
            }

            if (this.view.SubTabFragmentsButton != null)
            {
                this.view.SubTabFragmentsButton.onClick.RemoveAllListeners();
                this.view.SubTabFragmentsButton.onClick.AddListener(() => this.HandleSubTabClicked(InventorySubTab.Fragments));
            }

            if (this.view.SubTabItemsButton != null)
            {
                this.view.SubTabItemsButton.onClick.RemoveAllListeners();
                this.view.SubTabItemsButton.onClick.AddListener(() => this.HandleSubTabClicked(InventorySubTab.Items));
            }

            if (this.view.SubTabSoldiersButton != null)
            {
                this.view.SubTabSoldiersButton.onClick.RemoveAllListeners();
                this.view.SubTabSoldiersButton.onClick.AddListener(() => this.HandleSubTabClicked(InventorySubTab.Soldiers));
            }
        }

        /// <summary>
        /// Triggers sub-navigation logic and visual updates when a sub-tab is clicked.
        /// </summary>
        protected virtual void HandleSubTabClicked(InventorySubTab selectedSubTab)
        {
            this._currentSubTab = selectedSubTab;
            
            if (this.view != null)
            {
                this.view.UpdateSubTabVisuals(selectedSubTab);
            }

            this.OnSubTabSelected?.Invoke(selectedSubTab);
        }

        /// <summary>
        /// Public method to force switch active sub-tab programmatically.
        /// </summary>
        public virtual void SelectSubTab(InventorySubTab subTab)
        {
            this.HandleSubTabClicked(subTab);
        }
    }
}
