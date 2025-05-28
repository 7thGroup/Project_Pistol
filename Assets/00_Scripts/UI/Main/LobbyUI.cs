using System;
using DataDeclaration;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MainUI
{
    public override MainUIType UIType { get; protected set; }

    [SerializeField] private Button optionButton;
    [SerializeField] private Button achievementButton;
    [SerializeField] private Button leaderBoardButton;

    private bool isPopupActive = false;

    private void Awake()
    {
        UIType = MainUIType.Lobby;
    }

    private void Start()
    {
        optionButton.onClick.AddListener(OnClickToggleOptionUI);
        achievementButton.onClick.AddListener(OnClickToggleAchievementUI);
        leaderBoardButton.onClick.AddListener(OnClickToggleLeaderBoardUI);
    }

    private void TogglePopupUI<T>() where T : PopupUI
    {
        if (isPopupActive)
        {
            UIManager.Instance.ClosePopUpUI();
            isPopupActive = false;
        }
        else
        {
            UIManager.Instance.ClosePopUpUI();
            UIManager.Instance.OpenPopupUI<T>();
            isPopupActive = true;
        }
    }

    private void OnClickToggleOptionUI() => TogglePopupUI<PopupOption>();
    private void OnClickToggleAchievementUI() => TogglePopupUI<PopupAchievementView>();
    private void OnClickToggleLeaderBoardUI() => TogglePopupUI<PopupRankingBoard>();

    public override void SetActiveUI(MainUIType activeUIType)
    {
        gameObject.SetActive(UIType == activeUIType);
    }
}
