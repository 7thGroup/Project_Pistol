using System.Threading.Tasks;
using UnityEngine;

public class AchievementManager : SingletonBehaviour<AchievementManager>
{
    private AchivementDataContainer achivementDataContainer;

    /// <summary>
    /// 도전과제 출력 메서드
    /// 예) AchievementManager.Instance.SpawnAchivement("A0002");
    /// </summary>
    /// <param name="soId">도전과제 ID</param>
    public async void SpawnAchivement(string soId)
    {
        bool alreadyCompleted = await FirebaseManager.Instance.IsAchievementCompletedAsync(soId);
        if (alreadyCompleted)
        {
            return;
        }

        // 도전과제 데이터 로딩
        AchievementSO so = ResourceManager.Instance.Load<AchievementSO>($"Data/SO/AchievementSO/{soId}");
        var achivePref = ResourceManager.Instance.Load<AchivementDataContainer>("Prefabs/UI/AchievementItem");

        var pooledAchive = ObjectPoolManager.Instance.GetObject(achivePref, Vector3.zero, Quaternion.identity, 3f);
        if (pooledAchive != null)
        {
            pooledAchive.transform.SetParent(UIManager.Instance.CurMainUI.transform, false);

            achivementDataContainer = pooledAchive.GetComponent<AchivementDataContainer>();
            achivementDataContainer.SetData(so);

            await FirebaseManager.Instance.SaveAchievementAsync(soId);
            Debug.Log($"[도전과제] 새 도전과제 완료 및 저장: {soId}");
        }
    }
}
