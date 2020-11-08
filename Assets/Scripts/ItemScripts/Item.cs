using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemProperties properties;
    public string key;
    public int indexInInventory;

    [SerializeField]
    AudioClip onEquipSound;

    public virtual void OnAction1() { }
    public virtual void OnAction2() { }
    public virtual void OnCountChanged() { }
    public virtual void OnEquip() { 

        if(onEquipSound)
        {
            AudioSource a = gameObject.AddComponent<AudioSource>();
            a.PlayOneShot(onEquipSound);
            Destroy(a, onEquipSound.length);
        }
    
    }
    public virtual void OnUnequip() { }
    public virtual void OnDisabledInInventory() { }
}
