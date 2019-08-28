
[System.Serializable]
public class PlayerSkillsData
{
    public ushort antigravityPoints = 1;
    public ushort solarflarePoints = 1;
    public ushort quantumTunnelPoints = 1;
    public ushort gammaRayBurstPoints = 1;

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
