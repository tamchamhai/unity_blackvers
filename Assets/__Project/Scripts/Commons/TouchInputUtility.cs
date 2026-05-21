using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

/// <summary>
/// Provides shared utility methods for touch input detection.
/// Centralizes active-touch counting and UI overlay checks
/// to avoid duplicating logic across InputManager and CameraZoomController.
/// </summary>
public static class TouchInputUtility
{
    // Reusable list to avoid per-frame GC allocation during UI raycasting
    private static readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();

    /// <summary>
    /// Returns the number of touches currently active on the touchscreen.
    /// A touch is considered active when its phase is Began, Moved, or Stationary (isInProgress).
    /// Returns 0 if no touchscreen device is connected.
    /// </summary>
    public static int GetActiveTouchCount()
    {
        if (Touchscreen.current == null) return 0;

        int count = 0;
        foreach (TouchControl touch in Touchscreen.current.touches)
        {
            if (touch.isInProgress) count++;
        }
        return count;
    }

    /// <summary>
    /// Checks whether the pointer or any active touch is currently positioned over an interactive UI element.
    /// Uses EventSystem.RaycastAll to detect only active, visible Graphics with raycastTarget enabled.
    /// This correctly ignores transparent/inactive parent containers (e.g. an inventory wrapper
    /// whose child panel has been hidden via SetActive(false)).
    /// </summary>
    public static bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        // Mobile: check each active touch individually
        if (Touchscreen.current != null)
        {
            foreach (TouchControl touch in Touchscreen.current.touches)
            {
                if (!touch.isInProgress) continue;

                if (IsScreenPositionOverUI(touch.position.ReadValue())) return true;
            }

            return false;
        }

        // PC fallback: check mouse pointer position
        if (Mouse.current == null) return false;
        return IsScreenPositionOverUI(Mouse.current.position.ReadValue());
    }

    /// <summary>
    /// Attempts to retrieve the first two currently active TouchControls on the touchscreen.
    /// Returns true if exactly two or more active touches are found and outputs the first two.
    /// Returns false if fewer than two active touches exist.
    /// </summary>
    public static bool TryGetTwoActiveTouches(out TouchControl firstTouch, out TouchControl secondTouch)
    {
        firstTouch  = null;
        secondTouch = null;

        if (Touchscreen.current == null) return false;

        foreach (TouchControl touch in Touchscreen.current.touches)
        {
            if (!touch.isInProgress) continue;

            if (firstTouch == null)
            {
                firstTouch = touch;
                continue;
            }

            secondTouch = touch;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Performs a UI raycast at the given screen position using EventSystem.RaycastAll.
    /// Only detects UI elements that are active, visible, and have raycastTarget enabled.
    /// Unlike IsPointerOverGameObject(), this will NOT block on invisible/inactive container panels.
    /// </summary>
    private static bool IsScreenPositionOverUI(Vector2 screenPosition)
    {
        var pointerEventData = new PointerEventData(EventSystem.current) { position = screenPosition };

        _raycastResults.Clear();
        EventSystem.current.RaycastAll(pointerEventData, _raycastResults);

        return _raycastResults.Count > 0;
    }
}
