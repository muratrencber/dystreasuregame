using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedUIText : MonoBehaviour
{
    public string key;
    public Text text;

    private void Start()
    {
        text.text = LocalizationManager.GetText(key);
    }
}
