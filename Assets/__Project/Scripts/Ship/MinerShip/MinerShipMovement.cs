using UnityEngine;
using Blackvers.Commons;

namespace Blackvers.Ship.MinerShip
{
    /// <summary>
    /// Handles the physical movement and orientation of the Miner Ship.
    /// This component operates on its parent transform (the MinerShipController).
    /// </summary>
    public class MinerShipMovement : MasterMonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] protected float speed = 10f;
        [SerializeField] protected Vector3 targetPosition;
        [SerializeField] protected bool isMoving = false;

        [Header("Rotation Settings")]
        [SerializeField] protected float rotationSpeed = 300f;
        [SerializeField] protected float rotationOffset = 180f;

        protected override void Update()
        {
            this.HandleMovement();
            this.HandleRotation();
        }

        public virtual void SetSpeed(float newSpeed)
        {
            this.speed = newSpeed;
        }

        public virtual void SetTarget(Vector3 position)
        {
            this.targetPosition = position;
            this.isMoving = true;
        }

        /// <summary>
        /// Moves the parent transform towards the target position.
        /// </summary>
        protected virtual void HandleMovement()
        {
            if (!this.isMoving) return;

            Vector3 currentPos = this.transform.parent.position;
            this.transform.parent.position = Vector3.MoveTowards(
                currentPos, 
                this.targetPosition, 
                this.speed * Time.deltaTime
            );

            if (Vector3.Distance(this.transform.parent.position, this.targetPosition) < 0.01f)
            {
                this.StopMoving();
            }
        }

        /// <summary>
        /// Rotates the parent transform to face the direction of movement.
        /// </summary>
        protected virtual void HandleRotation()
        {
            if (!this.isMoving) return;
            
            Vector3 direction = this.GetCurrentDirection();
            if (direction == Vector3.zero) return;

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle + this.rotationOffset);

            this.transform.parent.rotation = Quaternion.RotateTowards(
                this.transform.parent.rotation, 
                targetRotation, 
                this.rotationSpeed * Time.deltaTime
            );
        }

        public virtual void StopMoving()
        {
            this.isMoving = false;
        }

        public virtual bool HasReachedTarget()
        {
            return !this.isMoving;
        }

        public virtual Vector3 GetCurrentDirection()
        {
            if (!this.isMoving) return Vector3.zero;
            return (this.targetPosition - this.transform.parent.position).normalized;
        }
    }
}
