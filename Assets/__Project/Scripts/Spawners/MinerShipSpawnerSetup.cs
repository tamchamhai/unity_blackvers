#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Blackvers.Data;
using Blackvers.Commons;
using Blackvers.Ship.MinerShip;

namespace Blackvers.Spawner
{
    /// <summary>
    /// Partial class for MinerShipSpawner containing editor-only setup logic.
    /// Handles automated generation of MinerShipData assets and syncing lists.
    /// </summary>
    public partial class MinerShipSpawner
    {
        [ContextMenu("Setup Miner Ships")]
        public void Setup()
        {
            if (this.prefabs == null)
            {
                Debug.LogError("[MinerShipSpawner] Please assign a prefabs (the 'Prefabs' object) before running setup.");
                return;
            }

            this.GenerateMinerShipData();
            this.SetupShipPrefabs();
        }

        public void SetupShipPrefabs()
        {
            List<MinerShipData> allData = this.GetAllMinerShipData();
            this.ClearPrefabs();

            foreach (MinerShipData data in allData)
            {
                this.CreateShip(data);
            }

            this.SyncPrefabList();
            Debug.Log($"[MinerShipSpawner] Successfully setup {allData.Count} ships in {this.prefabs.name}");
        }

        [ContextMenu("Sync Prefab List")]
        public void SyncPrefabList()
        {
            if (this.prefabs == null) return;
            
            this.prefabList.Clear();
            foreach (Transform child in this.prefabs)
            {
                var controller = child.GetComponent<MinerShipController>();
                if (controller != null)
                {
                    this.prefabList.Add(child);
                }
            }
        }

        protected virtual List<MinerShipData> GetAllMinerShipData()
        {
            this.GenerateMinerShipData(); // Ensure data exists

            string assetPath = "Assets/__Project/ScriptableObject/Ships";
            // Restrict search to the specific ships data folder
            string[] guids = AssetDatabase.FindAssets("t:MinerShipData", new[] { assetPath });
            List<MinerShipData> allData = new List<MinerShipData>();
            
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileNameWithoutExtension(path);

                // Filter: Only include data assets that follow the miner ship naming convention
                if (!fileName.StartsWith("mining_")) continue;

                MinerShipData data = AssetDatabase.LoadAssetAtPath<MinerShipData>(path);
                if (data != null)
                {
                    allData.Add(data);
                }
            }
            
            return allData;
        }

        protected virtual void ClearPrefabs()
        {
            for (int i = this.prefabs.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(this.prefabs.GetChild(i).gameObject);
            }
        }

        protected virtual void CreateShip(MinerShipData data)
        {
            GameObject shipObj = new GameObject(data.shipName);
            shipObj.transform.SetParent(this.prefabs);
            shipObj.transform.localPosition = Vector3.zero;
            shipObj.SetActive(false);

            MinerShipController controller = shipObj.AddComponent<MinerShipController>();
            controller.minerShipData = data;

            this.CreateModel(shipObj, controller);
            this.CreateMovement(shipObj, controller);
            this.CreateImpact(shipObj, controller);

            controller.Initialize(null, null); // Initial nulls, will be set on spawn
        }

        protected virtual void CreateModel(GameObject parent, MinerShipController controller)
        {
            GameObject modelObj = new GameObject("Model");
            modelObj.transform.SetParent(parent.transform);
            modelObj.transform.localPosition = Vector3.zero;

            SpriteRenderer spriteRenderer = modelObj.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = controller.minerShipData.shipSprite;
            spriteRenderer.sortingLayerName = SortingLayerName.Ships.ToString();
            
            controller.modelRenderer = spriteRenderer;
        }

        protected virtual void CreateMovement(GameObject parent, MinerShipController controller)
        {
            GameObject movementObj = new GameObject("Movement");
            movementObj.transform.SetParent(parent.transform);
            movementObj.transform.localPosition = Vector3.zero;

            MinerShipMovement movement = movementObj.AddComponent<MinerShipMovement>();
            controller.movement = movement;
        }

        protected virtual void CreateImpact(GameObject parent, MinerShipController controller)
        {
            GameObject impactObj = new GameObject("Impact");
            impactObj.transform.SetParent(parent.transform);
            impactObj.transform.localPosition = Vector3.zero;

            // Use CircleCollider2D and auto size it
            CircleCollider2D circle = impactObj.AddComponent<CircleCollider2D>();
            
            if (controller.minerShipData != null && controller.minerShipData.shipSprite != null)
            {
                Sprite sprite = controller.minerShipData.shipSprite;
                circle.radius = Mathf.Max(sprite.bounds.extents.x, sprite.bounds.extents.y);
            }

            MinerShipImpact impact = impactObj.AddComponent<MinerShipImpact>();
            Rigidbody2D rb = impactObj.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;

            controller.impact = impact;
        }

        private void GenerateMinerShipData()
        {
            string spritePath = "Assets/__Project/Sprites/Ships/Mining";
            string assetPath = "Assets/__Project/ScriptableObject/Ships/Mining";
            
            // Ensure the directory exists
            if (!Directory.Exists(Path.Combine(Application.dataPath, "../", assetPath)))
            {
                Directory.CreateDirectory(Path.Combine(Application.dataPath, "../", assetPath));
            }

            // Get all sprites in the specified folder
            string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { spritePath });
            Debug.Log($"[MinerShipSpawner] Found {guids.Length} sprites in {spritePath} {guids}");
            bool changed = false;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileNameWithoutExtension(path);

                // Only process sprites starting with "mining_"
                if (!fileName.StartsWith("mining_")) continue;

                string assetName = fileName + "_Data.asset";
                string fullAssetPath = assetPath + "/" + assetName;

                // Check if asset already exists
                MinerShipData asset = AssetDatabase.LoadAssetAtPath<MinerShipData>(fullAssetPath);

                if (asset == null)
                {
                    asset = ScriptableObject.CreateInstance<MinerShipData>();
                    AssetDatabase.CreateAsset(asset, fullAssetPath);
                    Debug.Log($"[MinerShipSpawner] Created new MinerShipData: {fullAssetPath}");
                    changed = true;
                }

                // Update data from sprite
                if (asset.shipSprite != AssetDatabase.LoadAssetAtPath<Sprite>(path))
                {
                    asset.shipName = fileName;
                    asset.shipSprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    EditorUtility.SetDirty(asset);
                    changed = true;
                }
            }

            if (changed)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            Debug.Log("[MinerShipSpawner] Miner Ship Data generation and sync complete!");
        }
    }
}
#endif
