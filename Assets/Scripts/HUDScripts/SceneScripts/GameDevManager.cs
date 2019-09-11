using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDevManager : MonoBehaviour
{
    [SerializeField] private GameObject gravityUnlockable;
    [SerializeField] private GameObject gravFieldsUnlockable;
    [SerializeField] private GameObject gravTimeDilation1Unlockable;
    [SerializeField] private GameObject gravTimeDilation2Unlockable;
    [SerializeField] private GameObject velocityTimeDilationUnlockable;

    void Start()
    {
        PlayerAchievementsData achievementsData = SaveManager.GetInstance().LoadPersistentData(SaveManager.ACHIEVMENTS_PATH).GetData<PlayerAchievementsData>();
        gravityUnlockable.SetActive(achievementsData.IsAchievementUnlocked(PlayerAchievementsData.SESSION_H_500K));
        gravFieldsUnlockable.SetActive(achievementsData.IsAchievementUnlocked(PlayerAchievementsData.SESSION_H_2M));
        gravTimeDilation1Unlockable.SetActive(achievementsData.IsAchievementUnlocked(PlayerAchievementsData.SESSION_30TD));
        gravTimeDilation2Unlockable.SetActive(achievementsData.IsAchievementUnlocked(PlayerAchievementsData.SESSION_45TD));
        velocityTimeDilationUnlockable.SetActive(achievementsData.IsAchievementUnlocked(PlayerAchievementsData.SESSION_VMAX));
    }
}
