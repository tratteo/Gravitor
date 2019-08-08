
[System.Serializable]
public class PlayerSkillsData
{
    public ushort antigravityPoints;
    public ushort solarflarePoints;
    public ushort quantumTunnelPoints;

    /// <summary>
    /// Create instance and set points to 1
    /// </summary>
    public PlayerSkillsData()
    {
        antigravityPoints = 1;
        solarflarePoints = 1;
        quantumTunnelPoints = 1;
    }
}
