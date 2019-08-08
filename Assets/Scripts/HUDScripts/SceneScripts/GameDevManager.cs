using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDevManager : MonoBehaviour
{
    [SerializeField] private GameObject gravityUnlockable;
    [SerializeField] private GameObject gravFieldsUnlockable;
    [SerializeField] private GameObject gravTimeDilation1Unlockable;
    [SerializeField] private GameObject gravTimeDilation2Unlockable;

    void Start()
    {
        PlayerAchievementsData achievementsData = SaveManager.GetInstance().LoadPersistentData(SaveManager.ACHIEVMENTS_PATH).GetData<PlayerAchievementsData>();
        if(achievementsData.IsAchievementUnlocked(PlayerAchievementsData.SESSION_H_300K))
        {
            gravityUnlockable.SetActive(true);
        }
        if(achievementsData.IsAchievementUnlocked(PlayerAchievementsData.SESSION_H_500K))
        {
            gravFieldsUnlockable.SetActive(true);
        }
        if(achievementsData.IsAchievementUnlocked(PlayerAchievementsData.SESSION_90TD))
        {
            gravTimeDilation1Unlockable.SetActive(true);
        }
        if (achievementsData.IsAchievementUnlocked(PlayerAchievementsData.SESSION_120TD))
        {
            gravTimeDilation2Unlockable.SetActive(true);
        }
    }
}
