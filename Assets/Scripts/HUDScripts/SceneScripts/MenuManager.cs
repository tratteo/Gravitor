using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Manager of MainMenu 
/// </summary>
public class MenuManager : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private Text highScoreText = null;
    [SerializeField] private Text gravityPointsText = null;

    private SettingsData settingsData;

    private int currentGP;
    

    void OnDisable()
    {
        GoogleIAPManager.GetInstance().UnSubscribeToProductPurchasedEvent(ProductBought);
    }

    private void Start()
    {
        InitializeData();

        AudioManager.GetInstance().NotifyAudioSettings(settingsData);
        AudioManager.GetInstance().currentMusic = AudioManager.GetInstance().PlaySound(AudioManager.MENU_SONG);

        GoogleIAPManager.GetInstance().SubscribeToProductPurchasedEvent(ProductBought);

    }

    public void QuitApp()
    {
        Application.Quit();
    }


    private void InitializeData()
    {
        SaveObject objectData;

        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SETTINGS_PATH);
        if (objectData == null)
        {
            settingsData = SaveManager.GetInstance().SavePersistentData(new SettingsData(), SaveManager.SETTINGS_PATH).GetData<SettingsData>();
        }
        else
        {
            settingsData = objectData.GetData<SettingsData>();
        }


        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.PLAYER_DATA);
        if (objectData == null)
        {
            objectData = SaveManager.GetInstance().SavePersistentData(new PlayerData(), SaveManager.PLAYER_DATA);
        }
        PlayerData playerData = objectData.GetData<PlayerData>();
        switch (playerData.playerState)
        {
            case PlayerManager.PlayerState.ASTEROID:
                break;
            case PlayerManager.PlayerState.COMET:
                break;
            default:
                playerData.playerState = PlayerManager.PlayerState.COMET;
                break;
        }
        if (playerData.deviceId == null)
        {
            playerData.deviceId = SystemInfo.deviceUniqueIdentifier;
        }
        SaveManager.GetInstance().SavePersistentData(playerData, SaveManager.PLAYER_DATA);


        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SKILLSDATA_PATH);
        if (objectData == null)
        {
            objectData = SaveManager.GetInstance().SavePersistentData(new PlayerSkillsData(), SaveManager.SKILLSDATA_PATH);
        }
        PlayerSkillsData skillData = objectData.GetData<PlayerSkillsData>();
        if (skillData.gammaRayBurstPoints == 0)
        {
            skillData.gammaRayBurstPoints = 1;
        }
        if(skillData.deviceId == null)
        {
            skillData.deviceId = SystemInfo.deviceUniqueIdentifier;
        }
        SaveManager.GetInstance().SavePersistentData(skillData, SaveManager.SKILLSDATA_PATH);


        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.ACHIEVMENTS_PATH);
        if(objectData == null)
        {
            SaveManager.GetInstance().SavePersistentData(new PlayerAchievementsData(), SaveManager.ACHIEVMENTS_PATH);
        }



        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.LEVELSDATA_PATH);
        if (objectData == null)
        {
            objectData = SaveManager.GetInstance().SavePersistentData<LevelsData>(new LevelsData(), SaveManager.LEVELSDATA_PATH);
        }
        LevelsData data = objectData.GetData<LevelsData>();
        if (data.deviceId == null)
        {
            data.deviceId = SystemInfo.deviceUniqueIdentifier;
        }
        SaveManager.GetInstance().SavePersistentData(data, SaveManager.LEVELSDATA_PATH);
        highScoreText.text = data.GetLevelHighScore(LevelsData.ENDLESS_ID).ToString();


        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.GRAVITYPOINTS_PATH);
        if (objectData == null)
        {
            objectData = SaveManager.GetInstance().SavePersistentData<int>(0, SaveManager.GRAVITYPOINTS_PATH);
        }
        currentGP = objectData.GetData<int>();
        gravityPointsText.text = "Gravity points\n" + currentGP.ToString();

    }

    private void ProductBought(string id)
    {
        switch (id)
        {
            case GoogleIAPManager.PRODUCT_GRBLVL3:

                PlayerSkillsData skillsData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SKILLSDATA_PATH).GetData<PlayerSkillsData>();
                skillsData.gammaRayBurstPoints = PlayerSkillsData.GRB_MAX_POINTS;
                SaveManager.GetInstance().SavePersistentData<PlayerSkillsData>(skillsData, SaveManager.SKILLSDATA_PATH);
                break;
        }
    }
}
