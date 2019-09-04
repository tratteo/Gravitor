using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Level : MonoBehaviour
{
    private Color32 goldColor = new Color32(255, 190, 0, 255);
    private Color32 silverColor = new Color32(150, 150, 150, 255);
    private Color32 bronzeColor = new Color32(125, 75, 40, 255);

    public enum LevelCategory { TIME, TIME_DILATED, DISTANCE, MAX_SPEED, OBSTACLES_DESTROY, ENDLESS }

    public int id;
    public Vector2 extrasSpawnRateRange;
    public bool overrideSpawnArea;
    public Vector2 randXSpawn, randYSpawn, randZSpawn;
    public bool isSpawnRateConst;
    public float constSpawnRate;
    public float a, b, g, d;
    public LevelCategory category;
    public float targetTime, targetTimeDilated, targetDistance;
    public int targetObstaclesDestoryed;
    public string levelObjective;
    public string levelInfo;
    public int bronzeScore;
    public int silverScore;
    public int goldScore;
    public PoolManager poolManager;


    private void Start()
    {
        Image mainImage = GetComponent<Image>();
        Image[] images = GetComponentsInChildren<Image>();
        GameObject lockObj = Array.Find(images, obj => obj.gameObject.name == "Lock").gameObject;
        Image grade = Array.Find(images, obj => obj.gameObject.name == "Grade");

        LevelsData data = SaveManager.GetInstance().LoadPersistentData(SaveManager.LEVELSDATA_PATH).GetData<LevelsData>();
        if(data.IsLevelUnlocked(id))
        {
            lockObj.SetActive(false);
            grade.gameObject.SetActive(true);
            int levelScore = data.GetLevelHighScore(id);
            if(levelScore > bronzeScore)
            {
                grade.color = bronzeColor;
            }
            if(levelScore > silverScore)
            {
                grade.color = silverColor;
            }
            if(levelScore > goldScore)
            {
                grade.color = goldColor;
            }
            mainImage.raycastTarget = true;
        }
        else
        {
            lockObj.SetActive(true);
            grade.gameObject.SetActive(false);
            mainImage.raycastTarget = false;
        }
    }
}
