using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(PlayerManager))]
public class AchievementsManager : MonoBehaviour
{   
    private PlayerAchievementsData playerAchievements;
    private PlayerManager playerManager;

    public AchievementInfo[] achievementsInfo;
    [System.Serializable]
    public struct AchievementInfo
    {
        public string id;
        public Sprite sprite;
    }

    void OnDisable()
    {
        playerManager.UnsubscribeToSessionStatsEvent(CheckAchievements);
    }

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        playerManager.SubscribeToSessionStatsEvent(CheckAchievements);
        playerAchievements = SaveManager.GetInstance().LoadPersistentData(SaveManager.ACHIEVMENTS_PATH).GetData<PlayerAchievementsData>();
    }

    private void CheckAchievements(PlayerManager.SessionStats stats)
    {
        //TD
        if (stats.distortedTime > 1800)
        {
            NotifyAchievement(PlayerAchievementsData.SESSION_30TD);
        }
        if (stats.distortedTime > 2700)
        {
            NotifyAchievement(PlayerAchievementsData.SESSION_45TD);
        }
        if (stats.distortedTime > 3600)
        {
            NotifyAchievement(PlayerAchievementsData.SESSION_60TD);
        }
        if (stats.distortedTime > 5400)
        {
            NotifyAchievement(PlayerAchievementsData.SESSION_M_TD);
        }

        //SCORE
        if (stats.score >= 300000)
        {
            NotifyAchievement(PlayerAchievementsData.SESSION_H_300K);
        }
        if (stats.score >= 500000)
        {
            NotifyAchievement(PlayerAchievementsData.SESSION_H_500K);
        }
        if (stats.score >= 1000000)
        {
            NotifyAchievement(PlayerAchievementsData.SESSION_H_1M);
        }
        if (stats.score >= 1500000)
        {
            NotifyAchievement(PlayerAchievementsData.SESSION_H_1_5M);
        }

        //HIT
        if (stats.obstaclesHit >= 10)
        {
            NotifyAchievement(PlayerAchievementsData.SESSION_HIT_10);
        }
        if (stats.obstaclesHit >= 20)
        {
            NotifyAchievement(PlayerAchievementsData.SESSION_HIT_20);
        }
        if (stats.obstaclesHit >= 30)
        {
            NotifyAchievement(PlayerAchievementsData.SESSION_HIT_30);
        }

        //TP
        if (stats.timePlayed >= 60)
        {
            NotifyAchievement(PlayerAchievementsData.SESSION_60TP);
        }
        if (stats.timePlayed >= 150)
        {
            NotifyAchievement(PlayerAchievementsData.SESSION_150TP);
        }
        if (stats.timePlayed >= 300)
        {
            NotifyAchievement(PlayerAchievementsData.SESSION_300TP);
        }

        //SPEED
        if (stats.maxSpeedReached == playerManager.movementManager.maxSpeed)
        {
            NotifyAchievement(PlayerAchievementsData.SESSION_VMAX);
        }

        SaveManager.GetInstance().SavePersistentData<PlayerAchievementsData>(playerAchievements, SaveManager.ACHIEVMENTS_PATH);
    }

    private void NotifyAchievement(string achievementId)
    {
        if (!playerAchievements.IsAchievementUnlocked(achievementId))
        {
            AchievementInfo current = Array.Find(achievementsInfo, achiev => achiev.id == achievementId);
            HUDManager.GetInstance().Toast(HUDManager.ToastType.ACHIEVEMENT_TOAST, "Achievement unlocked", current.sprite, 2.5f, 0.25f, true);
            playerAchievements.UnlockAchievement(achievementId);
        }
    }

}
