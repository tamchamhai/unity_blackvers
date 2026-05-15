using UnityEngine;
using Blackvers.Data;

namespace Blackvers.Planet
{
    /// <summary>
    /// Controller for planet behavior.
    /// Manages data and adjusts visuals on the 'Model' child object.
    /// </summary>
    public class PlanetController : MasterMonoBehaviour
    {
        public PlanetData planetData;
        public string planetId;
        public float radius;

        [Header("Internal References")]
        public SpriteRenderer modelRenderer;
        public PlanetMineralManager mineralManager;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadMineralManager();
        }

        protected virtual void LoadMineralManager()
        {
            if (this.mineralManager != null) return;
            this.mineralManager = this.GetComponentInChildren<PlanetMineralManager>();
        }

        /// <summary>
        /// Updates the planet's visuals based on the assigned data.
        /// </summary>
        public void Initialize()
        {
            if (this.planetData == null) return;
            this.planetId = Blackvers.Commons.CommonUtils.GenerateId();
            this.radius = this.planetData.radius;

            if (this.modelRenderer != null)
            {
                this.modelRenderer.sprite = this.planetData.planetSprite;
            }

            // Apply scale based on planet radius
            float scale = this.planetData.radius;
            this.transform.localScale = new Vector3(scale, scale, 1f);
            
            this.gameObject.name = this.planetData.planetName;

            if (this.mineralManager != null)
            {
                this.mineralManager.Initialize();
            }
        }

        [ContextMenu("Sync Visuals")]
        private void Sync() => this.Initialize();
    }
}
