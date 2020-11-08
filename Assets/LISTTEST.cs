using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LISTTEST : MonoBehaviour
{
    public List<GameObject> g = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            List<GameObject> objectsToDelete = new List<GameObject>();
            foreach (GameObject gs in g)
            {
                if (!gs)
                    objectsToDelete.Add(gs);
            }
            foreach(GameObject gs in objectsToDelete)
            {
                g.Remove(gs);
            }
            foreach (GameObject gs in g)
            {
                if (gs)
                    print("Damn");
                else
                     print("Oh!");
            }
        }
    }
}
