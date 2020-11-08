using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(IInteractableMono))]
[RequireComponent(typeof(ItemProperties))]
public class CatScript : MonoBehaviour, IAreaListener, IDamageable
{
    [SerializeField]
    AudioClip[] meows;

    [SerializeField]
    Animator animator;
    [SerializeField]
    List<Transform> catPositions = new List<Transform>();

    float deathType;
    bool dead = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Hit(float damage, HitType type)
    {
        AchievementManager.SetAchievement("killedCat");
        Die(0);
    }

    public void Meow()
    {
        AudioSource aso = gameObject.AddComponent<AudioSource>();
        AudioClip meow = meows[Random.Range(0, meows.Length)];
        aso.PlayOneShot(meow);
        Destroy(aso, meow.length);
    }
    
    void Die(float type)
    {
        Destroy(GetComponent<Rigidbody>());
        foreach (Collider c in GetComponents<Collider>())
            Destroy(c);
        animator.SetFloat("deathType", type);
        animator.SetBool("dead", true);
        dead = true;
        deathType = type;
        GetComponent<IInteractableMono>().canBeInteracted = false;
        Destroy(this);
    }

    private void Update()
    {
        animator.SetFloat("deathType", deathType);
        animator.SetBool("dead", dead);
    }

    public void GetEaten()
    {
        Die(1);
    }

    public void SetWalk(bool value)
    {
        animator.SetFloat("idleType", value ? 1 : 0);
    }

    public void OnEnteredArea()
    {
        if(catPositions.Count > 0)
            transform.position = catPositions[Random.Range(0, catPositions.Count)].position;
    }

    public void OnLeftArea()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        ItemProperties props = other.GetComponent<ItemProperties>();
        if (props && props.representedItem && props.representedItem.key == "chips")
        {
            ItemProperties thisProps = GetComponent<ItemProperties>();
            IInteractableMono interactable = GetComponent<ItemProperties>();
            if(thisProps.representedItem.properties)
                thisProps.representedItem.properties.interactableName = "interact_name_20";
            AchievementManager.SetAchievement("gaveChipsToCat");
            interactable.interactableName = "interact_name_20";
            ProgressManager.isCatFriendly = true;
            Destroy(other.gameObject);
        }
    }
}
