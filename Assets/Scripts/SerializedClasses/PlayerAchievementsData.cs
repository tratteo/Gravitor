using System.Collections.Generic;

[System.Serializable]
public class PlayerAchievementsData
{
    public const string SESSION_60TD = "session_60Td";
    public const string SESSION_90TD = "session_90Td";
    public const string SESSION_120TD = "session_120Td";
    public const string SESSION_M_TD = "session_M_td";
    public const string SESSION_H_300K = "session_H_300K";
    public const string SESSION_H_500K = "session_H_500K";
    public const string SESSION_H_1M = "session_H_1M";
    public const string SESSION_HIT_10 = "session_HIT_10";
    public const string SESSION_HIT_20 = "session_HIT_20";
    public const string SESSION_HIT_30 = "session_HIT_30";
    public const string SESSION_60TP = "session_60Tp";
    public const string SESSION_150TP = "session_150Tp";
    public const string SESSION_300TP = "session_300Tp";
    public const string SESSION_VMAX = "session_Vmax";


    private List<string> unlockedAchievements;

    public PlayerAchievementsData()
    {
        unlockedAchievements = new List<string>();
    }

    public void UnlockAchievement(string achievementId)
    {
        if(!unlockedAchievements.Contains(achievementId))
        {
            unlockedAchievements.Add(achievementId);
        }
    }

    public bool IsAchievementUnlocked(string achievementId)
    {
        return unlockedAchievements.Contains(achievementId);
    }
}
