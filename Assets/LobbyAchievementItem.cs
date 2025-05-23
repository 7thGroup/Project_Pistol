using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LobbyAchievementItem : MonoBehaviour
{
    public AchievementSO achievementSO;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    

    public void InitData()
    {
        titleText.text = achievementSO.Name;
        descriptionText.text = achievementSO.Description;
    }
}
