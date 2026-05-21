using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Blackvers.Managers;

/// <summary>
/// Controls the camera's orthographic size (zoom) and position (panning)
/// using Unity's new Input System, with support for both PC (mouse scroll / RMB drag)
/// and Mobile (pinch-to-zoom / single-finger drag). Respects UI boundaries so that
/// inputs occurring over UI panels (e.g. inventory) do not trigger camera movement.
/// </summary>
public class CameraZoomController : MasterMonoBehaviour
{
    [Header("Components")]
    [SerializeField] protected Camera zoomCamera;

    [Header("Zoom Settings")]
    [SerializeField] protected float zoomSpeed = 10f;
    [SerializeField] protected float minSize   = 10f;
    [SerializeField] protected float maxSize   = 500f;

    [Header("Panning Settings")]
    [SerializeField] protected bool  canPan   = true;
    [SerializeField] protected float panSpeed = 1f;

    private Vector3 _dragOrigin;
    private bool    _isDragging;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadZoomCamera();
    }

    protected virtual void LoadZoomCamera()
    {
        if (this.zoomCamera != null) return;

        this.zoomCamera = GetComponent<Camera>();
        if (this.zoomCamera == null)
        {
            Debug.LogWarning($"{this.transform.name}: Zoom Camera not assigned!", this.gameObject);
        }
    }

    protected override void Update()
    {
        this.HandleZoomInput();
        this.HandlePanningInput();
    }

    /// <summary>
    /// Routes zoom input to the correct handler depending on active platform and touch count.
    /// Priority: two-finger pinch → mobile guard → PC scroll wheel.
    /// </summary>
    protected virtual void HandleZoomInput()
    {
        if (this.zoomCamera == null) return;
        if (TouchInputUtility.IsPointerOverUI()) return;

        // Two or more active fingers → pinch-to-zoom (also covers editor touch simulation)
        if (TouchInputUtility.GetActiveTouchCount() >= 2)
        {
            this.HandleMobilePinchZoom();
            return;
        }

        // On a real mobile device, block scroll emulation triggered by single-finger swipes
        if (Application.isMobilePlatform) return;

        // PC: read scroll wheel via Input Actions
        this.HandleScrollWheelZoom();
    }

    /// <summary>
    /// Zooms the camera using the mouse scroll wheel (PC only).
    /// </summary>
    protected virtual void HandleScrollWheelZoom()
    {
        if (InputManager.Instance == null || InputManager.Instance.InputActions == null) return;

        float scrollInput = InputManager.Instance.InputActions.Gameplay.Zoom.ReadValue<Vector2>().y;
        if (scrollInput == 0) return;

        float newSize = this.zoomCamera.orthographicSize - (scrollInput / 120f * this.zoomSpeed);
        this.zoomCamera.orthographicSize = Mathf.Clamp(newSize, this.minSize, this.maxSize);
    }

    /// <summary>
    /// Zooms the camera by measuring the change in distance between two active fingers.
    /// </summary>
    protected virtual void HandleMobilePinchZoom()
    {
        if (!TouchInputUtility.TryGetTwoActiveTouches(out TouchControl touch0, out TouchControl touch1)) return;

        bool touch0Moved = touch0.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved;
        bool touch1Moved = touch1.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved;
        if (!touch0Moved && !touch1Moved) return;

        Vector2 position0 = touch0.position.ReadValue();
        Vector2 position1 = touch1.position.ReadValue();

        float previousDistance = Vector2.Distance(position0 - touch0.delta.ReadValue(),
                                                   position1 - touch1.delta.ReadValue());
        float currentDistance  = Vector2.Distance(position0, position1);
        float distanceDelta    = currentDistance - previousDistance;

        float newSize = this.zoomCamera.orthographicSize - (distanceDelta * this.zoomSpeed * 0.05f);
        this.zoomCamera.orthographicSize = Mathf.Clamp(newSize, this.minSize, this.maxSize);
    }

    /// <summary>
    /// Handles camera panning via right-mouse-button drag (PC) or single-finger drag (Mobile).
    /// Automatically cancelled when two or more fingers are active (during pinch-to-zoom).
    /// </summary>
    protected virtual void HandlePanningInput()
    {
        if (!this.canPan || this.zoomCamera == null) return;
        if (InputManager.Instance == null || InputManager.Instance.InputActions == null) return;

        // Disable panning while the user is pinching to zoom
        if (TouchInputUtility.GetActiveTouchCount() >= 2)
        {
            this._isDragging = false;
            return;
        }

        InputAction panAction             = InputManager.Instance.InputActions.Gameplay.Pan;
        InputAction pointerPositionAction = InputManager.Instance.InputActions.Gameplay.PointerPosition;
        Vector2     pointerPosition       = pointerPositionAction.ReadValue<Vector2>();

        this.TryBeginDrag(panAction, pointerPosition);
        this.TryEndDrag(panAction);
        this.ApplyDrag(panAction, pointerPosition);
    }

    /// <summary>
    /// Starts a drag if the pan action was pressed this frame and the pointer is not over UI.
    /// </summary>
    protected virtual void TryBeginDrag(InputAction panAction, Vector2 pointerPosition)
    {
        if (!panAction.WasPressedThisFrame()) return;

        if (TouchInputUtility.IsPointerOverUI())
        {
            this._isDragging = false;
            return;
        }

        this._isDragging = true;
        this._dragOrigin = this.zoomCamera.ScreenToWorldPoint(pointerPosition);
    }

    /// <summary>
    /// Ends the active drag when the pan action is no longer held.
    /// </summary>
    protected virtual void TryEndDrag(InputAction panAction)
    {
        if (!panAction.IsPressed())
        {
            this._isDragging = false;
        }
    }

    /// <summary>
    /// Moves the camera by the world-space delta from the original drag origin to the current pointer position.
    /// </summary>
    protected virtual void ApplyDrag(InputAction panAction, Vector2 pointerPosition)
    {
        if (!panAction.IsPressed() || !this._isDragging) return;

        Vector3 currentWorldPosition = this.zoomCamera.ScreenToWorldPoint(pointerPosition);
        Vector3 delta = this._dragOrigin - currentWorldPosition;

        this.transform.position += new Vector3(delta.x, delta.y, 0) * this.panSpeed;
    }
}
