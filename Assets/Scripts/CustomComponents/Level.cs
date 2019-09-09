using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Level : MonoBehaviour
{
    public static Color32 GOLD_COLOR = new Color32(255, 190, 0, 255);
    public static Color32 SILVER_COLOR = new Color32(150, 150, 150, 255);
    public static Color32 BRONZE_COLOR = new Color32(125, 75, 40, 255);

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
    public int bronzeGP;
    public int silverGP;
    public int goldGP;
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
                grade.color = BRONZE_COLOR;
            }
            if(levelScore > silverScore)
            {
                grade.color = SILVER_COLOR;
            }
            if(levelScore > goldScore)
            {
                grade.color = GOLD_COLOR;
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
