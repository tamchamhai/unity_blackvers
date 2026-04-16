using UnityEngine;
using System.Collections.Generic;
using Blackvers.Planet;
using Blackvers.Data;

namespace Blackvers.Spawner
{
    public partial class PlanetSpawner : MasterMonoBehaviour
    {
        private static PlanetSpawner instance;
        public static PlanetSpawner Instance => instance;

        [Header("Setup References")]
        [Tooltip("The parent object where planet prefabs will be generated.")]
        public Transform prefabs;

        [Header("Data")]
        public List<PlanetController> planetPrefabs = new List<PlanetController>();

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadInstance();
            this.LoadPrefabs();
        }

        protected virtual void LoadPrefabs()
        {
            this.prefabs = this.transform.Find("Prefabs");
        }

        protected virtual void LoadInstance()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            instance = this;
        }
    }
}
