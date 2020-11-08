 using UnityEngine;

public class Door : IInteractableMono
{
    [SerializeField]
    string targetAreaKey;
    [SerializeField]
    int areaPosIndex;
    [SerializeField]
    bool isLadder;

    public override void OnInteract()
    {
        AreaManager.ChangeArea(targetAreaKey, areaPosIndex, isLadder);
    }
}
