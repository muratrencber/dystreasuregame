using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class VoidEvent : UnityEvent { }
public class CatVent : MonoBehaviour
{
    public VoidEvent OnStarted;
    [SerializeField]
    public VoidEvent OnEnteredVent;
    [SerializeField]
    public VoidEvent OnLeftVent;

    [SerializeField]
    float speed;
    public float waitDuration;

    [SerializeField]
    Transform outside;
    [SerializeField]
    Transform inside;

    bool startedGoing;
    CatScript cat;

    public List<GameObject> objectsToSpawn = new List<GameObject>();

    void OnTriggerEnter(Collider other)
    {
        CatScript newCat = other.GetComponent<CatScript>();
        if(newCat && !startedGoing && ProgressManager.isCatFriendly)
        //if (newCat && !startedGoing)
        {
            cat = newCat;
            StartGoing();
        }
    }

    void StartGoing()
    {
        if (OnStarted != null)
            OnStarted.Invoke();
        startedGoing = true;
        StopAllCoroutines();
        StartCoroutine(CatRoutine());
    }

    IEnumerator CatRoutine()
    {
        PlayerModules.SetInteract(false);
        cat.GetComponent<IInteractableMono>().canBeInteracted = false;
        yield return new WaitForSeconds(.2f);
        cat.SetWalk(true);
        Vector3 catStart = cat.transform.position;
        Vector3 toInside = inside.position - cat.transform.position;
        Vector3 toInsideWithoutY = toInside;
        toInsideWithoutY.y = 0;
        float dist = toInside.magnitude;
        float time = dist / speed;
        float fullTime = time;
        cat.Meow();
        while (time > 0)
        {
            Vector3 toCat = cat.transform.position - PlayerMovement.PlayerPosition;
            cat.GetComponent<ThreeDSpriteLook>().invert = Vector3.SignedAngle(toCat, toInsideWithoutY, Vector3.up) < 0;
            Vector3 newPos = Vector3.Lerp(catStart + toInside, catStart, time / fullTime);
            newPos.y = cat.transform.position.y;
            cat.transform.position = newPos;
            time -= Time.deltaTime;
            yield return null;
        }
        cat.transform.position = inside.position;
        if(OnEnteredVent != null)
            OnEnteredVent.Invoke();
        cat.gameObject.SetActive(false);
        yield return new WaitForSeconds(waitDuration);
        cat.gameObject.SetActive(true);
        cat.SetWalk(true);
        catStart = cat.transform.position;
        Vector3 toOutside = outside.position - cat.transform.position;
        dist = toOutside.magnitude;
        time = dist / speed;
        fullTime = time;
        while (time > 0)
        {
            Vector3 toCat = cat.transform.position - PlayerMovement.PlayerPosition;
            cat.GetComponent<ThreeDSpriteLook>().invert = Vector3.SignedAngle(toCat, toOutside, Vector3.up) < 0;
            Vector3 newPos = Vector3.Lerp(catStart + toOutside, catStart, time / fullTime);
            newPos.y = cat.transform.position.y;
            cat.transform.position = newPos;
            time -= Time.deltaTime;
            yield return null;
        }
        cat.transform.position = outside.position;
        if (OnLeftVent != null)
            OnLeftVent.Invoke();

        Vector3 toPlayer = PlayerMovement.PlayerPosition - cat.transform.position;
        Vector3 spawnPoint = cat.transform.position + toPlayer.normalized * .5f;
        cat.GetComponent<ThreeDSpriteLook>().invert = true;
        cat.SetWalk(false);
        yield return new WaitForSeconds(1f);
        cat.Meow();
        cat.GetComponent<IInteractableMono>().canBeInteracted = true;

        foreach (GameObject g in objectsToSpawn)
        {
            GameObject instance = Instantiate(g);
            instance.transform.position = spawnPoint;
            Rigidbody rb = instance.GetComponent<Rigidbody>();
            if (rb)
                rb.AddForce(toPlayer.normalized * 200);
        }
        PlayerModules.SetInteract(true);
    }
}
