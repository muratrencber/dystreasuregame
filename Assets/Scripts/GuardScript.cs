using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardScript : Character
{
    [SerializeField]
    float durationBetweenTextsMin;
    [SerializeField]
    float durationBetweenTextsMax;
    [SerializeField]
    AudioClip onDeath;

    float timer;
    bool saidSomething;
    bool attacked;
    bool inRange;
    bool talkIsOver;

    public GameObject card;
    public GameObject cardWorldObject;
    public Transform giveTransform;
    public bool cardGiven = false;

    public override void OnInteract()
    {
        DialogueManager.SetNewDialogue(talker, animator);
        animator.SetBool("talking", true);
    }

    public void Attacked()
    {
        Hit(0, HitType.Melee);
    }

    public override void Hit(float damage, HitType type)
    {
        AchievementManager.SetAchievement("killedGuardWithYourGun");
        animator.SetBool("talking", false);
        talker.SetText("guard_dialogue_15", .7f);
        saidSomething = true;
        timer = Random.Range(durationBetweenTextsMin, durationBetweenTextsMax);
        attacked = true;
        if (damage > 0)
            animator.SetTrigger("hit");
        base.Hit(damage, type);
    }

    void OnTriggerStay(Collider coll)
    {
        if (talker.inTalk || talker.inDialogue)
            return;
        ItemProperties props = coll.GetComponent<ItemProperties>();
        if (props && props.representedItem)
        {
            if (props.representedItem.key == "ring" && animator.GetBool("talkedAboutFather"))
            {
                AchievementManager.SetAchievement("gaveRingToGuard");
                string[] texts = { "guard_dialogue_09", "guard_dialogue_10" };
                float[] durations = { 2, 2f };
                talkIsOver = true;
                talker.SetTexts(texts, durations);
                GiveCard();
                Destroy(props.gameObject);
            }
        }
    }

    void GiveCard()
    {
        if (!cardGiven)
            StartCoroutine(CardGiveRoutine());
    }

    IEnumerator CardGiveRoutine()
    {
        yield return new WaitForSeconds(1);
        GameObject cardObject = Instantiate(card);
        cardObject.transform.position = giveTransform.position;
        cardGiven = true;
    }

    public override void Die()
    {
        health = 0;
        animator.SetTrigger("hit");
        ProgressManager.KilledGuard();
        talker.gameObject.SetActive(false);
        if(!cardGiven)
        {
            GameObject cardWorldObject = Instantiate(card);
            cardWorldObject.transform.position = transform.position;
        }
        AudioSource.PlayClipAtPoint(onDeath, transform.position);
        canBeInteracted = false;
        animator.SetTrigger("die");
    }

    public override void OnLookStart()
    {
        inRange = true;
    }

    public override void OnLookExit()
    {
        inRange = false;
        saidSomething = false;
    }

    private void Update()
    {
        if (health <= 0)
            return;
        animator.SetFloat("isFriendly", attacked ? 0 : 1);
        canBeInteracted = !talker.inDialogue && !attacked && !talker.inTalk && !talkIsOver;
        if(attacked && !talker.inTalk)
        {
            if(!saidSomething)
            {
                saidSomething = true;
                string something = "";
                switch(Random.Range(0,3))
                {
                    case 0:
                        something = "guard_dialogue_11";
                        break;

                    case 1:
                        something = "guard_dialogue_12";
                        break;

                    case 2:
                        something = "guard_dialogue_13";
                        break;
                }
                talker.SetText(something, 1.5f);
                timer = Random.Range(durationBetweenTextsMin, durationBetweenTextsMax);
            }
            else
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                    saidSomething = false;
            }
        }
    }

    public override void InitiateFunction(string name)
    {
        if(name == "TalkedWithGuard")
        {
            ProgressManager.TalkedWithGuard();
        }
    }
}
