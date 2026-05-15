using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using UnityEditorInternal;
using Blackvers.Data;

namespace Blackvers.EditorTools
{
    /// <summary>
    /// Utility class containing various tools for the Blackvers project.
    /// Access these tools via the 'Tools' menu in the Unity Editor.
    /// </summary>
    public static class BlackversEditorTools
    {
        private const string SORTING_LAYER_FILE_PATH = "Assets/__Project/Scripts/Commons/SortingLayerName.cs";
        private const string MINING_SHIP_SPRITE_PATH = "Assets/__Project/Sprites/Ships/Mining";
        private const string SHIP_DATA_PATH = "Assets/__Project/ScriptableObject/Ships/Mining";

        [MenuItem("Tools/Generate Sorting Layer Enum")]
        public static void GenerateSortingLayerEnum()
        {
            string[] layerNames = GetSortingLayerNames();
            string content = BuildEnumContent(layerNames);
            SaveEnumFile(content);
            
            AssetDatabase.Refresh();
            Debug.Log("<color=green>[BlackversEditorTools]</color> Successfully generated SortingLayerName enum at: " + SORTING_LAYER_FILE_PATH);
        }

        [MenuItem("Tools/Generate Miner Ship Data")]
        public static void GenerateMinerShipData()
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { MINING_SHIP_SPRITE_PATH });
            int count = 0;

            if (!Directory.Exists(SHIP_DATA_PATH))
            {
                Directory.CreateDirectory(SHIP_DATA_PATH);
            }

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                
                if (sprite == null) continue;
            Debug.Log($"<color=green>[BlackversEditorTools]</color> sprite: {sprite.name}");

                string assetPath = $"{SHIP_DATA_PATH}/{sprite.name}_Data.asset";
                
                // Skip if already exists
                if (File.Exists(assetPath)) continue;

                MinerShipData data = ScriptableObject.CreateInstance<MinerShipData>();
                data.shipName = sprite.name;
                data.shipSprite = sprite;
                
                AssetDatabase.CreateAsset(data, assetPath);
                count++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"<color=green>[BlackversEditorTools]</color> Successfully generated {count} MinerShipData assets in {SHIP_DATA_PATH}");
        }

        [MenuItem("Tools/Fix All Planets Radius")]
        public static void FixAllPlanetsRadius()
        {
            string[] guids = AssetDatabase.FindAssets("t:PlanetData");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                PlanetData data = AssetDatabase.LoadAssetAtPath<PlanetData>(path);
                
                if (data == null) continue;
                
                data.radius = 1f;
                EditorUtility.SetDirty(data);
            }
            
            AssetDatabase.SaveAssets();
            Debug.Log("<color=green>[BlackversEditorTools]</color> Successfully updated radius to 0.5 for all PlanetData assets.");
        }

        [MenuItem("Tools/Generate Default Minerals")]
        public static void GenerateDefaultMinerals()
        {
            string mineralPath = "Assets/__Project/ScriptableObject/Minerals";
            if (!Directory.Exists(mineralPath))
            {
                Directory.CreateDirectory(mineralPath);
            }

            string[] mineralNames = new string[] 
            {
                "Copper Ore", "Iron Ore", "Aluminum Ore", "Gold Ore", "Uranium", "Titanium", "Platinum", "Silicon"
            };

            float[] baseValues = new float[] { 10, 15, 20, 100, 200, 50, 150, 25 };

            for (int i = 0; i < mineralNames.Length; i++)
            {
                string rName = mineralNames[i];
                string assetName = rName.Replace(" ", "_").ToLower() + "_data.asset";
                string assetPath = $"{mineralPath}/{assetName}";

                if (File.Exists(assetPath)) continue;

                MineralData data = ScriptableObject.CreateInstance<MineralData>();
                data.mineralName = rName;
                data.baseValue = baseValues[i];
                data.description = $"Raw {rName} mined from planets.";

                AssetDatabase.CreateAsset(data, assetPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("<color=green>[BlackversEditorTools]</color> Successfully generated default MineralData assets.");
        }

        [MenuItem("Tools/Assign Default Minerals To Planets")]
        public static void AssignDefaultMineralsToPlanets()
        {
            string[] mineralGuids = AssetDatabase.FindAssets("t:MineralData");
            if (mineralGuids.Length == 0)
            {
                Debug.LogWarning("No MineralData found. Run Generate Default Minerals first.");
                return;
            }

            MineralData[] allMinerals = new MineralData[mineralGuids.Length];
            for (int i = 0; i < mineralGuids.Length; i++)
            {
                string rPath = AssetDatabase.GUIDToAssetPath(mineralGuids[i]);
                allMinerals[i] = AssetDatabase.LoadAssetAtPath<MineralData>(rPath);
            }

            string[] planetGuids = AssetDatabase.FindAssets("t:PlanetData");
            foreach (string guid in planetGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                PlanetData pData = AssetDatabase.LoadAssetAtPath<PlanetData>(path);
                if (pData == null) continue;

                // Clear and reassign 1-3 random minerals
                pData.minerals.Clear();
                int numMinerals = UnityEngine.Random.Range(1, 4);

                for (int i = 0; i < numMinerals; i++)
                {
                    MineralData randomMineral = allMinerals[UnityEngine.Random.Range(0, allMinerals.Length)];
                    
                    // Check if already has this mineral
                    if (pData.minerals.Exists(r => r.mineralData == randomMineral))
                    {
                        i--;
                        continue;
                    }

                    PlanetMineral pm = new PlanetMineral
                    {
                        mineralData = randomMineral,
                        mineRate = UnityEngine.Random.Range(1f, 5f)
                    };
                    pData.minerals.Add(pm);
                }

                EditorUtility.SetDirty(pData);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("<color=green>[BlackversEditorTools]</color> Successfully assigned random minerals to all planets.");
        }

        private static string[] GetSortingLayerNames()
        {
            SortingLayer[] layers = SortingLayer.layers;
            string[] names = new string[layers.Length];
            
            for (int i = 0; i < layers.Length; i++)
            {
                names[i] = layers[i].name;
            }
            
            return names;
        }

        private static string BuildEnumContent(string[] layerNames)
        {
            StringBuilder stringBuilder = new StringBuilder();
            
            stringBuilder.AppendLine("// THIS FILE IS AUTOMATICALLY GENERATED. DO NOT EDIT MANUALLY.");
            stringBuilder.AppendLine("namespace Blackvers.Commons");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("    public enum SortingLayerName");
            stringBuilder.AppendLine("    {");

            for (int i = 0; i < layerNames.Length; i++)
            {
                string name = layerNames[i];
                string validName = name.Replace(" ", "");
                if (string.IsNullOrEmpty(validName)) continue;
                
                stringBuilder.AppendLine($"        {validName} = {i},");
            }

            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");
            
            return stringBuilder.ToString();
        }

        private static void SaveEnumFile(string content)
        {
            string directory = Path.GetDirectoryName(SORTING_LAYER_FILE_PATH);
            
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory ?? throw new InvalidOperationException());

            File.WriteAllText(SORTING_LAYER_FILE_PATH, content);
        }
    }
}
