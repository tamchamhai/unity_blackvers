using UnityEngine;

namespace Blackvers.Planet
{
    public class PlanetImpact : MasterMonoBehaviour
    {
        [Header("References")]
        public PlanetController planetController;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadPlanetController();
        }

        protected virtual void LoadPlanetController()
        {
            if (this.planetController != null) return;
            this.planetController = this.transform.parent.GetComponent<PlanetController>();
        }
    }
}