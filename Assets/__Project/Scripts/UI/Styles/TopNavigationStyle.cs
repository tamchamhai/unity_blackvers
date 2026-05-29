using UnityEngine;

namespace Blackvers.UI.Styles
{
    /// <summary>
    /// Style settings for the Top Navigation UI component.
    /// Controls the colors of primary tabs and text states.
    /// </summary>
    [System.Serializable]
    public class TopNavigationStyle
    {
        [Header("Colors")]
        [SerializeField] private Color activeTabColor = new Color(0.12f, 0.38f, 0.18f, 1f); // Sleek Emerald
        [SerializeField] private Color inactiveTabColor = new Color(0.18f, 0.18f, 0.18f, 0.9f); // Dark Gray
        [SerializeField] private Color activeTextColor = new Color(0.85f, 1f, 0.85f, 1f);
        [SerializeField] private Color inactiveTextColor = new Color(0.7f, 0.7f, 0.7f, 1f);

        public Color ActiveTabColor => this.activeTabColor;
        public Color InactiveTabColor => this.inactiveTabColor;
        public Color ActiveTextColor => this.activeTextColor;
        public Color InactiveTextColor => this.inactiveTextColor;
    }
}
