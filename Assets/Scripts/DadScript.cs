using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DadScript : Character
{
    [SerializeField]
    AudioClip[] grunts;
    [SerializeField]
    AudioClip pistolShot;
    [SerializeField]
    AudioClip rifleShot;

    [SerializeField]
    float rifleDamage;
    [SerializeField]
    float handgunDamage;

    [SerializeField]
    float normalStunDuration;
    [SerializeField]
    float criticStunDuration;
    [SerializeField]
    float minShootDelay;
    [SerializeField]
    float maxShootDelay;
    [SerializeField]
    LayerMask layers;

    [SerializeField]
    float dangerRadius;
    [SerializeField]
    float lethalRadius;
    [SerializeField]
    float waitDuration;
    float waitTimer;
    bool warned;
    public bool wild = true;

    [SerializeField]
    GameObject waterBottle;
    [SerializeField]
    Transform throwPos;

    float stunTimer;
    bool stunned;

    public bool attackPlayer;
    bool noOptions;

    bool talkedAboutCat;

    [SerializeField]
    GameObject ringObject;
    [SerializeField]
    GameObject rifleObject;
    [SerializeField]
    GameObject handgunObject;

    public override void OnInteract()
    {
        DialogueManager.SetNewDialogue(talker, animator);
        animator.SetBool("talking", true);
    }

    void Grunt()
    {
        AudioClip grunt = grunts[Random.Range(0, grunts.Length)];
        AudioSource s = gameObject.AddComponent<AudioSource>();
        s.PlayOneShot(grunt);
        Destroy(s, grunt.length);
    }

    void OnTriggerStay(Collider coll)
    {
        if (talker.inTalk || attackPlayer)
            return;
        ItemProperties props = coll.GetComponent<ItemProperties>();
        if (props && props.representedItem)
        {
            if (props.representedItem.key == "chips")
            {
                string[] texts = { "dad_dialogue_16", "dad_dialogue_17" };
                float[] durations = { 2, 3f };
                animator.SetBool("tradeOffered", true);
                animator.SetBool("firstTalk", false);
                wild = false;
                talker.SetTexts(texts, durations);
                Destroy(props.gameObject);
            }
            else if (!talkedAboutCat && props.representedItem.key == "cat")
            {
                string[] texts = { "dad_dialogue_18", "dad_dialogue_19", "dad_dialogue_20" };
                float[] durations = { 1, 1.5f, 1f };
                talker.SetTexts(texts, durations);
                talkedAboutCat = true;
            }
        }
    }
   

    public override void InitiateFunction(string name)
    {
        if (name == "BeEnemy")
            attackPlayer = true;
        else if(name == "GiveWater")
        {
            wild = false;
            ThrowBottle();
        }
        else if(name == "ChangeGuns")
        {
            AchievementManager.SetAchievement("swappedWeapons");
            animator.SetFloat("hasRifle", 0);
            Inventory.ReplaceItem("handgun", "rifle");
        }
        else if(name == "TalkedWithDad")
        {
            ProgressManager.TalkedWithDad();
        }
        else if(name == "CheckAll")
        {
            if (animator.GetBool("killedGuard") && !animator.GetBool("firstTalk") && animator.GetBool("waterGiven") && animator.GetBool("talkedWithGuard") && animator.GetBool("tradeFinished"))
                noOptions = true;
        }
    }

    void ThrowBottle()
    {
        if(waterBottle)
        {
            GameObject bottle = Instantiate(waterBottle);
            bottle.transform.position = throwPos.position;
            bottle.GetComponent<Rigidbody>().AddForce((transform.TransformVector(-throwPos.forward) + Vector3.up * .2f) * 500f);
            waterBottle = null;
        }
    }

    public override void Hit(float damage, HitType type)
    {
        Grunt();
        animator.SetFloat("hitType", Random.Range((int)0, (int)3));
        animator.SetTrigger("hit");
        health -= damage;
        if(health <= 0)
        {
            Die();
            return;
        }
        stunned = true;
        stunTimer = attackPlayer ? normalStunDuration : criticStunDuration;
        attackPlayer = true;
    }

    public override void Die()
    {
        foreach (Collider c in GetComponents<Collider>())
            Destroy(c);
        AchievementManager.SetAchievement("killedFather");
        ProgressManager.KilledDad();
        GameObject ring = Instantiate(ringObject);
        ring.transform.position = transform.position + (PlayerMovement.PlayerPosition - transform.position).normalized * Random.Range(.3f,1f);
        GameObject rifle = Instantiate(!animator.GetBool("tradeFinished") ? rifleObject : handgunObject);
        rifle.transform.position = transform.position + (PlayerMovement.PlayerPosition - transform.position).normalized * Random.Range(.3f, 1f);
        if(waterBottle)
        {
            GameObject bottle = Instantiate(waterBottle);
            bottle.transform.position = transform.position + (PlayerMovement.PlayerPosition - transform.position).normalized * Random.Range(.3f, 1f);
            waterBottle = null;
        }
        canBeInteracted = false;
        animator.SetTrigger("die");
    }



    private void Update()
    {
        if (health <= 0)
            return;
        if (attackPlayer)
            ProgressManager.AttackedDad();
        animator.SetBool("rude", wild);
        animator.SetFloat("isFriendly", (wild || attackPlayer) ? 0 : 1);
        talker.inDialogue = animator.GetBool("talking");
        canBeInteracted = !talker.inDialogue && !attackPlayer && !noOptions;

        if(attackPlayer)
        {
            if(stunned)
            {
                stunTimer -= Time.deltaTime;
                if (stunTimer <= 0)
                    stunned = false;
            }
            else
                TryShoot();
        }
        else if(wild)
        {
            float distance = Vector3.Distance(transform.position, PlayerMovement.PlayerPosition);
            if (distance < lethalRadius)
            {
                talker.SetText("dad_dialogue_21", 1);
                attackPlayer = true;
            }
            else if (distance < dangerRadius)
            {
                if (!warned)
                {
                    string textToShow = "";
                    int rnd = Random.Range(0, 4);
                    switch (rnd)
                    {
                        case 0:
                            textToShow = "dad_dialogue_22";
                            break;
                        case 1:
                            textToShow = "dad_dialogue_23";
                            break;
                        case 2:
                            textToShow = "dad_dialogue_24";
                            break;
                        case 3:
                            textToShow = "dad_dialogue_25";
                            break;
                    }
                    talker.SetText(textToShow, 1);
                    warned = true;
                    waitTimer = waitDuration;
                }
                else
                {
                    waitTimer -= Time.deltaTime;
                    if (waitTimer <= 0)
                        warned = false;
                }
            }
            else
                warned = false;
        }
    }

    void TryShoot()
    {
        RaycastHit hitInfo;
        if(Physics.Raycast(new Ray(transform.position, PlayerMovement.PlayerHead - transform.position), out hitInfo, 100, layers))
            if(hitInfo.collider.gameObject != PlayerMovement.PlayerObject)
                if (Physics.Raycast(new Ray(transform.position, PlayerMovement.PlayerPosition - transform.position), out hitInfo, 100, layers))
                    if (hitInfo.collider.gameObject != PlayerMovement.PlayerObject)
                        if (Physics.Raycast(new Ray(transform.position, PlayerMovement.PlayerFeet - transform.position), out hitInfo, 100, layers))
                            if (hitInfo.collider.gameObject != PlayerMovement.PlayerObject)
                                return;
        animator.SetTrigger("attack");
        AudioSource s = gameObject.AddComponent<AudioSource>();
        AudioClip c = animator.GetBool("tradeFinished") ? pistolShot : rifleShot;
        s.PlayOneShot(c);
        Destroy(s, c.length);
        PlayerVitals.Hit(animator.GetBool("tradeFinished") ? handgunDamage : rifleDamage);
        stunned = true;
        stunTimer = Mathf.Max(stunTimer, Random.Range(minShootDelay, maxShootDelay));
    }
}
