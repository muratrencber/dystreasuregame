using System.Collections.Generic;

[System.Serializable]
public class LocalizedText
{
    public string key;
    public string value;
    
    public LocalizedText(string key, string value)
    {
        this.key = key;
        this.value = value;
    }
}

[System.Serializable]
public class LocalizedTexts
{
    public List<LocalizedText> texts = new List<LocalizedText>();

    public void Add(LocalizedText t)
    {
        texts.Add(t);
    }

    public void Remove(LocalizedText t)
    {
        texts.Remove(t);
    }

    public void Clear()
    {
        texts.Clear();
    }
}
