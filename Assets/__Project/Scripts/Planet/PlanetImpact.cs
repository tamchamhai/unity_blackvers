using UnityEngine;

namespace Blackvers.Planet
{
    public class PlanetImpact : MasterMonoBehaviour
    {
        [Header("References")]
        public PlanetController planetController;
        [SerializeField] protected CircleCollider2D circleCollider;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadPlanetController();
            this.LoadCollider();
        }

        protected virtual void LoadPlanetController()
        {
            if (this.planetController != null) return;
            this.planetController = this.transform.parent.GetComponent<PlanetController>();
        }

        protected virtual void LoadCollider()
        {
            if (this.circleCollider != null) return;
            this.circleCollider = transform.GetComponent<CircleCollider2D>();
            if (this.circleCollider == null)
            {
                this.circleCollider = gameObject.AddComponent<CircleCollider2D>();
            }
            this.circleCollider.isTrigger = true;
            this.circleCollider.radius = 1f;
        }
    }
}