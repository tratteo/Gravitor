using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Manager of MainMenu 
/// </summary>
public class MenuManager : MonoBehaviour
{
    public enum RewardPanelConfigure { REWARD, INFO }

    [System.Serializable]
    private struct LevelEffect
    {
        public string name;
        public GameObject effectPrefab;
        public int startLevel;
        public int endLevel;
    }

    #region Variables

    [Header("References")]
    [SerializeField] private ToastScript toast = null;
    [Header("Texts")]
    [SerializeField] private Text highScoreText = null;
    [SerializeField] private Text gravityPointsText = null;
    [SerializeField] private Text gravitonsText = null;
    [SerializeField] private Text levelText = null;
    [SerializeField] private Text currentExpText = null;
    [SerializeField] private Text expNeededText = null;
    [Header("Timed reward")]
    [SerializeField] private Text rewardTimeText = null;
    [SerializeField] private GameObject adIcon = null;
    [SerializeField] private GameObject gravitonsIcon = null;
    [SerializeField] private Text costText = null;
    [SerializeField] private Image rewardIcon = null;
    [SerializeField] private Text rewardText = null;
    [SerializeField] private Text rewardInfoText = null;

    [Header("Reward Panel")]
    [SerializeField] private GameObject rewardPanel = null;

    [Header("Available rewards panel")]
    [SerializeField] private GameObject availableRewardsPanel = null;
    [SerializeField] private Transform availableRewardsParent = null;
    [SerializeField] private GameObject rewardPrefab = null;
    [Header("Level")]
    [SerializeField] private Image levelBar = null;
    [SerializeField] private Transform playerLevelTransform = null;
    [SerializeField] private LevelEffect[] levelsEffect;


    private SettingsData settingsData;
    private ServicesData servData = null;
    private CurrencyData currencyData = null;
    private PlayerAspectData aspectData = null;

    private IEnumerator timedReward_c;
    private bool rewardReady = false;
    private List<GameObject> availableAspectRewardRefs = new List<GameObject>();
    private GameObject obtainedAspect = null;
    private Dictionary<string, GameObject> availableRewards = new Dictionary<string, GameObject>();

    #endregion

    private void SceneChanged(string name)
    {
        servData.lastAccess = System.DateTime.Now;
        SaveManager.GetInstance().SavePersistentData(servData, SaveManager.SERVICES_PATH);
    }

    private void Start()
    {
        InitializeData();

        AudioManager.GetInstance().NotifyAudioSettings(settingsData);
        AudioManager.GetInstance().currentMusic = AudioManager.GetInstance().PlaySound(AudioManager.MENU_SONG);
        PersistentPrefs.GetInstance().SetUnlockableAspects();

        timedReward_c = DailyReward_C();
        StartCoroutine(timedReward_c);

        List<PersistentPrefs.Reward> rewards = PersistentPrefs.GetInstance().GetAllRewards();
        foreach (PersistentPrefs.Reward r in rewards)
        {
            GameObject obj = Instantiate(rewardPrefab);
            obj.transform.SetParent(availableRewardsParent);
            obj.transform.localScale = new Vector3(1, 1, 1);
            switch (r.type)
            {
                case PersistentPrefs.RewardType.GRAVITY_POINT:
                    obj.GetComponentInChildren<Text>().text = r.amount.ToString();
                    obj.GetComponentsInChildren<Image>()[1].sprite = PersistentPrefs.GetInstance().gravityPointsIcon;
                    break;
                case PersistentPrefs.RewardType.GRAVITON:
                    obj.GetComponentInChildren<Text>().text = r.amount.ToString();
                    obj.GetComponentsInChildren<Image>()[1].sprite = PersistentPrefs.GetInstance().gravitonsIcon;
                    break;
                case PersistentPrefs.RewardType.ASPECT:
                    GameObject aspectRef = SharedUtilities.GetInstance().GetGameObjectsInChildrenWithTag(obj, "Preview")[0];
                    aspectRef.SetActive(true);
                    obj.GetComponentInChildren<Text>().text = aspectData.AspectStringFromId(r.id);
                    aspectRef.GetComponent<MeshRenderer>().material = PersistentPrefs.GetInstance().GetAspectWithId(r.id).UI_material;
                    availableAspectRewardRefs.Add(aspectRef);
                    break;
            }
            availableRewards.Add(r.id, obj);
        }
    }

    private void FixedUpdate()
    {
        if(availableAspectRewardRefs != null && availableRewardsPanel.activeSelf)
        {
            for (int i = 0; i < availableAspectRewardRefs.Count; i++)
            {
                availableAspectRewardRefs[i]?.transform.Rotate(new Vector3(0f, 1f, 0f) * 0.35f);
            }
        }
        if(obtainedAspect != null && rewardPanel.activeSelf)
        {
            obtainedAspect.transform.Rotate(new Vector3(0f, 1f, 0f) * 0.35f);
        }
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
        currencyData = currencyObj.GetData<CurrencyData>();
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
        aspectData = objectData.GetData<PlayerAspectData>();
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
        //yield return new WaitForSecondsRealtime(0.1f);
        while (true)
        {
            if (difference < PersistentPrefs.GetInstance().timeDelay)
            {
                gravitonsIcon.SetActive(true);
                adIcon.SetActive(false);
                rewardTimeText.text = SharedUtilities.GetInstance().GetTimeStringFromSeconds(PersistentPrefs.GetInstance().timeDelay - difference);
                rewardTimeText.gameObject.SetActive(true);
                costText.text = PersistentPrefs.GetInstance().gravitonsCost.ToString();
                rewardReady = false;
            }
            else
            {
                if (GoogleAdsManager.GetInstance().IsRewardedAdLoaded(GoogleAdsManager.RewardedAdType.TIMED_REWARD))
                {
                    gravitonsIcon.SetActive(false);
                    adIcon.SetActive(true);
                    rewardTimeText.gameObject.SetActive(false);
                    rewardReady = true;
                }
                else
                {
                    rewardTimeText.gameObject.SetActive(true);
                    rewardTimeText.text = "Loading ad...";
                    costText.text = PersistentPrefs.GetInstance().gravitonsCost.ToString();
                    rewardReady = false;
                }
            }
            yield return new WaitForSecondsRealtime(1f);
            difference = (int)(System.DateTime.Now - servData.lastRewardClaimed).TotalSeconds;
        }
    }


