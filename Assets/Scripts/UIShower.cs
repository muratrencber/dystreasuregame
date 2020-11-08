using UnityEngine;

public class UIShower : Item
{
    [SerializeField]
    GameObject UIObject;

    public override void OnAction1()
    {
        PlayerModules.SetPlayerActive(false);
        UIObject.SetActive(true);
    }

    public void ReturnToGame()
    {
        PlayerModules.SetPlayerActive(true);
        UIObject.SetActive(false);
    }
}
