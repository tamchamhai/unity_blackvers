using UnityEngine;

public class MotherShipImpact : MasterMonoBehaviour
{
    [SerializeField] protected Rigidbody2D rigidBody;
    [SerializeField] protected EdgeCollider2D edgeCollider;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadCollider();
        this.LoadRigidBody();
    }

    protected virtual void LoadCollider()
    {
        if (this.edgeCollider != null) return;
        this.edgeCollider = transform.GetComponent<EdgeCollider2D>();
        this.edgeCollider.isTrigger = true;
    }

    protected virtual void LoadRigidBody()
    {
        if   (this.rigidBody != null) return;
        this.rigidBody = transform.GetComponent<Rigidbody2D>();
        this.rigidBody.bodyType = RigidbodyType2D.Kinematic;
    }
}
