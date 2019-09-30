
using UnityEngine;

[System.Serializable]
public class PlayerData : EncryptedData
{
    public const int INITIAL_EXPERIENCE = 2000;
    public const short RESILIENCE_MAX_POINTS = 15;
    public const short THRUSTFORCE_MAX_POINTS = 30;
    private const float HEALTH_INCREASE = 50f;
    public ushort thrustForcePoints;
    public float resilience;
    public PlayerManager.PlayerState playerState;
    public bool isFirstLaunch;

    public int playerLevel;
    public int currentExp;

    /// <summary>
    /// Create instance and set points values to default and playerState to ASTEROID  
    /// </summary>
    public PlayerData()
    {
        thrustForcePoints = 1;
        resilience = 100f;
        playerState = PlayerManager.PlayerState.ASTEROID;
        isFirstLaunch = true;
        playerLevel = 1;
    }

    public int CalculateLevel(int expGained)
    {
        int expNeeded = GetLevelExpNeeded();
        if (currentExp + expGained < expNeeded)
        {
            currentExp += expGained;
            return 0;
        }
        else
        {
            int rest = expGained + currentExp;
            while(rest >= expNeeded)
            {
                playerLevel++;
                rest = rest - expNeeded;
                expNeeded = GetLevelExpNeeded();
            }
            currentExp = rest;
            return playerLevel;
        }
    }

    public int GetLevelExpNeeded()
    {
        if (playerLevel == 1) return INITIAL_EXPERIENCE;
        else
        {
            return (int)(INITIAL_EXPERIENCE * (playerLevel - 1) * 3f);
        }
    }

    public void InitializeMissingData()
    {
        base.InitializeDeviceId();
        playerLevel = playerLevel == 0 ? 1 : playerLevel;
        resilience = resilience < 100f ? 100f : resilience;
        if (currentExp > GetLevelExpNeeded())
        {
            int exp = currentExp;
            currentExp = 0;
            CalculateLevel(exp);
        }

        switch (playerState)
        {
            case PlayerManager.PlayerState.COMET:
                break;
            default:
                playerState = PlayerManager.PlayerState.COMET;
                break;
        }
    }

    public void IncreaseHealth()
    {
        resilience += HEALTH_INCREASE;
    }
   
    public int GetHealthPoints()
    {
        return (int)(resilience / HEALTH_INCREASE) - 1;
    }
}
