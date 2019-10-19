using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class PersistentPrefs : MonoBehaviour
{
    #region SINGLETON
    private static PersistentPrefs instance = null;
    public static PersistentPrefs GetInstance() { return instance; }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    public enum RewardType { GRAVITY_POINT, GRAVITON, ASPECT}
    [System.Serializable]
    public struct Reward
    {
        public string id;
        public RewardType type;
        [Range(0f, 1f)]
        public float probability;
        public int amount;
    }

    [HideInInspector] public PlayerAchievementsData playerAchievements;

    [Header("Timed Reward")]
    public int timeDelay;
    public int gravitonsCost;
    [SerializeField] private List<Reward> rewards;
    private List<Reward> originalRewards;

    [Header("Shared UI")]
    public Sprite transparentIcon = null;
    public Sprite gravitonsIcon = null;
    public Sprite gravityPointsIcon = null;
    public Sprite magneticShieldIcon = null;
    public Sprite GRBIcon = null;
    public Sprite antiGravityIcon = null;
    public Sprite quantumTunnelIcon = null;
    public Sprite solarflareIcon = null;

    [Header("Achievements")]
    public List<AchievementInfo> achievements;
    [Header("Aspects")]
    public List<PlayerAspect> playerAspects;

    private void OnEnable()
    {
        originalRewards = new List<Reward>(rewards);
    }

    public void CheckAchievements(PlayerManager.SessionStats stats)
    {
        playerAchievements = SaveManager.GetInstance().LoadPersistentData(SaveManager.ACHIEVMENTS_PATH).GetData<PlayerAchievementsData>();
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
        if (stats.distortedTime > 9000)
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
        if (achInfo != null)
        {
            if (!playerAchievements.IsAchievementUnlocked(achInfo.id))
            {
                HUDManager.GetInstance().Toast(HUDManager.ToastType.ACHIEVEMENT_TOAST, "Achievement unlocked", achInfo.sprite, 2.5f, 0.25f, true);
                playerAchievements.UnlockAchievement(achInfo.id);
            }
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

    public List<Reward> GetAllRewards()
    {
        return rewards;
    }

    private void OnApplicationPause(bool pause)
    {
        SaveObject saveObj = SaveManager.GetInstance().LoadPersistentData(SaveManager.SERVICES_PATH);
        if (saveObj != null)
        {
            ServicesData servData = saveObj.GetData<ServicesData>();
            servData.lastAccess = System.DateTime.Now;
            SaveManager.GetInstance().SavePersistentData(servData, SaveManager.SERVICES_PATH);
        }
    }

    public Reward GetRandomReward()
    {
        int count = rewards.Count;
        if (count == 0) return default(Reward);
        int index = -1;
        float radix = Random.Range(0f, 1f);

        while (radix > 0 && index < count)
        {
            radix -= rewards[++index].probability;
        }
        return rewards[index];
    }

    public void SetUnlockableAspects()
    {
        rewards = originalRewards;
        PlayerAspectData aspect = SaveManager.GetInstance().LoadPersistentData(SaveManager.ASPECTDATA_PATH).GetData<PlayerAspectData>();
        int length = rewards.Count;
        List<Reward> temp = new List<Reward>(rewards);
        for (int i = 0; i < length; i++)
        {
            if(rewards[i].type == RewardType.ASPECT && aspect.IsAspectUnlocked(rewards[i].id))
            {
                temp.Remove(rewards[i]);
            }
        }
        rewards = temp;
        NormalizeRewardsProbabilities();
    }

    private void NormalizeRewardsProbabilities()
    {
        Reward[] temp = rewards.ToArray();
        float total = 0f;
        int length = temp.Length;
        for (int i = 0; i < length; i++)
        {
            total += temp[i].probability;
        }
        for (int i = 0; i < length; i++)
        {
            temp[i].probability /= total;
        }
        rewards = temp.ToList();
    }
}
