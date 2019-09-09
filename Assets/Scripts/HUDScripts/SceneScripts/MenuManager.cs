using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;


/// <summary>
/// Manager of MainMenu 
/// </summary>
public class MenuManager : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private Text highScoreText = null;
    [SerializeField] private Text gravityPointsText = null;
    [Header("Toast")]
    [SerializeField] private ToastScript toast = null;

    private SettingsData settingsData;

    private int currentGP;
    private int adBonusGP;
    private bool isAdLoading = false;
    

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
        adBonusGP = 15000;
        PlayerData playerData = objectData.GetData<PlayerData>();
        switch (playerData.playerState)
        {
            case PlayerManager.PlayerState.ASTEROID:
                adBonusGP = 15000;
                break;
            case PlayerManager.PlayerState.COMET:
                adBonusGP = 60000;
                break;
            default:
                playerData.playerState = PlayerManager.PlayerState.COMET;
                SaveManager.GetInstance().SavePersistentData(playerData, SaveManager.PLAYER_DATA);
                adBonusGP = 60000;
                break;
        }

        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SKILLSDATA_PATH);
        if (objectData == null)
        {
            objectData = SaveManager.GetInstance().SavePersistentData(new PlayerSkillsData(), SaveManager.SKILLSDATA_PATH);
        }
        PlayerSkillsData skillData = objectData.GetData<PlayerSkillsData>();
        if (skillData.gammaRayBurstPoints == 0)
        {
            skillData.gammaRayBurstPoints = 1;
            SaveManager.GetInstance().SavePersistentData(skillData, SaveManager.SKILLSDATA_PATH);
        }


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

                toast.EnqueueToast("GRB level 3 purchased", null, 2.5f);
                break;
        }
    }
}
