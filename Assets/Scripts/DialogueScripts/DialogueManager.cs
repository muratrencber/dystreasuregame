using UnityEngine;
using System.Collections;

public class DialogueManager : MonoBehaviour
{

    [SerializeField]
    private GameObject _dialogueOptionObject;
    [SerializeField]
    private GameObject _dialogueContainer;

    public static bool currentlyTalking { get { return dialogueOptions != null && dialogueOptions.Length > 0; } }

    public float dialogueRadius = 12;

    public static GameObject DialogueOptionObject;
    public static GameObject DialogueContainer;

    private static Animator currentAnimator;
    private static DialogueOption[] dialogueOptions;
    public static Talker currentTalker;

    static bool checkForDistance;

    Coroutine OptionWait;
    private void Start()
    {
        DialogueOptionObject = _dialogueOptionObject;
        DialogueContainer = _dialogueContainer;
    }

    private void Update()
    {
        if(dialogueOptions != null && dialogueOptions.Length > 0 && OptionWait == null)
        {
            int len = dialogueOptions.Length;
            if (Input.GetKeyDown(KeyCode.Alpha1))
                OptionWait = StartCoroutine(ShowSelectedOption(0));
            if (Input.GetKeyDown(KeyCode.Alpha2) && len > 1)
                OptionWait = StartCoroutine(ShowSelectedOption(1));
            if (Input.GetKeyDown(KeyCode.Alpha3) && len > 2)
                OptionWait = StartCoroutine(ShowSelectedOption(2));
            if (Input.GetKeyDown(KeyCode.Alpha4) && len > 3)
                OptionWait = StartCoroutine(ShowSelectedOption(3));
            if (Input.GetKeyDown(KeyCode.Alpha5) && len > 4)
                OptionWait = StartCoroutine(ShowSelectedOption(4));
        }

        if(currentTalker && currentAnimator && checkForDistance)
        {
            float dist = Vector3.Distance(currentTalker.transform.position, PlayerMovement.PlayerPosition);
            if (dist > dialogueRadius || !currentAnimator.GetBool("talking"))
                FinishDialogue();
        }
    }

    public static void FinishDialogue()
    {
        if(currentAnimator && currentAnimator.gameObject.activeInHierarchy)
            currentAnimator.SetBool("talking", false);
        if (currentTalker && currentTalker.gameObject.activeInHierarchy)
        {
            currentTalker.inDialogue = false;
            currentTalker.SetTextForDialogue("");
        }
        currentAnimator = null;
        dialogueOptions = null;
        currentTalker = null;
        PlayerModules.SetPlayerActive(true);
        ClearOptions();
    }

    public static void SetDialogueOptions(Animator sender, DialogueOption[] options)
    {
        ClearOptions();
        dialogueOptions = options;

        for(int i = 0; i < Mathf.Min(5, options.Length); i++)
        {
            GameObject optionObject = Instantiate(DialogueOptionObject, DialogueContainer.transform);
            optionObject.GetComponent<DialogueOptionUI>().Set(options[i], i+1);
        }
    }

    public static void SendDialogueOption(int val)
    {
        if(currentAnimator)
        {
            currentAnimator.SetInteger("selectedOption", val);
            dialogueOptions = null;
        }
    }

    public static void SetNewDialogue(Talker t, Animator a, bool lookAtSomething = true, bool checkForDist= true)
    {
        if(lookAtSomething)
            PlayerModules.CameraLookAt(t.transform.position * .5f + a.transform.position * .5f);
        checkForDistance = checkForDist;
        FinishDialogue();
        PlayerModules.SetPlayerActive(false);
        t.inDialogue = true;
        a.SetBool("talking", true);
        currentAnimator = a;
        currentTalker = t;
    }

    public static void SetTalkerText(string text)
    {
        if (currentTalker)
            currentTalker.SetTextForDialogue(text);
    }

    static void ClearOptions()
    {
        for(int i = 0; i < DialogueContainer.transform.childCount; i++)
            Destroy(DialogueContainer.transform.GetChild(i).gameObject);
    }

    IEnumerator ShowSelectedOption(int optionVal)
    {
        GameObject optionObject = DialogueContainer.transform.GetChild(optionVal).gameObject;
        optionObject.transform.SetParent(DialogueContainer.transform.parent);

        DialogueOptionUI optUI = optionObject.GetComponent<DialogueOptionUI>();
        optUI.HideKey();
        ClearOptions();

        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Space))
                break;
            yield return null;
        }

        Destroy(optionObject);

        SendDialogueOption(optUI.option.returnValue);

        OptionWait = null;
    }
}
