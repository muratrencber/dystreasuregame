using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AchievementManager : MonoBehaviour, IInitiateFunctionFromAnimator
{
    class Achievement
    {
        public string dialogue_key;
        public bool value;

        public Achievement(string keyName, bool val = false)
        {
            dialogue_key = keyName;
            value = val;
        }
    }

    static AchievementManager manager;

    public GameObject languageScreen;
    public GameObject mainMenuScreen;
    public GameObject controlsScreen;
    public GameObject cutsceneScreen;
    public GameObject pauseScreen;
    public GameObject splashScreen;

    public Animator cutsceneManager;
    public Talker cutsceneTalker;

    bool firstPlaythrough = true;

    static float playtime;
    public static string Playtime { get {
            float hours = Mathf.FloorToInt(playtime / 3600);
            float minutes = Mathf.FloorToInt(playtime / 60);
            float seconds = Mathf.FloorToInt(playtime % 60);
            return (hours < 10 ? "0" : "") + hours.ToString() + ":" + (minutes < 10 ? "0" : "") + minutes.ToString() + ":" + (seconds < 10 ? "0" : "") + seconds.ToString();
        } }

    Dictionary<string, Achievement> achievements = new Dictionary<string, Achievement>();

    int achievedCount = 0;

    public static string AchievementStatus { get { return manager.achievedCount.ToString() + "/" + manager.achievements.Count.ToString(); } }


    bool inMainMenu;
    bool inControls;
    bool inLanguage;
    bool inGame;
    bool inPause;
    bool inSplash;

    List<Achievement> newAchievementsToShow = new List<Achievement>();

    private void Awake()
    {
        if (!manager)
        {
            manager = this;
        }
        else if (manager == this)
        {
            firstPlaythrough = false;
        }
        else
            Destroy(this.gameObject);
        mainMenuScreen = GameObject.Find("MainMenuScreen");
        controlsScreen = GameObject.Find("ControlsScreen");
        cutsceneScreen = GameObject.Find("CutsceneScreen");
        languageScreen = GameObject.Find("LanguageScreen");
        pauseScreen = GameObject.Find("PauseScreen");
        splashScreen = GameObject.Find("SplashScreen");
        cutsceneTalker = cutsceneScreen.GetComponent<Talker>();
        SceneManager.sceneLoaded += OnLoaded;
        DontDestroyOnLoad(this.gameObject);
    }

    void OnLoaded(Scene c, LoadSceneMode m)
    {
        mainMenuScreen = GameObject.Find("MainMenuScreen");
        controlsScreen = GameObject.Find("ControlsScreen");
        cutsceneScreen = GameObject.Find("CutsceneScreen");
        languageScreen = GameObject.Find("LanguageScreen");
        pauseScreen = GameObject.Find("PauseScreen");
        splashScreen = GameObject.Find("SplashScreen");
        cutsceneTalker = cutsceneScreen.GetComponent<Talker>();
        RestartGame();
    }

    public static void SetAchievement(string key)
    {
        if (manager.achievements.ContainsKey(key))
        {
            if(!manager.achievements[key].value)
            {
                manager.achievements[key].value = true;
                manager.newAchievementsToShow.Add(manager.achievements[key]);
            }
        }
        manager.achievedCount = 0;
        foreach(KeyValuePair<string, Achievement> kvp in manager.achievements)
        {
            if (kvp.Value.value)
                manager.achievedCount++;
        }
    }

    private void Update()
    {
        playtime += Time.unscaledDeltaTime;
        if(!inPause)
        {
            if(inSplash)
            {
                if (!splashScreen.activeInHierarchy)
                    ChangeScreen(languageScreen);
            }
            if (inLanguage)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    LocalizationManager.SetLanguage("en");
                    ChangeScreen(mainMenuScreen);
                }
                else if (Input.GetKeyDown(KeyCode.T))
                {
                    LocalizationManager.SetLanguage("tr");
                    ChangeScreen(mainMenuScreen);
                }
            }
            if (Input.GetKeyDown(KeyCode.Space) && firstPlaythrough)
            {
                if (inMainMenu)
                {
                    ChangeScreen(controlsScreen);
                }
                else if (inControls)
                {
                    ChangeScreen(cutsceneScreen);

                    cutsceneManager.SetBool("talking", true);
                    cutsceneManager.SetBool("startScene", true);
                    DialogueManager.SetNewDialogue(cutsceneTalker, cutsceneManager, false, false);
                }
            }

            if (inGame)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    controlsScreen.SetActive(true);
                }
                else if (Input.GetKeyUp(KeyCode.Tab))
                {
                    controlsScreen.SetActive(false);
                }

                if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Pause)) && !firstPlaythrough)
                {
                    ChangeScreen(pauseScreen);
                }

            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                ChangeScreen(null);
            else if (Input.GetKeyDown(KeyCode.X))
            {
                Application.Quit();
            }

        }
        //else if (Input.GetKeyDown(KeyCode.Escape) && inGame)
        //{
        //    pauseScreen.SetActive(true);
        //    Time.timeScale = 0f;
        //    inPause = true;
        //}
        //else if (inPause)
        //{
        //    if (Input.GetKeyDown(KeyCode.Escape))
        //    {
        //        pauseScreen.SetActive(false);
        //        //Time.timeScale = 1f;
        //        inPause = false;
        //    }
        //    else if (Input.GetKeyDown(KeyCode.X))
        //    {
        //        Application.Quit();
        //    }
        //}
    }

    void ChangeScreen(GameObject screen)
    {
        inSplash = false;
        inMainMenu = false;
        inControls = false;
        inLanguage = false;
        inPause = false;
        inGame = false;
        Time.timeScale = 1;

        if (languageScreen != screen)
            languageScreen.SetActive(false);
        else
            inLanguage = true;
        if (mainMenuScreen != screen)
            mainMenuScreen.SetActive(false);
        else
            inMainMenu = true;
        if (controlsScreen != screen)
            controlsScreen.SetActive(false);
        else
            inControls = true;
        if (pauseScreen != screen)
            pauseScreen.SetActive(false);
        else
        {
            Time.timeScale = 0;
            inPause = true;
        }
        if (splashScreen != screen)
            splashScreen.SetActive(false);
        else
            inSplash = true;
        if (cutsceneScreen != screen)
                cutsceneScreen.SetActive(false);
        if (screen)
            screen.SetActive(true);
        else
            inGame = true;


        PlayerModules.SetPlayerActive(inGame);


    }

    private void Start()
    {
        if (firstPlaythrough)
            StartGameFromStart();
        else
            RestartGame();
    }

    void StartGameFromStart()
    {
        achievements.Add("killedCat", new Achievement("achievement_01"));//You killed the cat. || Kediyi öldürdün.
        achievements.Add("killedFather", new Achievement("achievement_02"));//You killed the father of the child. What a rotten way to die. || Çocuğun babasını öldürdün. Ne kötü bir ölüm şekli.
        achievements.Add("madeShortcut", new Achievement("achievement_03"));//You made a shortcut for yourself. || Kendine bir kısayol oluşturdun.
        achievements.Add("openedSafe", new Achievement("achievement_04"));//You opened the safe. || Kasayı açtın.
        achievements.Add("gaveRingToGuard", new Achievement("achievement_05"));//You gave the ring to the security guard. || Yüzüğü güvenlik görevlisine verdin.
        achievements.Add("killedGuardWithCat", new Achievement("achievement_06"));//You made the cat slaughter the guard. || Kediye güvenlik görevlisini katlettirdin.
        achievements.Add("killedGuardWithYourGun", new Achievement("achievement_07"));//You killed the guard with a gun. || Güvenlik görevlisini bir silahla öldürdün.
        achievements.Add("killedZombie", new Achievement("achievement_08"));//You killed zombie chef. || Zombi şefi öldürdün.
        achievements.Add("usedCashRegister", new Achievement("achievement_09"));//You opened the cash register. || Yazar kasayı açtın.
        achievements.Add("fedCatToZombie", new Achievement("achievement_10"));//You fed the zombie with the cat. || Zombi şefi kediyle besledin.
        achievements.Add("fedChipsToZombie", new Achievement("achievement_11"));//You fed the zombie with potato chips. || Zombi şefi patates cipsiyle besledin.
        achievements.Add("swappedWeapons", new Achievement("achievement_12"));//You swapped weapons with the father. || Baba ile silahları değiş tokuş yaptın.
        achievements.Add("usedVentInSecurityRoom", new Achievement("achievement_13"));//You used the vent in the security room. || Güvenlik odasındaki havalandırmayı kullandın.
        achievements.Add("gaveChipsToChild", new Achievement("achievement_14"));//You gave the potato chips to the child. || Çocuğa patates cipslerini verdin.
        achievements.Add("gaveCatToChild", new Achievement("achievement_15"));//You gave the cat to the child. || Çocuğa kediyi verdin.
        achievements.Add("gaveChipsToCat", new Achievement("achievement_16"));//You fed the cat with the potate chips. || Kediyi patates cipsleriyle besledin.

        PlayerModules.SetPlayerActive(false);
        ChangeScreen(splashScreen);
        inLanguage = true;
        inMainMenu = false;
        inControls = false;
        inGame = false;
        inPause = false;
    }

    void RestartGame()
    {
        ChangeScreen(null);
        inMainMenu = false;
        inControls = false;
        inPause = false;
        inGame = true;
    }

    public static void EndGame()
    {
        manager.cutsceneManager.SetBool("success", ProgressManager.waterCount >= 9);
        manager.cutsceneManager.SetBool("endScene", true);
        DialogueManager.SetNewDialogue(manager.cutsceneTalker, manager.cutsceneManager, false, false);
        manager.ChangeScreen(manager.cutsceneScreen);
    }

    public static void OnDeath()
    {
        manager.ChangeScreen(manager.cutsceneScreen);
        AreaManager.currentArea.SetActive(false);
        PlayerModules.SetPlayerActive(false);
        manager.cutsceneManager.SetBool("dead", true);
        DialogueManager.SetNewDialogue(manager.cutsceneTalker, manager.cutsceneManager, false, false);
    }

    public void InitiateFunction(string name)
    {
        if(name == "StartEnded")
        {
            PlayerModules.SetPlayerActive(true);
            firstPlaythrough = false;
            ChangeScreen(null);
            inGame = true;
        }
        else if(name == "RestartGame")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            manager.cutsceneManager.SetBool("dead", false);
            manager.cutsceneManager.SetBool("endScene", false);
            manager.cutsceneManager.SetBool("talking", false);
            manager.cutsceneManager.SetBool("success", false);
        }
        else if(name == "ShowAchievements")
        {
            newAchievementsToShow.Insert(0, new Achievement("achievement_start"));
            if(newAchievementsToShow.Count == 1)
                newAchievementsToShow.Add(new Achievement("achievement_nothing"));
            StartCoroutine(AchievementShowRoutine());
        }
    }

    IEnumerator AchievementShowRoutine()
    {
        foreach(Achievement a in newAchievementsToShow)
        {
            DialogueManager.SetTalkerText(a.dialogue_key);
            float timer = 0;
            while(timer <= 5)
            {
                if (Input.GetKeyDown(KeyCode.Space) && cutsceneTalker.readyToSkip)
                    timer = 5;
                timer += Time.deltaTime;
                yield return null;
            }
        }
        cutsceneManager.SetTrigger("proceed");
        newAchievementsToShow.Clear();
    }
}
