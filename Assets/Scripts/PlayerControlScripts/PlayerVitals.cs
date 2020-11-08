using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVitals : MonoBehaviour
{
    static PlayerVitals vitals;

    public float health = 100;
    public Text healthText;

    public Animator hands;
    public Animator UI;

    public AudioClip hitClip;
    
    private void Awake()
    {
        vitals = this;
    }

    void RefreshHealthText()
    {
        healthText.text = health.ToString();
    }

    public static void Hit(float damage)
    {
        if (vitals.health <= 0)
            return;
        AudioSource s = vitals.gameObject.AddComponent<AudioSource>();
        s.PlayOneShot(vitals.hitClip);
        Destroy(s, vitals.hitClip.length);
        vitals.health -= damage;
        vitals.health = Mathf.Clamp(vitals.health, 0, 100);
        vitals.RefreshHealthText();
        vitals.UI.SetTrigger("hit");
        vitals.hands.SetTrigger("hit");
        if (vitals.health <= 0)
        {
            AchievementManager.OnDeath();
            return;
        }
    }

    
}
