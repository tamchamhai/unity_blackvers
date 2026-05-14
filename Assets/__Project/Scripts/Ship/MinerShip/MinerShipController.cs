using UnityEngine;
using Blackvers.Data;
using Blackvers.Commons;

namespace Blackvers.Ship.MinerShip
{
    /// <summary>
    /// Controller for individual Miner Ships.
    /// Manages its own data and visual representation.
    /// </summary>
    public class MinerShipController : MasterMonoBehaviour
    {
        [Header("Data")]
        public MinerShipData minerShipData;

        [Header("Runtime Stats")]
        public float currentSpeed;
        public int currentCapacity;
        public float currentMiningSpeed;

        [Header("Mission Targets")]
        [SerializeField] protected Transform targetPlanet;
        [SerializeField] protected Transform motherShip;
        public Transform MotherShip => this.motherShip;
        [SerializeField] protected MinerShipState currentState;

        [Header("Components")]
        public SpriteRenderer modelRenderer;
        public MinerShipMovement movement;
        public MinerShipImpact impact;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadModelRenderer();
            this.LoadMovement();
            this.LoadImpact();
            this.LoadDataFromSO();
        }

        protected virtual void LoadMovement()
        {
            if (this.movement != null) return;
            this.movement = this.transform.Find("Movement")?.GetComponent<MinerShipMovement>();
        }

        protected virtual void LoadImpact()
        {
            if (this.impact != null) return;
            this.impact = this.transform.Find("Impact")?.GetComponent<MinerShipImpact>();
        }

        /// <summary>
        /// Loads initial statistics from the assigned ScriptableObject.
        /// </summary>
        protected virtual void LoadDataFromSO()
        {
            if (this.minerShipData == null) return;

            this.currentSpeed = this.minerShipData.baseSpeed;
            this.currentCapacity = this.minerShipData.capacity;
            this.currentMiningSpeed = this.minerShipData.miningSpeed;
        }

        /// <summary>
        /// Automatically finds the SpriteRenderer in the 'Model' child.
        /// </summary>
        protected virtual void LoadModelRenderer()
        {
            if (this.modelRenderer != null) return;
            this.modelRenderer = this.transform.Find("Model")?.GetComponent<SpriteRenderer>();
        }

        [Header("Load Settings")]
        [SerializeField] protected bool isFullLoad = false;
        public bool IsFullLoad => this.isFullLoad;

        /// <summary>
        /// Called by MinerShipImpact when the ship collides with something.
        /// </summary>
        public virtual void OnImpactEntered(Collider2D collision)
        {
            if (!this.isFullLoad && collision.transform.parent == this.targetPlanet)
            {
                this.isFullLoad = true;
                this.SetState(MinerShipState.ToMotherShip);
                return;
            }

            if (this.isFullLoad && collision.transform.parent == this.motherShip)
            {
                this.isFullLoad = false;
                this.SetState(MinerShipState.ToPlanet);
            }
        }

        protected virtual void SetState(MinerShipState state)
        {
            this.currentState = state;
            this.UpdateMovementTarget();
        }

        protected virtual void UpdateMovementTarget()
        {
            if (this.movement == null) return;
            if (this.targetPlanet == null || this.motherShip == null) return;

            Vector3 targetPos = this.currentState == MinerShipState.ToPlanet 
                ? this.targetPlanet.position 
                : this.motherShip.position;

            this.movement.SetTarget(targetPos);
        }

        /// <summary>
        /// Initializes the ship based on its assigned data and mission.
        /// </summary>
        public virtual void Initialize(Transform planet, Transform mother)
        {
            this.LoadComponents();
            this.targetPlanet = planet;
            this.motherShip = mother;
            this.isFullLoad = false;
            
            this.movement.SetSpeed(this.currentSpeed);
            
            this.SetState(MinerShipState.ToPlanet);
            this.UpdateVisuals();
        }

        /// <summary>
        /// Updates the sprite based on the MinerShipData.
        /// </summary>
        protected virtual void UpdateVisuals()
        {
            if (this.minerShipData == null || this.modelRenderer == null) return;
            this.modelRenderer.sprite = this.minerShipData.shipSprite;

            if (this.impact != null)
            {
                this.impact.UpdateColliderRadius();
            }
        }
    }
}
