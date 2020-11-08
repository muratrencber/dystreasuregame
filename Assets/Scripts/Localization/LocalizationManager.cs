using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    static LocalizationManager manager;
    Dictionary<string, string> LocalizedTexts = new Dictionary<string, string>();

    public string trString;
    public string engString;

    private void Awake()
    {
        manager = this;
        SetLanguage("en");
    }

    public static void SetLanguage(string lang)
    {
        string json = "";
#if UNITY_WEBGL
        if (lang == "tr")
            json = manager.trString;
        else if (lang == "en")
            json = manager.engString;
#else
        json = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/lang_" + lang + ".txt");
#endif
        LocalizedTexts texts = JsonUtility.FromJson<LocalizedTexts>(json) as LocalizedTexts;
        manager.LocalizedTexts.Clear();
        foreach(LocalizedText t in texts.texts)
        {
            manager.LocalizedTexts.Add(t.key, t.value);
        }
    }

    public static string GetText(string key)
    {
        string result = "ERROR";
        if (key == "")
            key = "empty";
        else if (key == " ")
            key = "space";
        if (manager.LocalizedTexts.TryGetValue(key, out result))
        {
            if (result.Contains("#achievements#"))
                result = result.Replace("#achievements#", AchievementManager.AchievementStatus);
            if (result.Contains("#playtime#"))
                result = result.Replace("#playtime#", AchievementManager.Playtime);
            if (result.Contains("#password#"))
                result = result.Replace("#password#", ProgressManager.password);
            return result;
        }
        else
            Debug.LogError("CANT FIND STRING FOR: " + key);
        return result;
    }
}
