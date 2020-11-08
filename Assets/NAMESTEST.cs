using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NAMESTEST : MonoBehaviour
{
    private void Start()
    {
        IInteractableMono[] g = (IInteractableMono[])Resources.FindObjectsOfTypeAll(typeof(IInteractableMono));
        foreach(IInteractableMono iim in g)
        {
            Debug.Log("isim: " + iim.interactableName);
            Debug.Log("tip: " + iim.interactType);
        }
    }
}
