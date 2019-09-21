using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsSceneManager : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup layout = null;
    [SerializeField] private GameObject achievementPrefab = null;
    private PlayerAchievementsData achievementsData;

    private List<AchievementInfo> achievements;
    void Start()
    {
        achievementsData = SaveManager.GetInstance().LoadPersistentData(SaveManager.ACHIEVMENTS_PATH).GetData<PlayerAchievementsData>();
        achievements = PersistentPlayerPrefs.GetInstance().GetAllAchievements();

        foreach(AchievementInfo achInfo in achievements)
        {
            GameObject instance = Instantiate(achievementPrefab);
            instance.transform.SetParent(layout.transform);
            Image image = instance.GetComponentInChildren<Image>();
            image.sprite = achInfo.sprite;
            instance.GetComponent<Text>().text = achInfo.description;
            instance.transform.localScale = new Vector3(1, 1, 1);
            if(achievementsData.IsAchievementUnlocked(achInfo.id))
            {
                image.color = Color.cyan;
            }
        }
    }
}
