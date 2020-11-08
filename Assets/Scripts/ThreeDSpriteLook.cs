using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDSpriteLook : MonoBehaviour
{
    public bool rotateX, rotateY, rotateZ;
    public bool invert = true;

    Vector3 startEuler;

    private void Start()
    {
        startEuler = transform.rotation.eulerAngles;
    }

    private void Update()
    {
        Vector3 toPlayer = PlayerMovement.PlayerPosition - transform.position;
        //transform.Rotate(Quaternion.FromToRotation(transform.forward * (invert ? -1 : 1), toPlayer).eulerAngles);
        Vector3 target = transform.position + (invert ? -toPlayer : toPlayer);
        transform.LookAt(target);
        Vector3 currentEuler = transform.rotation.eulerAngles;

        Vector3 newEuler = startEuler;
        if (rotateX)
            newEuler.x = currentEuler.x;
        if (rotateY)
            newEuler.y = currentEuler.y;
        if (rotateZ)
            newEuler.z = currentEuler.z;

        transform.rotation = Quaternion.Euler(newEuler);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 2);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * 2);
    }
}
