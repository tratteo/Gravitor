using System.Collections.Generic;

[System.Serializable]
public class PlayerSkins : EncryptedData
{
    public const string DEFAULT = "default";

    private List<string> unlockedSkinsId;
    private string equippedSkinId;

    public PlayerSkins()
    {
        unlockedSkinsId = new List<string>();
    }

    public void UnlockAchievement(string achievement)
    {
        if (!unlockedSkinsId.Contains(achievement))
        {
            unlockedSkinsId.Add(achievement);
        }
    }

    public bool IsAchievementUnlocked(string achievement)
    {
        return unlockedSkinsId.Contains(achievement);
    }
}
