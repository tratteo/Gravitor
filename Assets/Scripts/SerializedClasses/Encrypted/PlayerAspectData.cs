using System.Collections.Generic;

[System.Serializable]
public class PlayerAspectData : EncryptedData
{
    public const string DEFAULT = "default";
    public const string COMET = "comet";
    public const string DAMASCUS_STEEL = "damascus_steel";
    public const string SCI_FI = "sci_fi";
    public const string GOLDEN = "golden";
    public const string CHRISTMAS = "christmas";

    private List<string> unlockedSkinsId;
    public string equippedSkinId = null;

    public PlayerAspectData()
    {
        unlockedSkinsId = new List<string>();
        UnlockAspect(DEFAULT);
        equippedSkinId = null;
    }

    public void UnlockAspect(string achievement)
    {
        if (!unlockedSkinsId.Contains(achievement))
        {
            unlockedSkinsId.Add(achievement);
        }
    }

    public bool IsAspectUnlocked(string achievement)
    {
        return unlockedSkinsId.Contains(achievement);
    }

    public void InitializeMissingData()
    {
        equippedSkinId = equippedSkinId == null ? DEFAULT : equippedSkinId;
        base.InitializeDeviceId();
    }
}
