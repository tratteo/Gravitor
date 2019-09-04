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
    [Header("Ads")]
    [SerializeField] private GameObject showAdButton = null;
    [SerializeField] private GameObject loadingAdObj = null;
    [Header("Toast")]
    [SerializeField] private ToastScript toast = null;

    private SettingsData settingsData;

    private int currentGP;
    private int adBonusGP;
    private bool isAdLoading = false;
    

    void OnDisable()
    {
        GoogleAdsManager.GetInstance().UnsubscribeToRewardClaimed(EarnReward);
    }

    private void Start()
    {

        InitializeData();

        AudioManager.GetInstance().NotifyAudioSettings(settingsData);
        AudioManager.GetInstance().currentMusic = AudioManager.GetInstance().PlaySound(AudioManager.MENU_SONG);

        GoogleAdsManager.GetInstance().SubscribeToRewardClaimed(EarnReward);

        StartCoroutine(CheckAd_C());
    }

    private IEnumerator CheckAd_C()
    {
        while(true)
        {
            if (GoogleAdsManager.GetInstance().IsRewardedAdLoaded(GoogleAdsManager.RewardedAdType.BONUS_GP))
            {
                loadingAdObj.SetActive(false);
                showAdButton.SetActive(true);
                showAdButton.GetComponentInChildren<Text>().text = adBonusGP.ToString();
            }
            else
            {
                showAdButton.SetActive(false);
                loadingAdObj.SetActive(true);
                if (!isAdLoading)
                {
                    GoogleAdsManager.GetInstance().LoadAd(GoogleAdsManager.RewardedAdType.BONUS_GP);
                    isAdLoading = true;
                }
            }
            yield return new WaitForSeconds(2f);
        }
    }

    private void EarnReward(string reward)
    {
        if (reward == "GravityPoints" || reward == "coins")
        {
            currentGP += adBonusGP;
            SaveManager.GetInstance().SavePersistentData<int>(currentGP, SaveManager.GRAVITYPOINTS_PATH);
            Executer.GetInstance().AddJob(() => 
            {
                gravityPointsText.text = "Gravity points\n" + currentGP.ToString();
                toast.ShowToast("Bonus claimed", null, 1.5f);
            });
        }
    }

    public void QuitApp()
    {
        Application.Quit();
    }


    public void ShowRewardedAd()
    {
        showAdButton.SetActive(false);
        loadingAdObj.SetActive(true);
        GoogleAdsManager.GetInstance().ShowRewardedAd(GoogleAdsManager.RewardedAdType.BONUS_GP);
        isAdLoading = false;
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


        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.GRAVITYPOINTS_PATH);
        if (objectData == null)
        {
            objectData = SaveManager.GetInstance().SavePersistentData<int>(0, SaveManager.GRAVITYPOINTS_PATH);
        }
        currentGP = objectData.GetData<int>();
        gravityPointsText.text = "Gravity points\n" + currentGP.ToString();


        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.LEVELSDATA_PATH);
        if(objectData == null)
        {
            objectData = SaveManager.GetInstance().SavePersistentData<LevelsData>(new LevelsData(), SaveManager.LEVELSDATA_PATH);
        }
        LevelsData data = objectData.GetData<LevelsData>();
        highScoreText.text = data.GetLevelHighScore(LevelsData.ENDLESS_ID).ToString();
    }
}
