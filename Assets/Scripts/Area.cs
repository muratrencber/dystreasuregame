using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    public string key;
    public GameObject areaParent;
    public List<Transform> enterances = new List<Transform>();

    public void OnEntered()
    {
        for(int i = 0; i < areaParent.transform.childCount; i++)
        {
            GameObject g = areaParent.transform.GetChild(i).gameObject;
            IAreaListener l = g.GetComponent<IAreaListener>();
            if (l != null)
                l.OnEnteredArea();
        }
    }

    public void OnLeft()
    {
        for (int i = 0; i < areaParent.transform.childCount; i++)
        {
            GameObject g = areaParent.transform.GetChild(i).gameObject;
            IAreaListener l = g.GetComponent<IAreaListener>();
            if (l != null)
                l.OnLeftArea();
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach(Transform t in enterances)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(t.position, t.position + t.forward * 1.5f);
        }
    }
}
