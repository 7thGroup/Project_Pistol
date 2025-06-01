using UnityEngine;
using System.Threading.Tasks;

public class PopupAchievementView : PopupUI
{
    [SerializeField] private GameObject achievementItem;
    [SerializeField] private Transform itemPos;

    void OnEnable()
    {
        // 기존 아이템 반환
        for (int i = itemPos.childCount - 1; i >= 0; i--)
        {
            var child = itemPos.GetChild(i).gameObject;
            ObjectPoolManager.Instance.ReturnToPool(child);
        }

        // 아이템 초기화 실행
        _ = InitAchievementItems().ContinueWith(t =>
        {
            if (t.Exception != null)
                Debug.LogError(t.Exception.Flatten());
        });
    }

    private async Task InitAchievementItems()
    {
        AchievementSO[] allAchievements = ResourceManager.Instance.LoadAll<AchievementSO>("Data/SO/AchievementSO");

        foreach (var so in allAchievements)
        {
            GameObject itemGO = ObjectPoolManager.Instance.GetObject(achievementItem, Vector3.zero, Quaternion.identity);
            itemGO.transform.SetParent(itemPos, false);

            LobbyAchievementItem item = itemGO.GetComponent<LobbyAchievementItem>();
            item.achievementSO = so;
            await item.InitData();
        }
    }
}
