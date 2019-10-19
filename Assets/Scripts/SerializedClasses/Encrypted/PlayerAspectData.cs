using System.Collections.Generic;

[System.Serializable]
public class PlayerAspectData : EncryptedData
{
    public const string DEFAULT = "default";
    public const string COMET = "comet";
    public const string RED_COMET = "red_comet";
    public const string VIOLET_COMET = "violet_comet";
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

    public string AspectStringFromId(string id)
    {
        switch(id)
        {
            case DEFAULT: return "Asteroid";
            case COMET: return "Comet";
            case RED_COMET: return "Red Comet";
            case VIOLET_COMET: return "Violet Comet";
            case DAMASCUS_STEEL: return "Damascus Steel";
            case SCI_FI: return "Sci-Fi";
            case GOLDEN: return "Golden";
            case CHRISTMAS: return "Christmas";
            default: return "";
        }
    }

    public void InitializeMissingData()
    {
        equippedSkinId = equippedSkinId == null ? DEFAULT : equippedSkinId;
        base.InitializeDeviceId();
    }
}
