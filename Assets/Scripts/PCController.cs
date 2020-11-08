using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PCController : IInteractableMono
{
    [SerializeField]
    AudioClip keyPressed;
    [SerializeField]
    AudioClip granted;
    [SerializeField]
    AudioClip denied;
    [SerializeField]
    AudioClip dot;
    [SerializeField]
    AudioClip safeOpen;

    public string password { get { return ProgressManager.password; } }
    public Text text;
    public GameObject UIObject;

    public VoidEvent OnUnlocked;

    string currentString;
    bool canEdit = true;

    public override void OnInteract()
    {
        canEdit = true;
        PlayerModules.SetPlayerActive(false);
        UIObject.SetActive(true);
        canBeInteracted = false;
    }

    void PlayKey(AudioClip c)
    {
        AudioSource s = gameObject.AddComponent<AudioSource>();
        s.PlayOneShot(c);
        Destroy(s, c.length);
    }

    public void Delete()
    {
        PlayKey(keyPressed);
        if (!canEdit || currentString.Length == 0)
            return;
        currentString = currentString.Remove(currentString.Length - 1);
        RefreshText();
    }

    public void Close()
    {
        if (!canEdit)
            return;
        PlayKey(keyPressed);
        UIObject.SetActive(false);
        PlayerModules.SetPlayerActive(true);
        canBeInteracted = currentString != password;
    }

    void CloseWithoutButton()
    {
        UIObject.SetActive(false);
        PlayerModules.SetPlayerActive(true);
        canBeInteracted = currentString != password;
    }

    public void AddChar(string newChar)
    {
        PlayKey(keyPressed);
        if (!canEdit)
            return;
        currentString += newChar;
        RefreshText();
        if(currentString.Length == password.Length)
        {
            StopAllCoroutines();
            StartCoroutine(CheckRoutine());
        }
    }

    IEnumerator CheckRoutine()
    {
        canEdit = false;
        yield return new WaitForSeconds(.5f);
        PlayKey(dot);
        text.text = ".";
        yield return new WaitForSeconds(.5f);
        PlayKey(dot);
        text.text = "..";
        yield return new WaitForSeconds(.5f);
        PlayKey(dot);
        text.text = "...";
        yield return new WaitForSeconds(.5f);
        if(currentString == password)
        {
            PlayKey(granted);
            if (OnUnlocked!= null)
                OnUnlocked.Invoke();
            text.text = LocalizationManager.GetText("computer_ui_01");
            PlayKey(safeOpen);
            yield return new WaitForSeconds(.5f);
            CloseWithoutButton();
        }
        else
        {
            PlayKey(denied);
            text.text = LocalizationManager.GetText("computer_ui_02");
            yield return new WaitForSeconds(.5f);
            canEdit = true;
            currentString = "";
            RefreshText();
        }
    }

    void RefreshText()
    {
        string final = "";
        char[] chars = currentString.ToCharArray();
        for(int i = 0; i < password.Length; i++)
        {
            string passchar = i < chars.Length ? chars[i].ToString() : "_";
            final += passchar;
            if (i < password.Length-1)
                final += " ";
        }
        text.text = final;
    }
}
