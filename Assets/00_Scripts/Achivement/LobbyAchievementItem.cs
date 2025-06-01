using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class LobbyAchievementItem : MonoBehaviour
{
    public AchievementSO achievementSO;

    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private GameObject badge;
    [SerializeField] private Slider slider;

    /// <summary>
    /// 도전과제 정보 및 달성 상태 세팅 (비동기)
    /// </summary>
    public async Task InitData()
    {
        titleText.text = achievementSO.Name;
        descriptionText.text = achievementSO.Description;

        bool isCompleted = await FirebaseManager.Instance.IsAchievementCompletedAsync(achievementSO.ID);
        badge.SetActive(isCompleted);
    }
}
