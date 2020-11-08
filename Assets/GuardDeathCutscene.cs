using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CatVent))]
public class GuardDeathCutscene : MonoBehaviour
{
    [SerializeField]
    List<GameObject> lightsToDisable = new List<GameObject>();
    [SerializeField]
    GuardScript guard;
    [SerializeField]
    GameObject blood;
    [SerializeField]
    AudioClip guardDeath;
    [SerializeField]
    AudioClip lightsOut;

    CatVent ventScript;

    private void Start()
    {
        ventScript = GetComponent<CatVent>();
    }

    private void Update()
    {
        ventScript.enabled = guard.talker.inTalk || guard.talker.inDialogue;
    }


    public void StartScene()
    {
        StartCoroutine(SceneRoutine());
    }

    IEnumerator SceneRoutine()
    {
        guard.canBeInteracted = false;
        yield return new WaitForSeconds(1f);
        ventScript.objectsToSpawn.Clear();
        if (!guard.cardGiven)
            ventScript.objectsToSpawn.Add(guard.card);
        Vector3 pos = transform.position;
        foreach (GameObject l in lightsToDisable)
        {
            pos = l.transform.position;
            l.SetActive(false);
        }
        AudioSource.PlayClipAtPoint(lightsOut, pos);
        yield return new WaitForSeconds(1);
        if(guard.health>0)
        {
            if (blood)
                blood.SetActive(true);
            AchievementManager.SetAchievement("killedGuardWithCat");
            AudioSource.PlayClipAtPoint(guardDeath, guard.transform.position);
            guard.Die();
        }
        if (guard.cardWorldObject)
            Destroy(guard.cardWorldObject);
        yield return new WaitForSeconds(4);
        foreach (GameObject l in lightsToDisable)
            l.SetActive(true);
    }
}
