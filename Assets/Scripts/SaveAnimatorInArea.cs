using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimData
{
    public string n;
    public bool b;
    public int i;
    public AnimatorControllerParameterType t;

    public AnimData(AnimatorControllerParameter param, Animator anim)
    {
        n = param.name;
        t = param.type;
        switch (param.type)
        {
            case AnimatorControllerParameterType.Bool:
                b = anim.GetBool(param.name);
                break;
            case AnimatorControllerParameterType.Int:
                i = anim.GetInteger(param.name);
                break;
            default:
                break;
        }
    }

    public AnimData(string name, bool value)
    {
        n = name;
        t = AnimatorControllerParameterType.Bool;
        b = value;
    }
}

[RequireComponent(typeof(Animator))]
public class SaveAnimatorInArea : MonoBehaviour, IAreaListener
{
    AnimData[] lastParameters;
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    void IAreaListener.OnEnteredArea()
    {
        if (animator)
        {
            if (lastParameters != null)
                SetParametersForAnimator(lastParameters);
        }
    }

    void SetParametersForAnimator(AnimData[] data)
    {
        if (!animator)
            return;
        foreach (AnimData d in data)
        {
            switch (d.t)
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
        if (animator)
        {
            GetParametersFromAnimator();
        }
    }
}
