using UnityEngine;

namespace Blackvers.Managers
{
    /// <summary>
    /// Central game manager responsible for global application settings applied at startup.
    /// Configured as a singleton that persists across scene loads.
    /// Currently manages:
    ///   - Target frame rate (60 FPS) to ensure smooth gameplay on mobile devices.
    ///   - VSync disabled so targetFrameRate is respected on all platforms.
    /// </summary>
    public class GameManager : MasterMonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance;

        [Header("Performance Settings")]
        [Tooltip("Target frames per second for the application. Set to -1 to uncap.")]
        [SerializeField] protected int targetFrameRate = 120;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.InitializeSingleton();
        }

        protected override void Start()
        {
            base.Start();
            this.ApplyPerformanceSettings();
        }

        /// <summary>
        /// Ensures only one GameManager instance exists across all scenes.
        /// Destroys duplicates and marks the instance as persistent.
        /// </summary>
        protected virtual void InitializeSingleton()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        /// <summary>
        /// Applies global performance settings at startup.
        /// VSync is disabled so that targetFrameRate is respected on all platforms,
        /// including Android and iOS where vSyncCount > 0 would otherwise override it.
        /// </summary>
        protected virtual void ApplyPerformanceSettings()
        {
            QualitySettings.vSyncCount  = 0;
            Application.targetFrameRate = this.targetFrameRate;
        }
    }
}
