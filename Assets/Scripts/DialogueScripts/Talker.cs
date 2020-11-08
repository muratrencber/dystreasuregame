using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Talker : MonoBehaviour
{
    public Text talkerText;

    public bool readyToSkip;

    string targetText;

    public bool inDialogue;
    public bool inTalk;
    public bool playSound = true;
    public AudioSource source;

    private void Start()
    {
        talkerText.text = "";
        if (!source)
            source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.clip = ProgressManager.Beep;
        source.loop = true;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && inDialogue)
        {
            if (!readyToSkip)
            {
                source.Stop();
                StopAllCoroutines();
                talkerText.text = targetText;
                StartCoroutine(SetReadyToSkip());
            }
        }
    }

    public void SetText(string text, float duration)
    {
        if(!inDialogue && !inTalk)
        {
            text = LocalizationManager.GetText(text);
            source.Stop();
            StopAllCoroutines();
            StartCoroutine(SetTextRoutine(text, duration));
        }
    }
    public void SetTexts(string[] texts, float[] durations)
    {
        if (!inDialogue && !inTalk)
        {
            source.Stop();
            StopAllCoroutines();
            StartCoroutine(SetTextsRoutine(texts, durations));
        }
    }

    public void SetTextForDialogue(string text)
    {
        talkerText.text = "";
        text = LocalizationManager.GetText(text);
        targetText = text;
        readyToSkip = false;
        inTalk = false;
        source.Stop();
        StopAllCoroutines();
        StartCoroutine(SetDialogueTextRoutine(text));
    }

    IEnumerator SetReadyToSkip()
    {
        yield return null;
        readyToSkip = true;
    }

    IEnumerator SetTextRoutine(string text, float duration)
    {
        source.Play();

        inTalk = true;
        talkerText.color = new Color(talkerText.color.r, talkerText.color.g, talkerText.color.b, 0);
        talkerText.text = text;
        yield return null;
        float size = talkerText.cachedTextGenerator.fontSizeUsedForBestFit;
        talkerText.color = new Color(talkerText.color.r, talkerText.color.g, talkerText.color.b, 1);
        talkerText.resizeTextMaxSize = (int)size;
        talkerText.text = "";
        char[] chars = text.ToCharArray();
        source.Play();
        for (int i = 0; i < chars.Length; i++)
        {
            talkerText.text += chars[i];
            yield return new WaitForSeconds(.01f);
        }
        source.Stop();
        yield return new WaitForSeconds(duration);
        talkerText.text = "";
        inTalk = false;
    }

    IEnumerator SetTextsRoutine(string[] texts, float[] durations)
    {
        inTalk = true;
        for(int i = 0; i < texts.Length; i++)
        {
            string text = texts[i];
            text = LocalizationManager.GetText(text);
            float duration = durations[i];
            talkerText.color = new Color(talkerText.color.r, talkerText.color.g, talkerText.color.b, 0);
            talkerText.text = text;
            yield return null;
            float size = talkerText.cachedTextGenerator.fontSizeUsedForBestFit;
            talkerText.color = new Color(talkerText.color.r, talkerText.color.g, talkerText.color.b, 1);
            talkerText.resizeTextMaxSize = (int)size;
            talkerText.text = "";
            char[] chars = text.ToCharArray();
            source.Play();
            for (int f = 0; f < chars.Length; f++)
            {
                talkerText.text += chars[f];
                yield return new WaitForSeconds(.01f);
            }
            source.Stop();
            yield return new WaitForSeconds(duration);
            talkerText.text = "";
        }
        inTalk = false;
    }

    IEnumerator SetDialogueTextRoutine(string text)
    {
        readyToSkip = false;
        source.Play();
        talkerText.color = new Color(talkerText.color.r, talkerText.color.g, talkerText.color.b, 0);
        talkerText.text = text;
        yield return null;
        float size = talkerText.cachedTextGenerator.fontSizeUsedForBestFit;
        talkerText.color = new Color(talkerText.color.r, talkerText.color.g, talkerText.color.b, 1);
        talkerText.resizeTextMaxSize = (int)size;
        talkerText.text = "";
        char[] chars = text.ToCharArray();
        for(int i = 0; i < chars.Length; i++)
        {
            talkerText.text += chars[i];
            yield return new WaitForSeconds(.01f);
        }
        talkerText.resizeTextMaxSize = 300;
        source.Stop();
        readyToSkip = true;
    }
}
