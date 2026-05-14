using UnityEngine;
using System.Collections.Generic;
using Blackvers.Planet;

namespace Blackvers.Spawner
{
    /// <summary>
    /// Spawner specifically for planets.
    /// Inherits from the base Spawner to use Object Pooling.
    /// Manages random distribution and overlap prevention for planet generation.
    /// </summary>
    public partial class PlanetSpawner : Spawner
    {
        private static PlanetSpawner _instance;
        public static PlanetSpawner Instance => _instance;

        [Header("Random Spawn Settings")]
        [SerializeField] protected float minDistance = 15f;
        [SerializeField] protected float maxDistance = 150f;
        [SerializeField] protected float safeDistancePadding = 10f;
        [SerializeField] protected int maxSpawnAttempts = 100;
        [SerializeField] protected List<PlanetController> activePlanets = new List<PlanetController>();

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadInstance();
            this.LoadPrefabList();
        }

        protected virtual void LoadInstance()
        {
            if (PlanetSpawner._instance != null) return;
            PlanetSpawner._instance = this;
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
            this.OnStart();
        }

        protected virtual void OnStart()
        {
            this.SpawnAllPlanets();
        }

        protected virtual void SpawnAllPlanets()
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

            float realRadius = this.GetRealRadius(prefabController);
            if (!this.TryGetSafeSpawnPosition(realRadius, out var spawnPosition))
            {
                Debug.LogWarning($"[PlanetSpawner] Could not find a safe position for {prefab.name} after {this.maxSpawnAttempts} attempts.");
                return;
            }

            Transform spawned = this.Spawn(prefab.name, spawnPosition, Quaternion.identity);
            this.HandleSpawnedPlanet(spawned);
        }

        protected virtual void HandleSpawnedPlanet(Transform spawned)
        {
            if (spawned == null) return;

            PlanetController controller = spawned.GetComponent<PlanetController>();
            if (controller == null) return;

            controller.Initialize();
            this.AddActivePlanet(controller);
            this.SpawnMinerForPlanet(controller);
        }

        protected virtual void SpawnMinerForPlanet(PlanetController controller)
        {
            if (MinerShipSpawner.Instance == null) return;
            MinerShipSpawner.Instance.SpawnMinerForPlanet(controller);
        }

        protected virtual void AddActivePlanet(PlanetController controller)
        {
            if (this.activePlanets.Contains(controller)) return;
            this.activePlanets.Add(controller);
        }

        protected virtual bool TryGetSafeSpawnPosition(float radius, out Vector3 spawnPosition)
        {
            spawnPosition = Vector3.zero;

            for (var i = 0; i < this.maxSpawnAttempts; i++)
            {
                spawnPosition = this.GetRandomPositionInRing();
                if (!this.CheckOverlap(spawnPosition, radius)) return true;
            }

            return false;
        }

        protected virtual Vector3 GetRandomPositionInRing()
        {
            var angle = Random.Range(0f, Mathf.PI * 2);
            var distance = Random.Range(this.minDistance, this.maxDistance);
            
            var x = Mathf.Cos(angle) * distance;
            var y = Mathf.Sin(angle) * distance;
            
            return new Vector3(x, y, 0f);
        }

        protected virtual bool CheckOverlap(Vector3 position, float radius)
        {
            // 1. Avoid overlapping with MotherShip (Assuming MotherShip is at Vector3.zero with a safe radius)
            float motherShipSafeRadius = 15f; 
            if (Vector3.Distance(position, Vector3.zero) < (radius + motherShipSafeRadius)) return true;

            // 2. Check against other planets
            foreach (PlanetController other in this.activePlanets)
            {
                if (other == null) continue;
                if (!other.gameObject.activeInHierarchy) continue;

                float otherRadius = this.GetRealRadius(other);
                var distance = Vector3.Distance(position, other.transform.position);
                var minSafeDistance = radius + otherRadius + this.safeDistancePadding;
                
                if (distance < minSafeDistance) return true;
            }
            return false;
        }

        /// <summary>
        /// Calculates the real world radius of a planet based on its SpriteRenderer bounds.
        /// </summary>
        protected virtual float GetRealRadius(PlanetController controller)
        {
            if (controller.modelRenderer == null || controller.modelRenderer.sprite == null)
            {
                return controller.planetData != null ? controller.planetData.radius : 1f;
            }

            // Using bounds.extents.x which accounts for localScale and PPU
            return controller.modelRenderer.bounds.extents.x;
        }
    }
}
