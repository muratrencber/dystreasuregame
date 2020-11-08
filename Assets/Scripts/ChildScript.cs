using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildScript : Character
{
    [SerializeField]
    GameObject coin;
    [SerializeField]
    Transform givePos;

    public bool friendly = true;
    bool checkLater;
    List<Collider> collidersToCheck = new List<Collider>();

    public override void Hit(float damage, HitType type)
    {
        ProgressManager.DadProvoked();
    }

    void OnTriggerEnter(Collider coll)
    {
        if (talker.inTalk)
        {
            collidersToCheck.Add(coll);
            checkLater = true;
            return;
        }
        CheckItem(coll);
    }

    void OnTriggerExit(Collider coll)
    {
        if (collidersToCheck.Contains(coll))
            collidersToCheck.Remove(coll);
    }

    bool CheckItem(Collider coll)
    {
        if (!coll || !friendly)
            return false;
        ItemProperties props = coll.GetComponent<ItemProperties>();
        if (props && props.representedItem)
        {
            if (props.representedItem.key == "chips")
            {
                AchievementManager.SetAchievement("gaveChipsToChild");
                talker.SetText("child_talk_01", 1);
                GiveItem(coin);
                GiveItem(coin);
                Destroy(props.gameObject);
                return true;
            }
            else if (props.representedItem.key == "cat")
            {
                AchievementManager.SetAchievement("gaveCatToChild");
                string[] texts = { "child_talk_01", "child_talk_02", "child_talk_03", "child_talk_04" };
                float[] durations = { 1, 1.5f, 1.5f, 2f };
                talker.SetTexts(texts, durations);
                Destroy(props.gameObject);
                return true;
            }
        }
        return false;
    }

    void GiveItem(GameObject item, bool drop = false)
    {
        GameObject instance = Instantiate(item);
        instance.transform.position = givePos.position;
        Rigidbody rb = instance.GetComponent<Rigidbody>();
        if (rb)
            rb.AddForce((PlayerMovement.PlayerPosition - givePos.position).normalized * 200f);
    }

    void Update()
    {
        if (!friendly)
        {
            animator.SetBool("sad", true);
            return;
        }
        if(checkLater && !talker.inTalk)
        {
            for(int i = 0; i < collidersToCheck.Count; i++)
            {
                bool tr = CheckItem(collidersToCheck[i]);
                if(tr)
                {
                    for (int f = 0; f <= i; f++)
                        collidersToCheck.RemoveAt(0);
                    break;
                }
            }
            checkLater = collidersToCheck.Count > 0;
            collidersToCheck.Clear();
        }
    }

    public override void InitiateFunction(string name)
    {
        if(name == "BeSad")
        {
            friendly = false;
        }
    }

    public override void Die()
    {

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 2);
    }
}
