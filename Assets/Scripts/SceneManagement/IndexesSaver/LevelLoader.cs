using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader 
{
#region Singleton
    private static LevelLoader instance = null;
    public static LevelLoader GetInstance() 
    { 
        if(instance == null)
        {
            instance = new LevelLoader();
        }
        return instance; 
    }

    #endregion

    private Level currentLevel = null;

    public void SetCurrentLevel(Level level)
    {
        currentLevel = level;
    }

    public Level GetCurrentLevel()
    {
        return currentLevel;
    }
}
