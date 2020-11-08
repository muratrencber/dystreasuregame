using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handgun : Item
{
    public float damage;
    public int bulletCount;
    public Animator animator;
    public Vector3 checkScale;
    public Transform tip;
    public LayerMask obstacles;

    Vector3 debug_lastTip;
    Vector3 debug_lastTarget;
    Vector3 debug_lastToTarget;

    [SerializeField]
    AudioClip shotSound;
    [SerializeField]
    AudioClip emptySound;

    public override void OnEquip()
    {
        base.OnEquip();
        Inventory.SetDetails(bulletCount + "/0");
    }

    public override void OnUnequip()
    {
        animator.SetBool("shooting", false);
    }

    public override void OnAction1()
    {
        if (animator.GetBool("shooting") ||bulletCount <= 0)
        {
            if(bulletCount <= 0)
            {
                AudioSource aso = gameObject.AddComponent<AudioSource>();
                aso.PlayOneShot(emptySound);
                Destroy(aso, emptySound.length);
            }
            return;
        }
        AudioSource a = gameObject.AddComponent<AudioSource>();
        animator.SetTrigger("shoot");
        a.PlayOneShot(shotSound);
        Destroy(a, shotSound.length);
        bulletCount--;
        Inventory.SetDetails(bulletCount + "/0");
        Shoot();
    }

    void Shoot()
    {
        Vector3 toTip = tip.transform.position - PlayerMovement.PlayerPosition;
        if (Physics.Raycast(new Ray(PlayerMovement.PlayerPosition, toTip), toTip.magnitude, obstacles))
        {
            Debug.Log("inside");
            return;
        }

        Collider[] colliders = Physics.OverlapBox(transform.position + transform.forward * checkScale.z, checkScale, transform.rotation);
        GameObject targetObject = null;
        IDamageable target = null;
        float bestDist = checkScale.magnitude * 5;
        for(int i = 0; i < colliders.Length; i++)
        {
            GameObject newObject = colliders[i].gameObject;
            IDamageable newTarget = colliders[i].GetComponent<IDamageable>();
            Vector3 posWithoutY = newObject.transform.position;
            posWithoutY.y = transform.position.y;
            float newDist = Vector3.Distance(transform.position, posWithoutY);
            if (newTarget != null && (target == null || newDist < bestDist))
            {
                Debug.Log(newObject);
                targetObject = newObject;
                target = newTarget;
                bestDist = newDist;
            }
        }
        if(target != null && targetObject)
        {
            RaycastHit hitInfo;
            Vector3 pos = tip.position;
            Vector3 toTarget = targetObject.transform.position - pos;
            debug_lastTip = tip.position;
            debug_lastTarget = targetObject.transform.position;
            debug_lastToTarget = toTarget;
            if (Physics.Raycast(new Ray(pos, toTarget), out hitInfo, toTarget.magnitude*1.5f, obstacles))
            {
                if (hitInfo.collider.gameObject == targetObject || !hitInfo.collider)
                {
                    Debug.Log("Hit");
                    target.Hit(damage, HitType.Fire);
                }
                else
                    Debug.Log(hitInfo.collider.gameObject);
            }
            else
                target.Hit(damage, HitType.Fire);

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + transform.forward * checkScale.z, checkScale * 2);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(debug_lastTip, .1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(debug_lastTarget, .1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(debug_lastTip, debug_lastTip + debug_lastToTarget);
    }
}
