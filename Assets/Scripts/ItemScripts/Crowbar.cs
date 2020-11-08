using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crowbar : Item
{
    public float damage;
    public float hitDamage;
    public float durability;
    public TriggerResults results;
    public bool checkResults;
    public Animator animator;

    bool resultsChecked;

    public override void OnEquip()
    {
        base.OnEquip();
        Inventory.SetDetails(durability + "%");
    }

    public override void OnUnequip()
    {
        animator.SetBool("attacking", false);
    }

    public override void OnAction1()
    {
        if (animator.GetBool("attacking") ||durability <= 0)
            return;
        animator.SetTrigger("attack");
    }

    private void Update()
    {
        if(checkResults && !resultsChecked)
        {
            foreach(Collider coll in results.colliders)
            {
                if (!coll)
                    continue;
                IDamageable damageable = coll.GetComponent<IDamageable>();
                if(damageable != null)
                {
                    damageable.Hit(damage, HitType.Melee);
                    durability -= hitDamage;
                    durability = Mathf.Clamp(durability, 0, 100);
                    Inventory.SetDetails(durability + "%");
                    resultsChecked = true;
                    break;
                }
            }
        }

        if (!checkResults)
            resultsChecked = false;
    }
}
