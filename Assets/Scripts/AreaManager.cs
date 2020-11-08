using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaManager : MonoBehaviour
{
    static AreaManager manager;

    [SerializeField]
    List<Area> areas = new List<Area>();
    [SerializeField]
    Image blackScreen;

    public static GameObject currentArea { get { return manager.areas[manager.currentAreaIndex].gameObject; } }

    [SerializeField]
    float fadeDuration;
    [SerializeField]
    float waitDuration;

    [SerializeField]
    AudioClip doorChange;
    [SerializeField]
    AudioClip ladderChange;

    public int currentAreaIndex;

    private void Awake()
    {
        manager = this;
    }

    public static void ChangeArea(string key, int index, bool isLadder = false)
    {
        int areaIndex = -1;
        for(int i = 0; i < manager.areas.Count; i++)
        {
            Area a = manager.areas[i];
            if(a.key == key)
            {
                areaIndex = i;
                break;
            }
        }
        if(areaIndex >= 0 && areaIndex != manager.currentAreaIndex)
        {
            Area oldArea = manager.areas[manager.currentAreaIndex];
            Area newArea = manager.areas[areaIndex];
            manager.currentAreaIndex = areaIndex;
            manager.StartCoroutine(manager.ChangeAreaRoutine(oldArea, newArea, index, isLadder));
        }
    }

    IEnumerator ChangeAreaRoutine(Area from, Area to, int entranceIndex, bool isLadder)
    {
        AudioSource s = Camera.main.gameObject.AddComponent<AudioSource>();
        AudioClip c = isLadder ? ladderChange : doorChange;
        s.PlayOneShot(c);
        Destroy(s, c.length);
        from.OnLeft();
        PlayerModules.SetPlayerActive(false);
        yield return null;
        float timer = 0;
        while(timer <= fadeDuration)
        {
            blackScreen.color = Color.Lerp(new Color(0, 0, 0, 0), Color.black, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        to.areaParent.SetActive(true);
        yield return null;
        to.OnEntered();
        yield return new WaitForSeconds(waitDuration);
        s.volume = 0;
        Transform targetEntrance = to.enterances[entranceIndex];
        PlayerModules.SetPlayerPosition(targetEntrance.position);
        PlayerModules.LookAt(targetEntrance.forward);
        from.areaParent.SetActive(false);
        blackScreen.color = new Color(0, 0, 0, 0);
        PlayerModules.SetPlayerActive(true);
        
    }
}
