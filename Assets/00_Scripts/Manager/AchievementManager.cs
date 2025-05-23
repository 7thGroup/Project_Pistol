using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : SingletonBehaviour<AchievementManager>
{
    private AchivementDataContainer achivementDataContainer;
    private HashSet<string> showAchievements = new HashSet<string>();

    /// <summary>
    /// 도전과제 출력 메서드
    /// 예AchivementManager.Instance.SpawnAchivement("A0002");
    /// </summary>
    /// <param name="soId"></param>
    public void SpawnAchivement(string soId)
    {
        if (showAchievements.Contains(soId))
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
        }

    }
}
