using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField]
    float mouseSensitivity = 100f;
    [SerializeField]
    Transform playerBody;
    [SerializeField]
    bool invertY;
    
    float xRotation = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= invertY ? -mouseY : mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    public void Look(Vector3 target)
    {
        Vector3 firstEuler = transform.localEulerAngles;
        transform.LookAt(target);
        Vector3 newEuler = transform.localEulerAngles;
        newEuler.x = newEuler.x % 360;
        if (newEuler.x > 90 && newEuler.x <= 270)
            xRotation = 180 - newEuler.x;
        else if (newEuler.x > 270)
            xRotation = newEuler.x - 360;
        float yRotation = newEuler.y - firstEuler.y;
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerBody.Rotate(Vector3.up * yRotation);
    }

}