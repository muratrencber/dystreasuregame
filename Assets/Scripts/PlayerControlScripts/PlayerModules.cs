using UnityEngine;

public class PlayerModules : MonoBehaviour
{
    static PlayerModules pmodules;
    public PlayerMovement movement;
    public MouseLook look;
    public GameObject[] modules;
    public Interact interactModule;
    public Transform playerTransform;

    bool currentlyActive;
    private void Awake()
    {
        currentlyActive = true;
        pmodules = this;
    }

    public static void SetPlayerActive(bool value, bool unlockMouse = true)
    {
        if (value == pmodules.currentlyActive)
            return;
        pmodules.currentlyActive = value;
        foreach (GameObject g in pmodules.modules)
            g.SetActive(value);
        pmodules.movement.enabled = value;
        pmodules.look.enabled = value;
        if (value)
            Cursor.lockState = CursorLockMode.Locked;
        else if(unlockMouse)
            Cursor.lockState = CursorLockMode.None;
    }

    public static void LookAt(Vector3 targetForward)
    {
        Vector3 originalRotation = pmodules.playerTransform.rotation.eulerAngles;
        pmodules.playerTransform.LookAt(pmodules.playerTransform.position + targetForward);
        Vector3 newRotation = pmodules.playerTransform.rotation.eulerAngles;
        originalRotation.y = newRotation.y;
        pmodules.playerTransform.rotation = Quaternion.Euler(originalRotation);
    }
    public static void CameraLookAt(Vector3 target)
    {
        pmodules.look.Look(target);
    }

    public static void SetPlayerPosition(Vector3 pos)
    {
        pmodules.movement.Teleport(pos);
    }

    public static void SetInteract(bool value)
    {
        pmodules.interactModule.enabled = value;
    }

}
