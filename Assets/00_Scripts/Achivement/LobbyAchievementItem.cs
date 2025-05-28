using UnityEngine;
using UnityEngine.UI;

public class LobbyAchievementItem : MonoBehaviour
{
    public AchievementSO achievementSO;
    

    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private GameObject badge;
    [SerializeField] private Slider slider;


    /// <summary>
    /// 도전과제 정보 및 달성률 세팅
    /// </summary>
    public void InitData()
    {
        bool isCompleted = AchievementManager.Instance.IsAchievementCompleted(achievementSO.ID);
        badge.SetActive(isCompleted);

        titleText.text = achievementSO.Name;
        descriptionText.text = achievementSO.Description;
    }
}
