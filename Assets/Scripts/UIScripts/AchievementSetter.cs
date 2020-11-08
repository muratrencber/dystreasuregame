using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementSetter : MonoBehaviour
{
    public string keyName;

    public void Set()
    {
        AchievementManager.SetAchievement(keyName);
    }
}
