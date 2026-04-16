using UnityEngine;
using Blackvers.Data;

namespace Blackvers.Planet
{
    /// <summary>
    /// Controller for planet behavior.
    /// Manages data and adjusts visuals on the 'Model' child object.
    /// </summary>
    public class PlanetController : MonoBehaviour
    {
        public PlanetData planetData;
        public float radius;

        [Header("Internal References")]
        public SpriteRenderer modelRenderer;

        /// <summary>
        /// Updates the planet's visuals based on the assigned data.
        /// </summary>
        public void Initialize()
        {
            if (this.planetData == null) return;
            this.radius = this.planetData.radius;

            if (this.modelRenderer != null)
            {
                this.modelRenderer.sprite = this.planetData.planetSprite;
            }

            // Apply scale based on planet radius
            float scale = this.planetData.radius;
            this.transform.localScale = new Vector3(scale, scale, 1f);
            
            this.gameObject.name = this.planetData.planetName;
        }

        [ContextMenu("Sync Visuals")]
        private void Sync() => this.Initialize();
    }
}
