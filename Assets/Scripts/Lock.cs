using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
    public string lockName;
    public AudioClip clipAtUnlock;
    public VoidEvent OnUnlocked;
}
