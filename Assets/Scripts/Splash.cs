using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splash : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(WaitForSplash());
    }

    IEnumerator WaitForSplash()
    {
        yield return new WaitForSeconds(3);
        this.transform.gameObject.SetActive(false);
    }
}
