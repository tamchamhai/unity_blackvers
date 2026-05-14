using UnityEngine;
using System.Collections.Generic;
using Blackvers.Ship.MinerShip;
using Blackvers.Data;
using Blackvers.Planet;

namespace Blackvers.Spawner
{
    /// <summary>
    /// Spawner for Miner Ships.
    /// Handles spawning and object pooling for ships.
    /// </summary>
    public partial class MinerShipSpawner : Spawner
    {
        private static MinerShipSpawner _instance;
        public static MinerShipSpawner Instance => _instance;

        [Header("Miner Ship Settings")]
        [SerializeField] protected List<MinerShipController> activeShips = new List<MinerShipController>();

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadInstance();
            this.LoadPrefabList();
        }

        /// <summary>
        /// Initializes the singleton instance for this spawner.
        /// </summary>
        protected virtual void LoadInstance()
        {
            if (MinerShipSpawner._instance != null) return;
            MinerShipSpawner._instance = this;
        }

        /// <summary>
        /// Automatically populates the prefabList from the children of 'prefabs'.
        /// </summary>
        protected virtual void LoadPrefabList()
        {
            if (this.prefabList.Count > 0) return;
            if (this.prefabs == null) return;

            foreach (Transform child in this.prefabs)
            {
                if (child.GetComponent<MinerShipController>() != null)
                {
                    this.prefabList.Add(child);
                }
            }
        }
        
        /// <summary>
        /// Spawns a ship by its data name.
        /// </summary>
        public virtual MinerShipController SpawnShip(string shipName, Vector3 position, Quaternion rotation)
        {
            Transform spawned = this.Spawn(shipName, position, rotation);
            if (spawned == null) return null;

            MinerShipController controller = spawned.GetComponent<MinerShipController>();
            if (controller != null)
            {
                this.AddActiveShip(controller);
            }

            return controller;
        }

        /// <summary>
        /// Spawns a level 1 Miner Ship for the given planet.
        /// The ship starts at the MotherShip's position.
        /// </summary>
        public virtual void SpawnMinerForPlanet(PlanetController planet)
        {
            if (planet == null) return;
            if (MotherShipController.Instance == null) return;

            MinerShipController prefab = this.GetLevel1ShipPrefab();
            if (prefab == null)
            {
                Debug.LogWarning("[MinerShipSpawner] No level 1 Miner Ship prefab found.");
                return;
            }

            Vector3 spawnPos = MotherShipController.Instance.transform.position;
            MinerShipController ship = this.SpawnShip(prefab.name, spawnPos, Quaternion.identity);
            
            if (ship != null)
            {
                ship.Initialize(planet.transform, MotherShipController.Instance.transform);
            }
        }

        protected virtual MinerShipController GetLevel1ShipPrefab()
        {
            foreach (Transform prefab in this.prefabList)
            {
                MinerShipController controller = prefab.GetComponent<MinerShipController>();
                if (controller == null || controller.minerShipData == null) continue;
                
                if (controller.minerShipData.levelRequired == 1) return controller;
            }
            return null;
        }

        protected virtual void AddActiveShip(MinerShipController ship)
        {
            if (this.activeShips.Contains(ship)) return;
            this.activeShips.Add(ship);
        }
    }
}
