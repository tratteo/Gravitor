using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject content = null;
    private PlayerAchievementsData achievements;

    void Start()
    {
        achievements = SaveManager.GetInstance().LoadPersistentData(SaveManager.ACHIEVMENTS_PATH).GetData<PlayerAchievementsData>();
        foreach(Transform current in content.transform)
        { 
            EnableAchivementUI(current.gameObject, achievements.IsAchievementUnlocked(current.name));
        }
    }

    private void EnableAchivementUI(GameObject icon, bool state)
    {
        if (state)
        {
            icon.GetComponent<Image>().color = Color.cyan;
        }
    }
}
