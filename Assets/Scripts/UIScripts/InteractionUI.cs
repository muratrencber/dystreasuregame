using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    public Text nameText;
    public Text typeText;
    public RectTransform rect;
    private void Start()
    {
        nameText.text = "";
        typeText.text = "";
    }

    public void Set(IInteractableMono interactable)
    {
        nameText.text = LocalizationManager.GetText(interactable.interactableName);
        typeText.text = LocalizationManager.GetText(interactable.interactType);
    }
}
