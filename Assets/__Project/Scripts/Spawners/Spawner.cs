using System.Collections.Generic;
using UnityEngine;
using Blackvers.Commons;

namespace Blackvers.Spawner
{
    /// <summary>
    /// Base abstract class for all spawners.
    /// Uses the children of 'Holder' as the object pool.
    /// </summary>
    public abstract class Spawner : MasterMonoBehaviour
    {
        [Header("Spawner Settings")]
        [SerializeField] protected List<Transform> prefabList = new List<Transform>();
        [SerializeField] protected Transform prefabs;
        [SerializeField] protected Transform holder;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadPrefabs();
            this.LoadHolder();
        }

        protected virtual void LoadPrefabs()
        {
            if (this.prefabs != null) return;
            this.prefabs = transform.Find("Prefabs");
        }

        protected virtual void LoadHolder()
        {
            if (this.holder != null) return;
            this.holder = transform.Find("Holder");
        }

        /// <summary>
        /// Main method to spawn an object. 
        /// Default implementation handles pooling, positioning, and activation.
        /// </summary>
        public virtual Transform Spawn(string prefabName, Vector3 position, Quaternion rotation)
        {
            Transform newObject = this.Get(prefabName);
            
            // Guard Clause
            if (newObject == null) return null;

            newObject.SetPositionAndRotation(position, rotation);
            newObject.gameObject.SetActive(true);

            return newObject;
        }

        /// <summary>
        /// Gets an object from the holder's children (pool) or instantiates a new one.
        /// </summary>
        protected virtual Transform Get(string prefabName)
        {
            // Search for an inactive object in the holder
            foreach (Transform child in this.holder)
            {
                if (child.name == prefabName && !child.gameObject.activeSelf)
                {
                    return child;
                }
            }

            // Otherwise, find the prefab and instantiate
            Transform prefab = this.GetPrefabByName(prefabName);
            if (prefab == null)
            {
                Debug.LogWarning($"[Spawner] Prefab '{prefabName}' not found in {this.name}");
                return null;
            }

            Transform newObject = Instantiate(prefab);
            newObject.name = prefabName;
            newObject.SetParent(this.holder);
            return newObject;
        }

        protected virtual Transform GetPrefabByName(string prefabName)
        {
            foreach (Transform prefab in this.prefabList)
            {
                if (prefab.name == prefabName) return prefab;
            }
            return null;
        }

        /// <summary>
        /// Despawn an object by deactivating it. It remains in the holder for reuse.
        /// </summary>
        public virtual void Despawn(Transform obj)
        {
            if (obj == null) return;
            obj.gameObject.SetActive(false);
        }
    }
}
