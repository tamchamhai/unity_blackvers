using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;

namespace Blackvers.Managers
{
    public class InputManager : MasterMonoBehaviour
    {
        private static InputManager _instance;
        public static InputManager Instance => _instance;

        // Fired when an interaction (click/touch) happens, passing screen position
        public Action<Vector2> OnInteractAction;
        
        // Fired when an interaction hits a 2D Collider, passing the clicked GameObject
        public Action<GameObject> OnInteractObject;

        protected GameInputActions inputActions;

        protected override void Awake()
        {
            base.Awake();
            this.MakeSingleton();
        }

        protected override void OnEnable()
        {
            if (this.inputActions == null)
            {
                this.inputActions = new GameInputActions();
                this.inputActions.Gameplay.Interact.performed += this.OnInteractPerformed;
            }
            this.inputActions.Enable();
        }

        protected override void OnDisable()
        {
            if (this.inputActions != null)
            {
                this.inputActions.Gameplay.Interact.performed -= this.OnInteractPerformed;
                this.inputActions.Disable();
            }
        }

        protected virtual void MakeSingleton()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnInteractPerformed(InputAction.CallbackContext context)
        {
            // Read pointer position from the new input system
            Vector2 screenPosition = this.inputActions.Gameplay.PointerPosition.ReadValue<Vector2>();
            this.OnInteractAction?.Invoke(screenPosition);

            // Execute raycast to find physical objects
            this.DetectObjectInteraction(screenPosition);
        }

        protected virtual void DetectObjectInteraction(Vector2 screenPosition)
        {
            if (Camera.main == null) return;

            // Block click if interacting with UI elements
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                // Note: For mobile touch, IsPointerOverGameObject might need touchId, but for now this handles standard clicks
                return;
            }

            // Convert screen position to world position
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
            Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.y);

            // Perform raycast exactly at the point
            RaycastHit2D hit = Physics2D.Raycast(worldPos2D, Vector2.zero);
            if (hit.collider != null)
            {
                this.OnInteractObject?.Invoke(hit.collider.gameObject);
            }
        }
    }
}
