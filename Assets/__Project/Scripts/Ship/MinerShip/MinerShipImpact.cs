using UnityEngine;

namespace Blackvers.Ship.MinerShip
{
    /// <summary>
    /// Handles collision detection for the Miner Ship.
    /// Informs the Controller when it hits a target.
    /// </summary>
    [RequireComponent(typeof(CircleCollider2D))]
    public class MinerShipImpact : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] protected MinerShipController controller;
        [SerializeField] protected CircleCollider2D circleCollider;

        protected virtual void Awake()
        {
            this.LoadComponents();
        }

        protected virtual void LoadComponents()
        {
            this.LoadController();
            this.LoadCollider();
        }

        protected virtual void LoadController()
        {
            if (this.controller != null) return;
            this.controller = this.transform.parent.GetComponent<MinerShipController>();
        }

        protected virtual void LoadCollider()
        {
            if (this.circleCollider != null) return;
            this.circleCollider = this.GetComponent<CircleCollider2D>();
            this.circleCollider.isTrigger = true;
        }

        public virtual void UpdateColliderRadius()
        {
            if (this.circleCollider == null || this.controller == null || this.controller.modelRenderer == null) return;
            
            Sprite sprite = this.controller.modelRenderer.sprite;
            if (sprite == null) return;

            float maxExtent = Mathf.Max(sprite.bounds.extents.x, sprite.bounds.extents.y);
            this.circleCollider.radius = maxExtent;
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            this.HandleCollision(collision);
        }

        protected virtual void HandleCollision(Collider2D collision)
        {
            Debug.Log($"[MinerShipImpact] {this.transform.parent.name} collided with: {collision.name}");
            if (this.controller == null) return;

            // Ignore other miner ships
            if (collision.GetComponent<MinerShipImpact>() != null) return;

            // Ignore MotherShip if the ship is empty (to avoid spawn collision logs)
            if (!this.controller.IsFullLoad && collision.transform.parent == this.controller.MotherShip) return;

            this.controller.OnImpactEntered(collision);
        }
    }
}
