using UnityEngine;
using UnityEngine.UI;


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

    [Header("Texts")]
    [SerializeField] private Text highScoreText = null;
    [SerializeField] private Text gravityPointsText = null;
    [SerializeField] private Text gravitonsText = null;
    [SerializeField] private Text levelText = null;
    [SerializeField] private Text currentExpText = null;
    [SerializeField] private Text expNeededText = null;
    [SerializeField] private Image levelBar = null;
    [SerializeField] private Transform playerLevelTransform = null;

    [SerializeField] private LevelEffect[] levelsEffect;

    private SettingsData settingsData;


    private void Start()
    {
        InitializeData();
        AudioManager.GetInstance().NotifyAudioSettings(settingsData);
        AudioManager.GetInstance().currentMusic = AudioManager.GetInstance().PlaySound(AudioManager.MENU_SONG);
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
}
