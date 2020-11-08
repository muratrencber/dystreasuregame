using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YIGMATEST : MonoBehaviour
{
    public Transform parent;
    public List<Transform> starts = new List<Transform>();
    public List<GameObject> objects = new List<GameObject>();

    private void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.H))
        {
            GameObject g = Instantiate(objects[Random.Range(0, objects.Count)], starts[Random.Range(0, starts.Count)].position, Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));
            g.AddComponent<Rigidbody>();
            g.transform.SetParent(parent);
        }
    }
}
