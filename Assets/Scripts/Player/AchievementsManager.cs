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
        //TODO change Td achievements
        if (stats.distortedTime > 60)
        {
            NotifyAchievement(PlayerAchievementsData.SESSION_60TD);
        }
        if (stats.distortedTime > 90)
        {
            NotifyAchievement(PlayerAchievementsData.SESSION_90TD);
        }
        if (stats.distortedTime > 120)
        {
            NotifyAchievement(PlayerAchievementsData.SESSION_120TD);
        }
        if(stats.distortedTime > 180f)
        {
            NotifyAchievement(PlayerAchievementsData.SESSION_M_TD);
        }

        if(stats.score >= 300000)
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

        if(stats.timePlayed >= 60)
        {
            NotifyAchievement(PlayerAchievementsData.SESSION_60TP);
        }
        if(stats.timePlayed >= 150)
        {
            NotifyAchievement(PlayerAchievementsData.SESSION_150TP);
        }
        if(stats.timePlayed >= 300)
        {
            NotifyAchievement(PlayerAchievementsData.SESSION_300TP);
        }

        if(stats.maxSpeedReached == MovementManager.MAX_SPEED)
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
