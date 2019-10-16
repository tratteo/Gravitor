using UnityEngine;
using UnityEngine.UI;
using System.Collections;


/// <summary>
/// Manager of MainMenu 
/// </summary>
public class MenuManager : MonoBehaviour
{
    [System.Serializable]
    private struct LevelEffect
    {
        public string name;
        public GameObject effectPrefab;
        public int startLevel;
        public int endLevel;
    }
    [Header("References")]
    [SerializeField] private ToastScript toast = null;
    [Header("Texts")]
    [SerializeField] private Text highScoreText = null;
    [SerializeField] private Text gravityPointsText = null;
    [SerializeField] private Text gravitonsText = null;
    [SerializeField] private Text levelText = null;
    [SerializeField] private Text currentExpText = null;
    [SerializeField] private Text expNeededText = null;
    [SerializeField] private Text rewardTimeText = null;
    [SerializeField] private Text rewardText = null;
    [SerializeField] private Image levelBar = null;
    [SerializeField] private Transform playerLevelTransform = null;
    [SerializeField] private LevelEffect[] levelsEffect;


    private SettingsData settingsData;
    private ServicesData servData = null;

    private SceneLoader sceneLoader;
    private IEnumerator timedReward_c;
    private bool rewardReady = false;

    private void OnEnable()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();
        sceneLoader.SubscribeToSceneChangedEvent(SceneChanged);

    }

    private void OnDisable()
    {
        sceneLoader.UnSubscribeToSceneChangedEvent(SceneChanged);
    }

    private void SceneChanged(string name)
    {
        SaveManager.GetInstance().SavePersistentData(servData, SaveManager.SERVICES_PATH);
    }

    private void Start()
    {
        InitializeData();

        AudioManager.GetInstance().NotifyAudioSettings(settingsData);
        AudioManager.GetInstance().currentMusic = AudioManager.GetInstance().PlaySound(AudioManager.MENU_SONG);

        timedReward_c = DailyReward_C();
        StartCoroutine(timedReward_c);
    }


    public void QuitApp()
    {
        Application.Quit();
    }


    private void InitializeData()
    {
        SaveObject objectData;

        //SETTINGS DATA
        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SETTINGS_PATH);
        if (objectData == null)
        {
            settingsData = SaveManager.GetInstance().SavePersistentData(new SettingsData(), SaveManager.SETTINGS_PATH).GetData<SettingsData>();
        }
        else
        {
            settingsData = objectData.GetData<SettingsData>();
        }
        SharedUtilities.GetInstance().SetQualitySettings(settingsData.qualityLevel);


        //PLAYER DATA
        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.PLAYER_DATA);
        if (objectData == null)
        {
            objectData = SaveManager.GetInstance().SavePersistentData(new PlayerData(), SaveManager.PLAYER_DATA);
        }
        PlayerData playerData = objectData.GetData<PlayerData>();
        playerData.InitializeMissingData();
        levelText.text = playerData.playerLevel.ToString("0");
        currentExpText.text = playerData.currentExp.ToString("0");
        int expNeeded = playerData.GetLevelExpNeeded();
        expNeededText.text = expNeeded.ToString("0");
        levelBar.fillAmount = playerData.currentExp / (float)expNeeded;
        GameObject eff = GetLevelEffect(playerData.playerLevel);
        if (eff != null)
        {
            GameObject obj = Instantiate(eff, playerLevelTransform);
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.transform.localPosition = new Vector3(0, 40, 0);
        }
        SaveManager.GetInstance().SavePersistentData(playerData, SaveManager.PLAYER_DATA);


        //SKILLS DATA
        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SKILLSDATA_PATH);
        if (objectData == null)
        {
            objectData = SaveManager.GetInstance().SavePersistentData(new PlayerSkillsData(), SaveManager.SKILLSDATA_PATH);
        }
        PlayerSkillsData skillData = objectData.GetData<PlayerSkillsData>();
        skillData.InitializeMissingData();
        SaveManager.GetInstance().SavePersistentData(skillData, SaveManager.SKILLSDATA_PATH);
      

        //ACHIEVEMENTS
        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.ACHIEVMENTS_PATH);
        if(objectData == null)
        {
            SaveManager.GetInstance().SavePersistentData(new PlayerAchievementsData(), SaveManager.ACHIEVMENTS_PATH);
        }


        //LEVELS DATA
        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.LEVELSDATA_PATH);
        if (objectData == null)
        {
            objectData = SaveManager.GetInstance().SavePersistentData<LevelsData>(new LevelsData(), SaveManager.LEVELSDATA_PATH);
        }
        LevelsData data = objectData.GetData<LevelsData>();
        data.InitializeMissingData();
        SaveManager.GetInstance().SavePersistentData(data, SaveManager.LEVELSDATA_PATH);
        highScoreText.text = data.GetLevelHighScore(LevelsData.ENDLESS_ID).ToString();


        //CURRENCY DATA
        SaveObject currencyObj = SaveManager.GetInstance().LoadPersistentData(SaveManager.CURRENCY_PATH);
        if (currencyObj == null)
        {
            currencyObj = SaveManager.GetInstance().SavePersistentData<CurrencyData>(new CurrencyData(), SaveManager.CURRENCY_PATH);
        }
        CurrencyData currencyData = currencyObj.GetData<CurrencyData>();
        currencyData.InitializeMissingData();

        int currentGP = 0;
        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.GRAVITYPOINTS_PATH);
        if (objectData != null)
        {
            currentGP = objectData.GetData<int>();
            currencyData.gravityPoints = currentGP;
        }
        SaveManager.GetInstance().SavePersistentData<CurrencyData>(currencyData, SaveManager.CURRENCY_PATH);
        SaveManager.GetInstance().DeleteData(SaveManager.GRAVITYPOINTS_PATH);
        gravityPointsText.text = currencyData.gravityPoints.ToString();
        gravitonsText.text = currencyData.gravitons.ToString();

        //ASPECT DATA
        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.ASPECTDATA_PATH);
        if (objectData == null)
        {
            objectData = SaveManager.GetInstance().SavePersistentData(new PlayerAspectData(), SaveManager.ASPECTDATA_PATH);
        }
        PlayerAspectData aspectData = objectData.GetData<PlayerAspectData>();
        aspectData.InitializeMissingData();
        SaveManager.GetInstance().SavePersistentData(aspectData, SaveManager.ASPECTDATA_PATH);

        //SERVICES DATA
        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SERVICES_PATH);
        if (objectData == null)
        {
            objectData = SaveManager.GetInstance().SavePersistentData(new ServicesData(), SaveManager.SERVICES_PATH);
        }
        servData = objectData.GetData<ServicesData>();
        servData.InitializeMissingData();
        SaveManager.GetInstance().SavePersistentData(servData, SaveManager.SERVICES_PATH);
    }

    private GameObject GetLevelEffect(int level)
    {
        foreach(LevelEffect eff in levelsEffect)
        {
            if(level >= eff.startLevel)
            {
                if(eff.endLevel == -1 || level <= eff.endLevel) return eff.effectPrefab;
            }
        }
        return null;
    }

    private IEnumerator DailyReward_C()
    {
        servData.lastAccess = System.DateTime.Now;
        int difference = (int)(servData.lastAccess - servData.lastRewardClaimed).TotalSeconds;
        yield return new WaitForSecondsRealtime(0.1f);
        while (true)
        {
            servData.lastAccess = System.DateTime.Now;

            if (difference < PersistentPrefs.GetInstance().timeDelay)
            {
                rewardTimeText.text = SharedUtilities.GetInstance().GetTimeStringFromSeconds(PersistentPrefs.GetInstance().timeDelay - difference);
                difference = (int)(System.DateTime.Now - servData.lastRewardClaimed).TotalSeconds;
                rewardTimeText.gameObject.SetActive(true);
                rewardReady = false;
                rewardText.text = PersistentPrefs.GetInstance().gravitonsCost + " Gravitons";
            }
            else
            {
                if (GoogleAdsManager.GetInstance().IsRewardedAdLoaded(GoogleAdsManager.RewardedAdType.TIMED_REWARD))
                {
                    rewardTimeText.gameObject.SetActive(false);
                    rewardText.text = "Watch an ad and get the reward";
                    rewardReady = true;
                }
                else
                {
                    rewardTimeText.gameObject.SetActive(true);
                    rewardText.text = PersistentPrefs.GetInstance().gravitonsCost + " Gravitons";
                    rewardTimeText.text = "Loading ad..";
                    rewardReady = false;
                }

                difference = (int)(System.DateTime.Now - servData.lastRewardClaimed).TotalSeconds;
            }
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    public void GetDailyReward()
    {
        if(rewardReady)
        {
            rewardReady = false;
            GoogleAdsManager.GetInstance().ShowRewardedAd(GoogleAdsManager.RewardedAdType.TIMED_REWARD);
        }
        else
        {
            CurrencyData currency = SaveManager.GetInstance().LoadPersistentData(SaveManager.CURRENCY_PATH).GetData<CurrencyData>();
            if(currency.gravitons < PersistentPrefs.GetInstance().gravitonsCost)
            {
                toast.EnqueueToast("Not enough gravitons", null, 1.5f);
                return;
            }
            currency.gravitons -= PersistentPrefs.GetInstance().gravitonsCost;
            gravitonsText.text = currency.gravitons.ToString();
            SaveManager.GetInstance().SavePersistentData(currency, SaveManager.CURRENCY_PATH);
            toast.EnqueueToast("Reward paid", null, 1.5f);
        }
    }

    public void TimedRewardedEarned()
    {
        toast.EnqueueToast("Reward claimed", null, 1.5f);
        servData.lastRewardClaimed = System.DateTime.Now;
        SaveManager.GetInstance().SavePersistentData(servData, SaveManager.SERVICES_PATH);
    }
}
