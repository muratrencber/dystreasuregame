using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieScript : Character
{
    NavMeshAgent agent;

    [SerializeField]
    AudioClip[] growls;

    [SerializeField]
    float attackDamage;

    [SerializeField]
    float detectionRange;
    [SerializeField]
    float afterDetectionRange;
    [SerializeField]
    float attackRange;
    [SerializeField]
    LayerMask playerAndFoodLayers;

    [SerializeField]
    float attackCooldownMin;
    [SerializeField]
    float attackCooldownMax;
    [SerializeField]
    float firstAttackWait;
    float attackCooldownTimer;

    [SerializeField]
    float stunTimeMin;
    [SerializeField]
    float stunTimeMax;
    float stunTimer;

    [SerializeField]
    GameObject keyObject;

    [SerializeField]
    List<Transform> spawnPoints = new List<Transform>();

    const float textMin = 2;
    const float textMax = 5;
    float textTimer;

    Vector3 target;
    GameObject targetObject;

    public bool checkAttack;
    public TriggerResults results;
    bool attackChecked;

    public bool playerDetected;
    public bool catDetected;
    public bool chipsDetected;


    public bool detectedOnce;
    bool foodDetected { get { return chipsDetected || catDetected; } }
    bool detectedSomething { get { return foodDetected || (playerDetected && !isFriendly); } }
    public bool isFriendly = false;
    bool canAttack = false;

    bool keyGiven = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Growl()
    {
        AudioClip clip = growls[Random.Range(0, growls.Length)];
        AudioSource s = gameObject.AddComponent<AudioSource>();
        s.PlayOneShot(clip);
        Destroy(s, clip.length);
    }

    void Update()
    {
        if (health <= 0)
            return;
        if (checkAttack && !attackChecked)
        {
            foreach (Collider c in results.colliders)
            {
                if (c.gameObject == PlayerMovement.PlayerObject)
                {
                    attackChecked = true;
                    PlayerVitals.Hit(attackDamage);
                }
            }
        }
        if(!checkAttack)
            attackChecked = false;
        int side = 0;
        if(foodDetected ||playerDetected)
        {
            if (foodDetected)
            {
                Vector3 toPlayer = PlayerMovement.PlayerPosition - transform.position;
                Vector3 toTarget = target - transform.position;
                Vector3 playerToTarget = target - PlayerMovement.PlayerPosition;
                float dot = Vector3.Dot(toPlayer.normalized, toTarget.normalized);
                if (dot <= -.33f)
                    side = 0;
                else if (dot >= .33f)
                    side = 1;
                else
                {
                    if(Vector3.SignedAngle(PlayerMovement.PlayerObject.transform.forward, transform.forward, Vector3.up) < 0)
                        side = 2;
                    else
                        side = 3;
                }
            }
            else
                side = 1;
        }
        animator.SetFloat("side", side);
        if (animator.GetBool("talking") && isFriendly)
            return;
        if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0)
                agent.isStopped = false;
            else
                return;
        }
        CheckForTargets();
        float distance = Vector3.Distance(transform.position, target);
        canAttack = distance < attackRange;
        if (canAttack && detectedSomething)
        {
            attackCooldownTimer -= Time.deltaTime;
            if(attackCooldownTimer <= 0)
            {
                Attack();
                attackCooldownTimer = Random.Range(attackCooldownMin, attackCooldownMax);
            }
            agent.isStopped = true;
        }
        else if (distance <= (detectedOnce ? afterDetectionRange : detectionRange))
        {
            attackCooldownTimer = attackCooldownMin;
            agent.isStopped = false;
        }
        else
            attackCooldownTimer = firstAttackWait;

        if (detectedSomething)
        {
            textTimer -= Time.deltaTime;
            if (textTimer <= 0 && !talker.inDialogue && !talker.inTalk)
            {
                SaySomething();
                textTimer = Random.Range(textMin, textMax);
            }
        }
        else
            textTimer = 0;
    }

    public override void InitiateFunction(string name)
    {
        if(name == "GiveKey")
        {
            GiveKey();
        }
    }

    void GiveKey()
    {
        if (keyGiven)
            return;
        Growl();
        keyGiven = true;
        Vector3 toPlayer = PlayerMovement.PlayerPosition - transform.position;
        GameObject keyInstance = Instantiate(keyObject);
        keyInstance.transform.position = transform.position + toPlayer.normalized * .5f;
        Rigidbody rb = keyInstance.GetComponent<Rigidbody>();
        if (rb)
            rb.AddForce(toPlayer.normalized * 200);
    }

    void Attack()
    {
        Growl();
        if(foodDetected)
        {
            if(catDetected)
            {
                AchievementManager.SetAchievement("fedCatToZombie");
                DialogueManager.SetNewDialogue(talker, animator);
                animator.SetBool("ateCat", true);
                targetObject.GetComponent<CatScript>().GetEaten();
            }
            else
            {
                AchievementManager.SetAchievement("fedChipsToZombie");
                DialogueManager.SetNewDialogue(talker, animator);
                animator.SetBool("ateChips", true);
                Destroy(targetObject);
            }
            isFriendly = true;
        }
        else if(!isFriendly)
        {
            animator.SetTrigger("attack");
        }
    }

    public override void Hit(float damage, HitType type)
    {
        Growl();
        animator.SetBool("talking", false);
        animator.SetFloat("hitType", Random.Range((int)0, (int)3));
        animator.SetTrigger("hit");
        isFriendly = false;
        talker.SetText("zombie_dialogue_19", 1);
        agent.isStopped = true;
        stunTimer = Random.Range(stunTimeMin, stunTimeMax);

        health -= damage;
        if (health <= 0)
            Die();
    }

    public override void Die()
    {
        AchievementManager.SetAchievement("killedZombie");
        Destroy(agent);
        foreach (Collider c in GetComponents<Collider>())
            Destroy(c);
        foreach (Collider c in GetComponentsInChildren<Collider>())
            Destroy(c);
        GiveKey();
        animator.SetTrigger("die");
    }

    public override void EnteredArea()
    {
        if (health <= 0)
            return;
        transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        UpdateTarget(transform.position);
        playerDetected = false;
        catDetected = false;
        chipsDetected = false;
        detectedOnce = false;
    }
    void SaySomething()
    {
        string text = "";
        switch(Random.Range(0,4))
        {
            case 0:
                text = "zombie_dialogue_14";
                break;
            case 1:
                text = "zombie_dialogue_15";
                break;
            case 2:
                text = "zombie_dialogue_16";
                break;
            case 3:
                text = "zombie_dialogue_17";
                break;
            case 4:
                text = "zombie_dialogue_18";
                break;
        }
        talker.SetText(text, 1.5f);
    }

    void CheckForTargets()
    {

        playerDetected = false;
        chipsDetected = false;
        catDetected = false;
        Collider[] colls = Physics.OverlapSphere(transform.position, (detectedOnce ? afterDetectionRange : detectionRange), playerAndFoodLayers);
        
        foreach(Collider c in colls)
        {
            bool isPlayer = c.GetComponent<PlayerMovement>();
            if (!isPlayer)
            {
                if(c.GetComponent<CatScript>())
                {
                    targetObject = c.gameObject;
                    catDetected = true;
                    UpdateTarget(c.transform.position);
                }
                else if(!catDetected)
                {
                    targetObject = c.gameObject;
                    chipsDetected = true;
                    UpdateTarget(c.transform.position);
                }
            }
            else
                playerDetected = true;
        }
        if (!foodDetected && playerDetected && !isFriendly)
            UpdateTarget(PlayerMovement.PlayerPosition);

        if (foodDetected || playerDetected)
        {
            if (!detectedOnce)
                Growl();
            detectedOnce = true;
        }
        
    }

    void UpdateTarget(Vector3 newTarget)
    {
        agent.destination = newTarget;
        target = newTarget;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, afterDetectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
