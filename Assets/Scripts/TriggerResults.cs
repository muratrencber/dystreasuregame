using System.Collections.Generic;
using UnityEngine;

public class TriggerResults : MonoBehaviour
{
    public List<Collider> colliders = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        colliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        colliders.Remove(other);
    }
}
