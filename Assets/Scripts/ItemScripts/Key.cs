using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Item
{
    public string target_name;
    public Vector3 checkScale;
    public Vector3 offset;
    public bool destroyAfterUsing = true;

    public override void OnAction1()
    {
        Unlock();
    }

    void Unlock()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position + offset + transform.forward * checkScale.z, checkScale, transform.rotation);
        Lock target = null;
        for (int i = 0; i < colliders.Length; i++)
        {
            target = colliders[i].GetComponent<Lock>();
            if(target)
                break;
        }
        if (target && target.lockName == target_name)
        {
                    if(target.OnUnlocked != null)
                        target.OnUnlocked.Invoke();
            AudioSource.PlayClipAtPoint(target.clipAtUnlock, target.transform.position);
            if (destroyAfterUsing)
                Inventory.DisableItem(indexInInventory);

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + offset + transform.forward * checkScale.z, checkScale * 2);
    }
}
