using UnityEngine;
using System.Collections.Generic;
using Blackvers.Planet;

namespace Blackvers.Spawner
{
    /// <summary>
    /// Spawner specifically for planets.
    /// Inherits from the base Spawner to use Object Pooling.
    /// </summary>
    public partial class PlanetSpawner : Spawner
    {
        private static PlanetSpawner instance;
        public static PlanetSpawner Instance => instance;

        [Header("Random Spawn Settings")]
        [SerializeField] protected float minDistance = 15f;
        [SerializeField] protected float maxDistance = 150f;
        [SerializeField] protected float safeDistancePadding = 10f;
        [SerializeField] protected List<PlanetController> activePlanets = new List<PlanetController>();

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadInstance();
            this.LoadPrefabList();
        }

        protected virtual void LoadInstance()
        {
            if (PlanetSpawner.instance != null) return;
            PlanetSpawner.instance = this;
        }

        protected virtual void LoadPrefabList()
        {
            if (this.prefabList.Count > 0) return;
            if (this.prefabs == null) return;

            foreach (Transform child in this.prefabs)
            {
                this.prefabList.Add(child);
            }
        }

        protected override void Start()
        {
            base.Start();
            this.SpawnAllPlanets();
        }       

        /// <summary>
        /// Spawns all planets from the prefab list at random, non-overlapping positions.
        /// </summary>
        [ContextMenu("Spawn All Planets")]
        public virtual void SpawnAllPlanets()
        {
            foreach (Transform prefab in this.prefabList)
            {
                this.SpawnRandomPlanet(prefab);
            }
        }

        protected virtual void SpawnRandomPlanet(Transform prefab)
        {
            PlanetController prefabController = prefab.GetComponent<PlanetController>();
            if (prefabController == null) return;

            Vector3 spawnPosition = Vector3.zero;
            bool foundPosition = false;
            int maxAttempts = 100;

            for (int i = 0; i < maxAttempts; i++)
            {
                spawnPosition = this.GetRandomPositionInRing();
                if (!this.CheckOverlap(spawnPosition, prefabController.planetData.radius))
                {
                    foundPosition = true;
                    break;
                }
            }

            if (!foundPosition)
            {
                Debug.LogWarning($"[PlanetSpawner] Could not find a safe position for {prefab.name} after {maxAttempts} attempts.");
                return;
            }

            Transform spawned = this.Spawn(prefab.name, spawnPosition, Quaternion.identity);
            PlanetController controller = spawned.GetComponent<PlanetController>();
            if (controller != null)
            {
                controller.Initialize();
                this.activePlanets.Add(controller);
            }
        }

        protected virtual Vector3 GetRandomPositionInRing()
        {
            float angle = Random.Range(0f, Mathf.PI * 2);
            float distance = Random.Range(this.minDistance, this.maxDistance);
            
            float x = Mathf.Cos(angle) * distance;
            float y = Mathf.Sin(angle) * distance;
            
            return new Vector3(x, y, 0f);
        }

        protected virtual bool CheckOverlap(Vector3 pos, float radius)
        {
            foreach (PlanetController other in this.activePlanets)
            {
                if (other == null) continue;

                float distance = Vector3.Distance(pos, other.transform.position);
                float minSafeDistance = radius + other.planetData.radius + this.safeDistancePadding;
                
                if (distance < minSafeDistance) return true;
            }
            return false;
        }
    }
}
