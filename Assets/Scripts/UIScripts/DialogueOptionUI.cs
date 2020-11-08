using UnityEngine;
using UnityEngine.UI;

public class DialogueOptionUI : MonoBehaviour
{
    public Text optionText;
    public Text keyText;
    public DialogueOption option;

    public void Set(DialogueOption option, int key)
    {
        keyText.text = key.ToString();
        optionText.text = LocalizationManager.GetText(option.dialogue);
        this.option = option;
    }

    public void HideKey()
    {
        keyText.transform.parent.gameObject.SetActive(false);
    }
}
