using UnityEngine;
using TMPro;

namespace Blackvers.UI
{
    /// <summary>
    /// Displays the current frames per second (FPS) in a TextMeshPro UI element.
    /// FPS is calculated as a smoothed rolling average to avoid excessive flickering.
    /// </summary>
    public class FpsUI : MasterMonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected TextMeshProUGUI fpsText;

        [Header("Settings")]
        [Tooltip("How often the FPS display is refreshed, in seconds.")]
        [SerializeField] protected float updateInterval = 2f;

        private float _elapsedTime;
        private int   _frameCount;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadFpsText();
        }

        protected virtual void LoadFpsText()
        {
            if (this.fpsText != null) return;
            this.fpsText = this.GetComponentInChildren<TextMeshProUGUI>();
            if (this.fpsText == null)
            {
                Debug.LogWarning($"{this.transform.name}: FPSText not found!", this.gameObject);
            }
        }

        protected override void Update()
        {
            this.UpdateFps();
        }

        /// <summary>
        /// Accumulates frame count and elapsed time each frame.
        /// When the update interval is reached, calculates the average FPS
        /// over that period and refreshes the text display.
        /// Using unscaled time so the counter remains accurate if the game is paused or slowed.
        /// </summary>
        protected virtual void UpdateFps()
        {
            if (this.fpsText == null) return;

            this._elapsedTime += Time.unscaledDeltaTime;
            this._frameCount++;

            if (this._elapsedTime < this.updateInterval) return;

            float averageFps = this._frameCount / this._elapsedTime;
            this.fpsText.text = $"{averageFps:F0} FPS";

            this._elapsedTime = 0f;
            this._frameCount  = 0;
        }
    }
}
