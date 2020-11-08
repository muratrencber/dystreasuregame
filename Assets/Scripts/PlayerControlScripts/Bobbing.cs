using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobbing : MonoBehaviour
{
    public float bobbingLength;
    public float bobbingSpeed;
    public bool bobbing;
    public bool sprinting;

    float startY;
    float targetY;
    bool up = true;

    private void Start()
    {
        startY = transform.localPosition.y;
        targetY = startY + bobbingLength;
    }

    private void Update()
    {
        if (!bobbing)
        {
            up = true;
            targetY = startY + bobbingLength;
        }
        float toTarget = (!bobbing ? startY : targetY) - transform.localPosition.y;
        float magnitude = Mathf.Min(Mathf.Abs(toTarget), bobbingSpeed *(sprinting ? 2.5f : 1) * Time.deltaTime);
        if(magnitude > 0)
            transform.Translate(Vector3.up * magnitude * Mathf.Sign(toTarget));
        if(Mathf.Abs(toTarget) <= magnitude && bobbing)
        {
            up = !up;
            targetY = up ? startY + bobbingLength : startY - bobbingLength;
        }

    }
}
