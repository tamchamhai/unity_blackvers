using System.Text;
using UnityEngine;

namespace Blackvers.Commons
{
    /// <summary>
    /// Static utility class containing common helper functions for the Blackvers project.
    /// </summary>
    public static class CommonUtils
    {
        private const string CHARACTERS = "abcdefghijklmnopqrstuvwxyz0123456789";

        /// <summary>
        /// Generates a random unique ID consisting of lowercase letters and numbers.
        /// </summary>
        /// <param name="length">The length of the ID to generate. Default is 27.</param>
        /// <returns>A string representing the unique identifier.</returns>
        public static string GenerateId(int length = 27)
        {
            StringBuilder stringBuilder = new StringBuilder();
            
            for (int i = 0; i < length; i++)
            {
                int randomIndex = Random.Range(0, CHARACTERS.Length);
                stringBuilder.Append(CHARACTERS[randomIndex]);
            }
            
            return stringBuilder.ToString();
        }
    }
}
