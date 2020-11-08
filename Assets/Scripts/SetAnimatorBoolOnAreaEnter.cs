using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAnimatorBoolOnAreaEnter : MonoBehaviour, IAreaListener
{
    public bool setBool = false;
    public Animator animator;
    public string target;
    public bool value;

    public void Set(bool value)
    {
        setBool = value;
    }

    public void OnEnteredArea()
    {
        if (animator && setBool)
            animator.SetBool(target, value);
    }

    public void OnLeftArea()
    {

    }
}
