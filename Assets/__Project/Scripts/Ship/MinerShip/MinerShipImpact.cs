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
            if (this.controller != null)
            {
                Debug.LogWarning(this.transform.name + ": Controller already loaded", this.gameObject);
                return;
            }
            this.controller = this.transform.parent.GetComponent<MinerShipController>();
        }

        protected virtual void LoadCollider()
        {
            if (this.circleCollider != null)
            {
                Debug.LogWarning(this.transform.name + ": CircleCollider already loaded", this.gameObject);
                return;
            }
            this.circleCollider = this.GetComponent<CircleCollider2D>();
            this.circleCollider.isTrigger = true;
        }

        public virtual void UpdateColliderRadius()
        {
            if (this.circleCollider == null || this.controller == null || this.controller.modelRenderer == null)
            {
                Debug.LogWarning(this.transform.name + ": Missing components to update collider radius", this.gameObject);
                return;
            }
            
            Sprite sprite = this.controller.modelRenderer.sprite;
            if (sprite == null)
            {
                Debug.LogWarning(this.transform.name + ": Sprite is null", this.gameObject);
                return;
            }

            float maxExtent = Mathf.Max(sprite.bounds.extents.x, sprite.bounds.extents.y);
            this.circleCollider.radius = maxExtent;
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            this.HandleCollision(collision);
        }

        protected virtual void HandleCollision(Collider2D collision)
        {
            if (this.controller == null)
            {
                Debug.LogWarning(this.transform.name + ": Controller is null in HandleCollision", this.gameObject);
                return;
            }

            // Ignore other miner ships
            if (collision.GetComponent<MinerShipImpact>() != null) return;

            // Ignore MotherShip if the ship is empty (to avoid spawn collision logs)
            if (!this.controller.IsFullLoad && collision.transform.parent == this.controller.MotherShip) return;

            if (!this.controller.IsFullLoad && collision.transform.parent == this.controller.TargetPlanet)
            {
                this.MinePlanet(collision.transform.parent);
                return;
            }

            if (this.controller.IsFullLoad && collision.transform.parent == this.controller.MotherShip)
            {
                this.UnloadMinerals(collision.transform.parent);
            }
        }

        protected virtual void MinePlanet(Transform planetTransform)
        {
            if (this.controller.minerInventory == null)
            {
                Debug.LogWarning(this.transform.name + ": MinerInventory is null in MinePlanet", this.gameObject);
                return;
            }

            Blackvers.Planet.PlanetController planet = planetTransform.GetComponent<Blackvers.Planet.PlanetController>();
            if (planet != null && planet.mineralManager != null)
            {
                float availableCapacity = this.controller.minerInventory.MaxCapacity - this.controller.minerInventory.CurrentCapacity;
                
                if (availableCapacity > 0)
                {
                    var collectedMinerals = planet.mineralManager.CollectMineral(availableCapacity);
                    foreach (var item in collectedMinerals)
                    {
                        this.controller.minerInventory.AddMineral(item.mineralData, item.amount);
                    }
                }
            }

            // Always return to mother-ship after touching planet
            this.controller.IsFullLoad = true;
            this.controller.SetState(MinerShipState.ToMotherShip);
        }

        protected virtual void UnloadMinerals(Transform motherShipTransform)
        {
            if (this.controller.minerInventory == null)
            {
                Debug.LogWarning(this.transform.name + ": MinerInventory is null in UnloadMinerals", this.gameObject);
                return;
            }

            MotherShipController msController = MotherShipController.Instance;
            if (msController != null && msController.Inventory != null)
            {
                var itemsToUnload = this.controller.minerInventory.GetAllItems();
                foreach (var item in itemsToUnload)
                {
                    msController.Inventory.AddMineral(item.mineralData, item.amount);
                }
            }

            // Clear inventory and go back to mining
            this.controller.minerInventory.Clear();
            this.controller.IsFullLoad = false;
            this.controller.SetState(MinerShipState.ToPlanet);
        }
    }
}
