using UnityEngine;

public class MotherShipController : MasterMonoBehaviour
{   
    // Singleton Instance
    private static MotherShipController _instance;
    public static MotherShipController Instance => _instance;
    [SerializeField] protected MotherShipImpact motherShipImpact;
    
    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.MakeSingleton();
        this.LoadMotherShipImpact();
    }
    
    protected virtual void MakeSingleton()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            Debug.LogWarning("Duplicate MotherShipController deleted on " + gameObject.name);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject); // Keep the object alive when switching scenes
    }

    protected virtual void LoadMotherShipImpact(){
        if (this.motherShipImpact != null) return;
        this.motherShipImpact = transform.GetComponentInChildren<MotherShipImpact>();
        if (this.motherShipImpact == null)
        {
            Debug.LogWarning(transform.name + ": MotherShipImpact not found", gameObject);
        }
    }
}
