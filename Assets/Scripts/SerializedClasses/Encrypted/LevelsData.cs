using System.Collections.Generic;

[System.Serializable]
public class LevelsData : EncryptedData
{
    public const int ENDLESS_ID = 0;

    public Dictionary<int, int> levelsData;

    public LevelsData()
    {
        levelsData = new Dictionary<int, int>();
        levelsData.Add(1, 0);
        levelsData.Add(ENDLESS_ID, 0);
    }

    public bool IsLevelUnlocked(int id)
    {
        return levelsData.ContainsKey(id);
    }

    public void UnlockLevel(int id)
    {
        if (!IsLevelUnlocked(id))
        {
            levelsData.Add(id, 0);
        }
    }

    public void UpdateLevelScore(int id, int score)
    {
        int highScore = levelsData[id];
        if (score > highScore)
        {
            levelsData[id] = score;
        }
    }

    public int GetLevelHighScore(int id)
    {
        return levelsData[id];
    }
}
