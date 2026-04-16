using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZoomController : MasterMonoBehaviour
{
    [Header("Components")]
    [SerializeField] protected Camera zoomCamera;

    [Header("Zoom Settings")]
    [SerializeField] protected float zoomSpeed = 60f; 
    [SerializeField] protected float minSize = 10f;
    [SerializeField] protected float maxSize = 60f;

    [Header("Panning Settings")]
    [SerializeField] protected bool canPan = true;
    [SerializeField] protected float panSpeed = 1f;

    private Vector3 _dragOrigin;

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
            Debug.LogWarning($"{transform.name}: Zoom Camera not assigned!", gameObject);
        }
    }

    protected override void Update()
    {
        this.HandleZoomInput();
        this.HandlePanningInput();
    }

    protected virtual void HandlePanningInput()
    {
        if (!this.canPan || !this.zoomCamera) return;

        // Start drag (Right Mouse Button)
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            this._dragOrigin = this.zoomCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }

        // During drag
        if (Mouse.current.rightButton.isPressed)
        {
            Vector3 currentPos = this.zoomCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3 difference = this._dragOrigin - currentPos;

            // Maintain camera's original Z position
            Vector3 move = new Vector3(difference.x, difference.y, 0);
            transform.position += move * this.panSpeed;
        }
    }

    protected virtual void HandleZoomInput()
    {
        if (!this.zoomCamera) return;

        var scrollInput = Mouse.current.scroll.ReadValue().y;
        if (scrollInput == 0) return;

        var normalizedScroll = scrollInput / 120f;
        var newSize = this.zoomCamera.orthographicSize - (normalizedScroll * this.zoomSpeed);
            
        this.zoomCamera.orthographicSize = Mathf.Clamp(newSize, this.minSize, this.maxSize);
    }
}
