﻿using UnityEngine;
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
    [Header("Buttons")]
    [SerializeField] private GameObject showAdButton = null;
    [Header("Toast")]
    [SerializeField] private ToastScript toast = null;


    private SceneLoader loader;
    private SettingsData settingsData;
    private Sound menuSound;

    private int currentGP;
    private int adBonusGP;
    

    void OnDisable()
    {
        GoogleAdsManager.GetInstance().UnsubscribeToRewardClaimed(EarnReward);
    }

    private void Start()
    {
        loader = GetComponent<SceneLoader>();

        InitializeData();

        AudioManager.GetInstance().NotifyAudioSettings(settingsData);
        menuSound = AudioManager.GetInstance().PlaySound(AudioManager.MENU_SONG);

        GoogleAdsManager.GetInstance().SubscribeToRewardClaimed(EarnReward);

        StartCoroutine(CheckAd_C());
    }

    private IEnumerator CheckAd_C()
    {
        while(true)
        {
            if (GoogleAdsManager.GetInstance().IsRewardedAdLoaded(GoogleAdsManager.RewardedAdType.BONUS_GP))
            {
                showAdButton.SetActive(true);
                showAdButton.GetComponentInChildren<Text>().text = adBonusGP.ToString();
            }
            else
            {
                showAdButton.SetActive(false);
            }
            yield return new WaitForSeconds(1f);
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

    public void LoadGameLevel()
    {
        AudioManager.GetInstance().SmoothOutSound(menuSound, 0.05f, 1f);
        loader.LoadSceneAsynchronously(SceneLoader.LINEAR_MAP_NAME);
    }

    public void ShowRewardedAd()
    {
        GoogleAdsManager.GetInstance().ShowRewardedAd(GoogleAdsManager.RewardedAdType.BONUS_GP);
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
        adBonusGP = 5000;
        switch(objectData.GetData<PlayerData>().playerState)
        {
            case PlayerManager.PlayerState.ASTEROID:
                adBonusGP = 4000;
                break;
            case PlayerManager.PlayerState.COMET:
                adBonusGP = 6000;
                break;
            case PlayerManager.PlayerState.DENSE_PLANET:
                adBonusGP = 10000;
                break;
            case PlayerManager.PlayerState.STAR:
                adBonusGP = 20000;
                break;
        }


        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SKILLSDATA_PATH);
        if (objectData == null)
        {
            SaveManager.GetInstance().SavePersistentData(new PlayerSkillsData(), SaveManager.SKILLSDATA_PATH);
        }

        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.ACHIEVMENTS_PATH);
        if(objectData == null)
        {
            SaveManager.GetInstance().SavePersistentData(new PlayerAchievementsData(), SaveManager.ACHIEVMENTS_PATH);
        }

        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.HIGHSCORE_PATH);
        if (objectData == null)
        {
            objectData = SaveManager.GetInstance().SavePersistentData<int>(0, SaveManager.HIGHSCORE_PATH);
        }
        highScoreText.text = "Highscore\n" + objectData.GetData<int>().ToString();

        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.GRAVITYPOINTS_PATH);
        if (objectData == null)
        {
            objectData = SaveManager.GetInstance().SavePersistentData<int>(0, SaveManager.GRAVITYPOINTS_PATH);
        }
        currentGP = objectData.GetData<int>();
        gravityPointsText.text = "Gravity points\n" + currentGP.ToString();


        if (GoogleAdsManager.GetInstance().IsRewardedAdLoaded(GoogleAdsManager.RewardedAdType.BONUS_GP))
        {
            showAdButton.SetActive(true);
        }
        else
        {
            showAdButton.SetActive(false);
        }
    }
}