    public void GetDailyReward()
    {
        if(rewardReady)
        {
            rewardReady = false;
            GoogleAdsManager.GetInstance().ShowRewardedAd(GoogleAdsManager.RewardedAdType.TIMED_REWARD);
            EarnReward(true);
        }
        else
        {
            currencyData = SaveManager.GetInstance().LoadPersistentData(SaveManager.CURRENCY_PATH).GetData<CurrencyData>();
            if(currencyData.gravitons < PersistentPrefs.GetInstance().gravitonsCost)
            {
                toast.EnqueueToast("Not enough gravitons", null, 1.5f);
                return;
            }
            currencyData.gravitons -= PersistentPrefs.GetInstance().gravitonsCost;
            gravitonsText.text = currencyData.gravitons.ToString();
            SaveManager.GetInstance().SavePersistentData(currencyData, SaveManager.CURRENCY_PATH);
            EarnReward(false);
        }
    }

    public void OpenAvailableRewardsPanel(bool state)
    {
        availableRewardsPanel.SetActive(state);
    }

    public void EarnReward(bool restartTimer)
    {
        PersistentPrefs.Reward reward = PersistentPrefs.GetInstance().GetRandomReward();

        switch(reward.type)
        {
            case PersistentPrefs.RewardType.ASPECT:

                aspectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.ASPECTDATA_PATH).GetData<PlayerAspectData>();
                rewardInfoText.text = aspectData.AspectStringFromId(reward.id);
                rewardIcon.sprite = PersistentPrefs.GetInstance().transparentIcon;
                rewardText.text = "Aspect unlocked !";
                obtainedAspect = Instantiate(PersistentPrefs.GetInstance().GetAspectWithId(reward.id).prefab);
                obtainedAspect.transform.SetParent(rewardIcon.transform);
                obtainedAspect.transform.localScale = new Vector3(200, 200, 200);
                obtainedAspect.transform.localPosition = new Vector3(0, 0, -20);

                StartCoroutine(FadeInObtainedAspect());

                aspectData.UnlockAspect(reward.id);
                SaveManager.GetInstance().SavePersistentData(aspectData, SaveManager.ASPECTDATA_PATH);

                GameObject tmp = availableRewards[reward.id];
                Debug.Log(availableAspectRewardRefs.Remove(SharedUtilities.GetInstance().GetGameObjectsInChildrenWithTag(tmp, "Preview")[0]));
                Destroy(tmp);
                availableRewards.Remove(reward.id);

                PersistentPrefs.GetInstance().SetUnlockableAspects();
                //TODO reward
                break;

            case PersistentPrefs.RewardType.GRAVITON:
                rewardIcon.sprite = PersistentPrefs.GetInstance().gravitonsIcon;
                rewardInfoText.text = reward.amount.ToString();
                rewardText.text = "Gravitons earned !";
                currencyData.gravitons += reward.amount;
                SaveManager.GetInstance().SavePersistentData(currencyData, SaveManager.CURRENCY_PATH);
                break;

            case PersistentPrefs.RewardType.GRAVITY_POINT:
                rewardIcon.sprite = PersistentPrefs.GetInstance().gravityPointsIcon;
                rewardInfoText.text = reward.amount.ToString();
                rewardText.text = "Gravity Points earned !";
                currencyData.gravityPoints += reward.amount;
                SaveManager.GetInstance().SavePersistentData(currencyData, SaveManager.CURRENCY_PATH);
                break;
        }

        rewardPanel.SetActive(true);

        if (restartTimer)
        {
            servData.lastRewardClaimed = System.DateTime.Now;
            SaveManager.GetInstance().SavePersistentData(servData, SaveManager.SERVICES_PATH);
        }
    }


    private IEnumerator FadeInObtainedAspect()
    {
        float delay = rewardPanel.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length - 1;
        MeshRenderer renderer = obtainedAspect.GetComponent<MeshRenderer>();

        float transparency = 1;
        renderer.material.SetFloat("_Transparency", 1f);

        float stride = Time.fixedDeltaTime / delay;
        yield return new WaitForSecondsRealtime(1f);
        while (transparency - stride > 0)
        {
            transparency -= stride;
            renderer.material.SetFloat("_Transparency", transparency);
            yield return new WaitForFixedUpdate();
        }
        renderer.material.SetFloat("_Transparency", 0f);
    }



    public void CloseRewardPanel()
    {
        rewardPanel.SetActive(false);

        if(obtainedAspect != null)
        {
            Destroy(obtainedAspect);
            obtainedAspect = null;
        }
        UpdateView();
    }

    public void UpdateView()
    {
        gravitonsText.text = currencyData.gravitons.ToString();
        gravityPointsText.text = currencyData.gravityPoints.ToString();
    }
}
