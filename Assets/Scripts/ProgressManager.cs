using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressManager : MonoBehaviour
{
    static ProgressManager manager;

    public Character dad;
    public Character guard;
    public Character child;

    public Animator guardDialogueTree;
    public Animator dadDialogueTree;

    public Text waterText;

    public static int waterCount = 0;

    public static bool isCatFriendly;

    public static string password;

    [SerializeField]
    AudioClip waterClip;
    [SerializeField]
    AudioClip textBeep;

    public static AudioClip Beep {  get { return manager.textBeep; } }

    private void Awake()
    {
        isCatFriendly = false;
        waterCount = 0;
        manager = this;
    }

    private void Start()
    {
        password = Random.Range(0, 10).ToString() + Random.Range(0, 10).ToString() + Random.Range(0, 10).ToString() + Random.Range(0, 10).ToString();
    }

    public static void AddWater()
    {
        AudioSource s = manager.gameObject.AddComponent<AudioSource>();
        s.PlayOneShot(manager.waterClip);
        Destroy(s, manager.waterClip.length);
        waterCount++;
        manager.waterText.text = waterCount.ToString();
    }

    public static void DadProvoked()
    {
        if(manager.dad)
            manager.dad.InitiateFunction("BeEnemy");
        if (manager.child)
            manager.child.InitiateFunction("BeSad");
    }

    public static void AttackedDad()
    {
        if (manager.child)
            manager.child.InitiateFunction("BeSad");
    }

    public static void TalkedWithGuard()
    {
        if(manager.dadDialogueTree)
        {
            manager.dad.SetBool("talkedWithGuard", true);
        }
        if (manager.dad)
            manager.dad.interactableName = "interact_name_18";
    }
    public static void TalkedWithDad()
    {
        if (manager.guardDialogueTree)
            manager.guard.SetBool("talkedWithFather", true);
        if (manager.guard)
            manager.guard.interactableName = "interact_name_21";
    }

    public static void KilledGuard()
    {
        if (manager.dadDialogueTree)
            manager.dad.SetBool("killedGuard", true);
    }

    public static void KilledDad()
    {
        if (manager.guardDialogueTree)
            manager.guard.SetBool("killedFather", true);
    }
}
