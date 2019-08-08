﻿
[System.Serializable]
public class PlayerData
{
    private const float HEALTH_INCREASE = 50f;
    public ushort thrustForcePoints;
    public float health;
    public PlayerManager.PlayerState playerState;
    public bool isFirstLaunch;

    /// <summary>
    /// Create instance and set points values to default and playerState to ASTEROID  
    /// </summary>
    public PlayerData()
    {
        thrustForcePoints = 1;
        health = 100f;
        playerState = PlayerManager.PlayerState.ASTEROID;
        isFirstLaunch = true;
    }

    public void IncreaseHealth()
    {
        health += HEALTH_INCREASE;
    }
   
    public int GetHealthPoints()
    {
        return (int)(health / HEALTH_INCREASE) - 1;
    }
}
