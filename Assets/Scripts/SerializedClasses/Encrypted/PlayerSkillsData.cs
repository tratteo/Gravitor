
[System.Serializable]
public class PlayerSkillsData : EncryptedData
{

    public const short ANTIGRAVITY_MAX_POINTS = 20;
    public const short QUANTUMTUNNEL_MAX_POINTS = 20;
    public const short SOLARFLARE_MAX_POINTS = 20;

    public const short GRB_MAX_POINTS = 3;

    public int antigravityPoints = 1;
    public int solarflarePoints = 1;
    public int quantumTunnelPoints = 1;
    public int gammaRayBurstPoints = 1;

    /// <summary>
    /// Create instance and set points to 1
    /// </summary>
    public PlayerSkillsData()
    {
        antigravityPoints = 1;
        solarflarePoints = 1;
        quantumTunnelPoints = 1;
        gammaRayBurstPoints = 1;
    }
}
