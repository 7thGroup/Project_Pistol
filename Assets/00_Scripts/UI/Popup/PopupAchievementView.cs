using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupAchievementView : PopupUI
{
    [SerializeField] private GameObject achievementItem;
    [SerializeField] private Transform itemPos;
    void Start()
    {
        InitAchievementItems();
    }

    void InitAchievementItems()
    {
        AchievementSO[] allAchievement = ResourceManager.Instance.LoadAll<AchievementSO>("Data/SO/AchievementSO");

        foreach (var so in allAchievement)
        {
            GameObject achievementItems = ObjectPoolManager.Instance.GetObject(achievementItem, Vector3.zero, Quaternion.identity);
            achievementItems.transform.SetParent(itemPos, false);

            LobbyAchievementItem lobbyAchievement = achievementItems.GetComponent<LobbyAchievementItem>();
            lobbyAchievement.achievementSO = so;
            lobbyAchievement.InitData();
        }
    }
}
