using System.Collections.Generic;

[System.Serializable]
public class PlayerAchievementsData
{
    public const string SESSION_30TD = "session_30Td";
    public const string SESSION_45TD = "session_45Td";
    public const string SESSION_60TD = "session_60Td";
    public const string SESSION_M_TD = "session_M_td";
    public const string SESSION_H_500K = "session_H_500K";
    public const string SESSION_H_1M = "session_H_1M";
    public const string SESSION_H_2M = "session_H_2M";
    public const string SESSION_H_5M = "session_H_5M";
    public const string SESSION_HIT_10 = "session_HIT_10";
    public const string SESSION_HIT_20 = "session_HIT_20";
    public const string SESSION_HIT_30 = "session_HIT_30";
    public const string SESSION_60TP = "session_60Tp";
    public const string SESSION_150TP = "session_150Tp";
    public const string SESSION_300TP = "session_300Tp";
    public const string SESSION_VMAX = "session_Vmax";

    private List<string> unlockedAchievementsIds;

    public PlayerAchievementsData()
    {
        unlockedAchievementsIds = new List<string>();
    }

    public void UnlockAchievement(string achievement)
    {
        if(!unlockedAchievementsIds.Contains(achievement))
        {
            unlockedAchievementsIds.Add(achievement);
        }
    }

    public bool IsAchievementUnlocked(string achievement)
    {
        return unlockedAchievementsIds.Contains(achievement);
    }
}
