using UnityEngine;

namespace Blackvers.UI.Styles
{
    /// <summary>
    /// Style settings for the Left Sidebar UI component.
    /// Controls the colors of sub-tabs and text states.
    /// </summary>
    [System.Serializable]
    public class LeftSidebarStyle
    {
        [Header("Colors")]
        [SerializeField] private Color activeSubTabColor = new Color(0.12f, 0.38f, 0.18f, 1f); // Sleek Emerald
        [SerializeField] private Color inactiveSubTabColor = new Color(0.18f, 0.18f, 0.18f, 0.9f); // Dark Gray
        [SerializeField] private Color activeTextColor = new Color(0.85f, 1f, 0.85f, 1f);
        [SerializeField] private Color inactiveTextColor = new Color(0.7f, 0.7f, 0.7f, 1f);

        public Color ActiveSubTabColor => this.activeSubTabColor;
        public Color InactiveSubTabColor => this.inactiveSubTabColor;
        public Color ActiveTextColor => this.activeTextColor;
        public Color InactiveTextColor => this.inactiveTextColor;
    }
}
