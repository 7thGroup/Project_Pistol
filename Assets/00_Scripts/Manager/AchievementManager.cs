using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class AchievementManager : SingletonBehaviour<AchievementManager>
{
    private AchivementDataContainer achivementDataContainer;
    private HashSet<string> showAchievements = new HashSet<string>();

    /// <summary>
    /// 도전과제 출력 메서드
    /// 예AchivementManager.Instance.SpawnAchivement("A0002");
    /// </summary>
    /// <param name="soId"></param>
    public async void SpawnAchivement(string soId)
    {
        if (showAchievements.Contains(soId))//이미 달성한적 있는 도전과제 출력 x
        {
            return;
        }
        AchievementSO so = ResourceManager.Instance.Load<AchievementSO>($"Data/SO/AchievementSO/{soId}");

        var achivePref = ResourceManager.Instance.Load<AchivementDataContainer>("Prefabs/UI/AchievementItem");

        var pooledAchive = ObjectPoolManager.Instance.GetObject(achivePref, Vector3.zero, Quaternion.identity, 3f);

        pooledAchive.transform.SetParent(UIManager.Instance.CurMainUI.transform, false);


        if (pooledAchive != null)
        {
            achivementDataContainer = pooledAchive.GetComponent<AchivementDataContainer>();
            achivementDataContainer.SetData(so);

            showAchievements.Add(soId);

            await FirebaseManager.Instance.SaveAchievementAsync(soId);
        }

    }

    public bool IsAchievementCompleted(string id)
    {
        return showAchievements.Contains(id);
    }

    public async Task SyncAchievementsFromServer()
    {
        var serverData = await FirebaseManager.Instance.LoadCompletedAchievementsAsync();
        showAchievements = new HashSet<string>(serverData);
        Debug.Log("서버 도전과제 동기화 완료");
    }

}


