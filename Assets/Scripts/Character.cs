using UnityEngine;
using System.Collections.Generic;
public class Character : IInteractableMono, IDamageable, IAreaListener, IInitiateFunctionFromAnimator
{

    public float health = 3;

    public Animator animator;
    public Talker talker;
    public Renderer renderer;

    AnimData[] lastParameters;
    List<AnimData> boolsToSet = new List<AnimData>();

    public virtual void InitiateFunction(string name) { }

    public virtual void Hit(float damage, HitType type)
    {
        health -= damage;
        if (health <= 0)
            Die();
    }

    public virtual void SetBool(string s, bool v)
    {
        if (lastParameters != null)
        {
            foreach (AnimData d in lastParameters)
            {
                if (d.n == s)
                    d.b = v;
            }
        }
        else if (animator.isActiveAndEnabled)
            animator.SetBool(s, v);
        else
        {
            boolsToSet.Add(new AnimData(s, v));
        }
    }

    public virtual void Die() {
        foreach (Collider c in GetComponents<Collider>())
            Destroy(c);
        if (renderer)
            renderer.enabled = false;
        else
            Destroy(gameObject);
    }

    void IAreaListener.OnEnteredArea()
    {
        if(animator)
        {
            animator.SetBool("talking", false);
            if (lastParameters != null)
                SetParametersForAnimator(lastParameters);
            foreach (AnimData b in boolsToSet)
                animator.SetBool(b.n, b.b);
        }
        EnteredArea();
    }

    public virtual void EnteredArea() { }
    public virtual void LeftArea() { }


    void SetParametersForAnimator(AnimData[] data)
    {
        if (!animator)
            return;
        foreach(AnimData d in data)
        {
            switch(d.t)
            {
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(d.n, d.b);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(d.n, d.i);
                    break;
                default:
                    break;
            }
        }
    }

    void GetParametersFromAnimator()
    {
        if (!animator)
            return;
        lastParameters = new AnimData[animator.parameters.Length];
        for (int i = 0; i < animator.parameters.Length; i++)
            lastParameters[i] = new AnimData(animator.parameters[i], animator);
    }

    void IAreaListener.OnLeftArea()
    {
        if(animator)
        {
            animator.SetBool("talking", false);
            GetParametersFromAnimator();
        }
        LeftArea();
    }
}
