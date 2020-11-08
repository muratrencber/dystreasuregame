using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour, IDamageable
{
    public float health;
    public bool takeDamageFromMelee = true;
    public Animator animator;
    public VoidEvent OnBreak;

    public bool canBeHit = true;
    [SerializeField]
    AudioClip soundOnBreak;
    [SerializeField]
    AudioClip hitSound;
    float damageTaken;

    public void Hit(float damage, HitType type)
    {
        if (!canBeHit)
            return;
        if (!takeDamageFromMelee && type == HitType.Melee)
            return;
        damageTaken += damage;
        if(damageTaken >= health)
        {
            if (OnBreak != null)
                OnBreak.Invoke();
            AudioSource.PlayClipAtPoint(soundOnBreak, transform.position);
            Destroy(gameObject);
        }
        else if(animator)
        {
            if (hitSound)
                AudioSource.PlayClipAtPoint(hitSound, transform.position);
            animator.SetFloat("health", (health - damageTaken) * 10 / health);
        }
    }

    public void SetInvincible(bool value)
    {
        canBeHit = !value;
    }
}
