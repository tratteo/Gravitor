using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersistentPlayerPrefs : MonoBehaviour
{
    #region SINGLETON
    private static PersistentPlayerPrefs instance = null;
    public static PersistentPlayerPrefs GetInstance() { return instance; }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    [HideInInspector] public PlayerAchievementsData playerAchievements;

    [Header("Achievements")]
    public List<AchievementInfo> achievements;
    [Header("Aspects")]
    public List<PlayerAspect> playerAspects;

    private void Start()
    {
        playerAchievements = SaveManager.GetInstance().LoadPersistentData(SaveManager.ACHIEVMENTS_PATH).GetData<PlayerAchievementsData>();
    }

    public void CheckAchievements(PlayerManager.SessionStats stats)
    {
        //TD
        if (stats.distortedTime > 1800)
        {
            NotifyAchievement(GetAchievementWithId(PlayerAchievementsData.SESSION_30TD));
        }
        if (stats.distortedTime > 2700)
        {
            NotifyAchievement(GetAchievementWithId(PlayerAchievementsData.SESSION_45TD));
        }
        if (stats.distortedTime > 3600)
        {
            NotifyAchievement(GetAchievementWithId(PlayerAchievementsData.SESSION_60TD));
        }
        if (stats.distortedTime > 5400)
        {
            NotifyAchievement(GetAchievementWithId(PlayerAchievementsData.SESSION_M_TD));
        }

        //SCORE
        if (stats.score >= 500000)
        {
            NotifyAchievement(GetAchievementWithId(PlayerAchievementsData.SESSION_H_500K));
        }
        if (stats.score >= 1000000)
        {
            NotifyAchievement(GetAchievementWithId(PlayerAchievementsData.SESSION_H_1M));
        }
        if (stats.score >= 2000000)
        {
            NotifyAchievement(GetAchievementWithId(PlayerAchievementsData.SESSION_H_2M));
        }
        if (stats.score >= 5000000)
        {
            NotifyAchievement(GetAchievementWithId(PlayerAchievementsData.SESSION_H_5M));
        }

        //HIT
        if (stats.obstaclesHit >= 10)
        {
            NotifyAchievement(GetAchievementWithId(PlayerAchievementsData.SESSION_HIT_10));
        }
        if (stats.obstaclesHit >= 20)
        {
            NotifyAchievement(GetAchievementWithId(PlayerAchievementsData.SESSION_HIT_20));
        }
        if (stats.obstaclesHit >= 30)
        {
            NotifyAchievement(GetAchievementWithId(PlayerAchievementsData.SESSION_HIT_30));
        }

        //TP
        if (stats.timePlayed >= 60)
        {
            NotifyAchievement(GetAchievementWithId(PlayerAchievementsData.SESSION_60TP));
        }
        if (stats.timePlayed >= 150)
        {
            NotifyAchievement(GetAchievementWithId(PlayerAchievementsData.SESSION_150TP));
        }
        if (stats.timePlayed >= 300)
        {
            NotifyAchievement(GetAchievementWithId(PlayerAchievementsData.SESSION_300TP));
        }

        //SPEED
        if (stats.maxSpeedReached == stats.maxSpeed)
        {
            NotifyAchievement(GetAchievementWithId(PlayerAchievementsData.SESSION_VMAX));
        }

        SaveManager.GetInstance().SavePersistentData<PlayerAchievementsData>(playerAchievements, SaveManager.ACHIEVMENTS_PATH);
    }

    private void NotifyAchievement(AchievementInfo achInfo)
    {
        if (!playerAchievements.IsAchievementUnlocked(achInfo.id))
        {
            HUDManager.GetInstance().Toast(HUDManager.ToastType.ACHIEVEMENT_TOAST, "Achievement unlocked", achInfo.sprite, 2.5f, 0.25f, true);
            playerAchievements.UnlockAchievement(achInfo.id);
        }
    }


    public AchievementInfo GetAchievementWithId(string id)
    {
        int length = achievements.Count;
        for(int i = 0; i < length; i++)
        {
            if(achievements[i].id == id)
            {
                return achievements[i];
            }
        }
        return null;
    }

    public PlayerAspect GetAspectWithId(string id)
    {
        int length = playerAspects.Count;
        for (int i = 0; i < length; i++)
        {
            if (playerAspects[i].id == id)
            {
                return playerAspects[i];
            }
        }
        return null;
    }

    public List<AchievementInfo> GetAllAchievements()
    {
        return achievements;
    }

}
