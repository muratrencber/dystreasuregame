using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : MonoBehaviour
{
    [SerializeField]
    string currencyKey;
    [SerializeField]
    GameObject outputItem;
    [SerializeField]
    int price = 1;
    [SerializeField]
    int outputCount = 1;
    [SerializeField]
    Transform outputPosition;
    [SerializeField]
    int stock = 100;

    public List<GameObject> worldObjects = new List<GameObject>();
    public bool useOnlyWorldObjects;

    int current = 0;

    void OnTriggerEnter(Collider coll)
    {
        ItemProperties p = coll.GetComponent<ItemProperties>();
        if(p && p.representedItemKey == currencyKey)
        {
            if (Add())
                Destroy(p.gameObject);
        }
    }

    public void AddCoin()
    {
        Add();
    }

    public bool Add()
    {
        current++;
        List<GameObject> objectsToDelete = new List<GameObject>();
        foreach (GameObject g in worldObjects)
            if (!g)
                objectsToDelete.Add(g);
        foreach (GameObject gs in objectsToDelete)
            worldObjects.Remove(gs);
        if (current >= price)
        {
            current -= price;
            if (worldObjects.Count <= 0 && useOnlyWorldObjects)
                return false;
            for (int i = 0; i < outputCount; i++)
            {
                if(worldObjects.Count > 0)
                {
                    GameObject objectToDestroy = worldObjects[0];
                    if(objectToDestroy)
                    {
                        worldObjects.Remove(objectToDestroy);
                        Destroy(objectToDestroy);
                    }
                }
                else if(useOnlyWorldObjects)
                {
                    if (i > 0)
                        return true;
                    return false;
                }
                GameObject g = Instantiate(outputItem);
                g.transform.position = outputPosition.position;
                stock--;
            }
        }
        return true;
    }
}
