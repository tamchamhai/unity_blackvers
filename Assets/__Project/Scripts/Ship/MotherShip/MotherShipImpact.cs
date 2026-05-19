using UnityEngine;

public class MotherShipImpact : MasterMonoBehaviour
{
    [SerializeField] protected Rigidbody2D rigidBody;
    [SerializeField] protected BoxCollider2D boxCollider;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadBoxCollider();
        this.LoadRigidBody();
    }

    protected virtual void LoadBoxCollider()
    {
        if (this.boxCollider != null) return;
        this.boxCollider = transform.GetComponent<BoxCollider2D>();
        this.boxCollider.isTrigger = true;
    }

    protected virtual void LoadRigidBody()
    {
        if   (this.rigidBody != null) return;
        this.rigidBody = transform.GetComponent<Rigidbody2D>();
        this.rigidBody.bodyType = RigidbodyType2D.Kinematic;
    }

}
