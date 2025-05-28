using UnityEngine;
using System.Threading.Tasks;

public class PopupAchievementView : PopupUI
{
    [SerializeField] private GameObject achievementItem;
    [SerializeField] private Transform itemPos;

    void OnEnable()
    {
        for (int i = itemPos.childCount - 1; i >= 0; i--)
        {
            var child = itemPos.GetChild(i).gameObject;
            ObjectPoolManager.Instance.ReturnToPool(child);
        }
        
        _ = InitAchievementItems().ContinueWith(t =>
        {
            if (t.Exception != null)
                Debug.LogError(t.Exception.Flatten());
        });
    }

    async Task InitAchievementItems()
    {
        AchievementSO[] allAchievement = ResourceManager.Instance.LoadAll<AchievementSO>("Data/SO/AchievementSO");

        await AchievementManager.Instance.SyncAchievementsFromServer(); // 서버에서 최신 도전과제 상태 로딩

        foreach (var so in allAchievement)
        {
            GameObject achievementItems = ObjectPoolManager.Instance.GetObject(achievementItem, Vector3.zero, Quaternion.identity);
            achievementItems.transform.SetParent(itemPos, false);

            LobbyAchievementItem lobbyAchievement = achievementItems.GetComponent<LobbyAchievementItem>();
            lobbyAchievement.achievementSO = so;
            lobbyAchievement.InitData(); // 도전과제 상태 설정 (뱃지 포함)
        }
    }
}
