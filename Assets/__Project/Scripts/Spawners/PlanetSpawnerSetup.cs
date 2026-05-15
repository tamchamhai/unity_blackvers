using UnityEngine;
using System.Collections.Generic;
using Blackvers.Planet;
using Blackvers.Data;
using Blackvers.Commons;

#if UNITY_EDITOR
using UnityEditor;

namespace Blackvers.Spawner
{
    /// <summary>
    /// Partial class for PlanetSpawner containing editor-only setup logic.
    /// Keeping this separate keeps the runtime class clean.
    /// </summary>
    public partial class PlanetSpawner
    {
        [ContextMenu("Sync Prefab List")]
        public void SyncPrefabList()
        {
            if (this.prefabs == null) return;
            
            this.prefabList.Clear();
            foreach (Transform child in this.prefabs)
            {
                var controller = child.GetComponent<PlanetController>();
                if (controller != null)
                {
                    this.prefabList.Add(child);
                }
            }
        }

        [ContextMenu("Setup Planets")]
        public void SetupPlanets()
        {
            if (this.prefabs == null)
            {
                Debug.LogError("[PlanetSpawner] Please assign a prefabs (the 'Prefabs' object) before running setup.");
                return;
            }

            List<PlanetData> allData = this.GetAllPlanetData();
            this.ClearPrefabs();

            foreach (PlanetData data in allData)
            {
                this.CreatePlanet(data);
            }

            this.SyncPrefabList();
            Debug.Log($"[PlanetSpawner] Successfully setup {allData.Count} planets in {this.prefabs.name}");
        }

        protected virtual List<PlanetData> GetAllPlanetData()
        {
            string[] guids = AssetDatabase.FindAssets("t:PlanetData");
            List<PlanetData> allData = new List<PlanetData>();
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                allData.Add(AssetDatabase.LoadAssetAtPath<PlanetData>(path));
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

        protected virtual void CreatePlanet(PlanetData data)
        {
            GameObject planetObj = new GameObject(data.planetName);
            planetObj.transform.SetParent(this.prefabs);
            planetObj.transform.localPosition = Vector3.zero;
            planetObj.SetActive(false);

            PlanetController controller = planetObj.AddComponent<PlanetController>();
            controller.planetData = data;

            this.CreateModel(planetObj, controller);
            this.CreateImpact(planetObj, controller);
            this.CreateMineral(planetObj, controller);

            controller.Initialize();
        }

        protected virtual void CreateMineral(GameObject parent, PlanetController controller)
        {
            GameObject mineralObj = new GameObject("Mineral");
            mineralObj.transform.SetParent(parent.transform);
            mineralObj.transform.localPosition = Vector3.zero;

            PlanetMineralManager mineralManager = mineralObj.AddComponent<PlanetMineralManager>();
            mineralManager.planetController = controller;
            controller.mineralManager = mineralManager;
        }

        protected virtual void CreateModel(GameObject parent, PlanetController controller)
        {
            GameObject modelObj = new GameObject("Model");
            modelObj.transform.SetParent(parent.transform);
            modelObj.transform.localPosition = Vector3.zero;

            SpriteRenderer spriteRenderer = modelObj.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = controller.planetData.planetSprite;
            spriteRenderer.sortingLayerName = SortingLayerName.Planets.ToString();
            
            controller.modelRenderer = spriteRenderer;
        }

        protected virtual void CreateImpact(GameObject parent, PlanetController controller)
        {
            GameObject impactObj = new GameObject("Impact");
            impactObj.transform.SetParent(parent.transform);
            impactObj.transform.localPosition = Vector3.zero;

            PlanetImpact impact = impactObj.AddComponent<PlanetImpact>();
            impact.planetController = controller;

            CircleCollider2D collider = impactObj.GetComponent<CircleCollider2D>();
            if (collider == null)
            {
                collider = impactObj.AddComponent<CircleCollider2D>();
            }
            
            collider.isTrigger = true;
            
            if (controller.planetData != null && controller.planetData.planetSprite != null)
            {
                collider.radius = controller.planetData.planetSprite.bounds.extents.x;
            }
        }
    }
}
#endif
