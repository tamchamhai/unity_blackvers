using UnityEngine;

public class MasterMonoBehaviour : MonoBehaviour
{
    protected virtual void Awake()
    {
        this.LoadComponents();
    }

    protected virtual void Reset()
    {
        this.LoadComponents();
    }

    protected virtual void LoadComponents()
    {
        // Override this to load components 
    }

    protected virtual void Start()
    {
        // Override this for Start logic
    }

    protected virtual void Update()
    {
        // Override this for Update logic
    }

    protected virtual void FixedUpdate()
    {
        // Override this for FixedUpdate logic
    }

    protected virtual void LateUpdate()
    {
        // Override this for LateUpdate logic
    }

    protected virtual void OnEnable()
    {
        // Override this for OnEnable logic
    }

    protected virtual void OnDisable()
    {
        // Override this for OnDisable logic
    }
}
