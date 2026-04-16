using UnityEngine;
using Blackvers.Data;
using Blackvers.Commons;

namespace Blackvers.Planet
{
    /// <summary>
    /// Controller for planet behavior.
    /// Manages data and adjusts visuals on the 'Model' child object.
    /// </summary>
    public class PlanetController : MonoBehaviour
    {
        public PlanetData planetData;

        [Header("Internal References")]
        public SpriteRenderer modelRenderer;

        /// <summary>
        /// Updates the planet's visuals based on the assigned data.
        /// </summary>
        public void Initialize()
        {
            if (this.planetData == null) return;

            if (this.modelRenderer != null)
            {
                this.modelRenderer.sprite = this.planetData.planetSprite;
            }

            // Apply scale based on planet size
            float scale = this.planetData.size switch
            {
                PlanetSize.Small => 0.5f,
                PlanetSize.Medium => 1.0f,
                PlanetSize.Large => 1.8f,
                _ => 1.0f
            };
            this.transform.localScale = new Vector3(scale, scale, 1f);
            
            this.gameObject.name = this.planetData.planetName;
        }

        [ContextMenu("Sync Visuals")]
        private void Sync() => this.Initialize();
    }
}
