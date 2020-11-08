using UnityEngine;

public class IInteractableMono : MonoBehaviour
{
    public string interactableName;
    public string interactType;
    public Vector3 offset;
    public bool canBeInteracted = true;
    public VoidEvent OnInteracted;
    public virtual void OnLookStart() { }
    public virtual void OnLookStay() { }
    public virtual void OnLookExit() { }

    public virtual void OnInteract() { if (OnInteracted != null) OnInteracted.Invoke(); }

    public virtual void SetInteract(bool value) { canBeInteracted = value; }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + offset, .1f);
    }
}
