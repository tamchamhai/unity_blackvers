using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;

namespace Blackvers.Managers
{
    /// <summary>
    /// Central manager for all player input using Unity's new Input System.
    /// Handles interaction detection, UI blocking, and multitouch guard
    /// to prevent false triggers during pinch-to-zoom.
    ///
    /// The "Interact" action uses Unity's built-in Tap interaction, so "performed"
    /// only fires on a quick tap (press + release within tapTime). Long presses
    /// are automatically ignored by the Input System at the action level.
    /// </summary>
    public class InputManager : MasterMonoBehaviour
    {
        private static InputManager _instance;
        public static InputManager Instance => _instance;

        // Fired when an interaction (click/touch) happens, passing screen position
        public Action<Vector2> OnInteractAction;

        // Fired when a confirmed tap hits a 2D Collider, passing the tapped GameObject
        public Action<GameObject> OnInteractObject;

        protected GameInputActions inputActions;
        public GameInputActions InputActions => this.inputActions;

        protected override void Awake()
        {
            base.Awake();
            this.MakeSingleton();
        }

        protected override void OnEnable()
        {
            this.InitializeInputActions();
            this.inputActions.Enable();
        }

        protected override void OnDisable()
        {
            if (this.inputActions == null) return;
            this.inputActions.Gameplay.Interact.performed -= this.OnInteractPerformed;
            this.inputActions.Disable();
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

        protected virtual void InitializeInputActions()
        {
            if (this.inputActions != null) return;

            this.inputActions = new GameInputActions();

            // "performed" only fires on a quick Tap (configured in GameInputActions.inputactions).
            // Long presses / holds fire "canceled" without triggering "performed".
            this.inputActions.Gameplay.Interact.performed += this.OnInteractPerformed;
        }

        /// <summary>
        /// Called only when the player performs a confirmed quick tap.
        /// Long presses are filtered out by the Tap interaction in the Input Actions asset.
        /// </summary>
        protected virtual void OnInteractPerformed(InputAction.CallbackContext context)
        {
            Vector2 screenPosition = this.inputActions.Gameplay.PointerPosition.ReadValue<Vector2>();
            this.OnInteractAction?.Invoke(screenPosition);
            this.DetectObjectInteraction(screenPosition);
        }

        /// <summary>
        /// Performs a 2D raycast at the given screen position and fires OnInteractObject
        /// if a valid collider is hit. Blocked during multitouch gestures and when over UI.
        /// </summary>
        protected virtual void DetectObjectInteraction(Vector2 screenPosition)
        {
            if (Camera.main == null) return;
            if (TouchInputUtility.GetActiveTouchCount() >= 2) return;
            if (TouchInputUtility.IsPointerOverUI()) return;

            Vector3 worldPos   = Camera.main.ScreenToWorldPoint(screenPosition);
            Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.y);

            RaycastHit2D hit = Physics2D.Raycast(worldPos2D, Vector2.zero);
            if (hit.collider != null)
            {
                this.OnInteractObject?.Invoke(hit.collider.gameObject);
            }
        }
    }
}
