using System;
using DataDeclaration;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : MainUI
{
    public override MainUIType UIType { get; protected set; }

    private string characterText1;
    private string characterText2;
    private string characterText3;

    [SerializeField] private Button optionButton;//테스트
    [SerializeField] private Button achievementButton;
    private bool isAchieve = false;


    private void Awake()
    {
        UIType = MainUIType.Lobby;
    }

    private void Start()
    {
        optionButton.GetComponent<Button>().onClick.AddListener(OnClickToggleOptionUI);
        achievementButton.GetComponent<Button>().onClick.AddListener(OnClickToggleAchievementUI);
    }

    private void OnClickToggleOptionUI()
    {
        if (isAchieve == true)
        {
            UIManager.Instance.ClosePopUpUI();
            isAchieve = false;
        }
        else
        {
            UIManager.Instance.ClosePopUpUI();
            UIManager.Instance.OpenPopupUI<PopupOption>();
            isAchieve = true;
        }

    }

    private void OnClickToggleAchievementUI()
    {
        if (isAchieve == true)
        {
            UIManager.Instance.ClosePopUpUI();
            isAchieve = false;
        }
        else
        {
            UIManager.Instance.ClosePopUpUI();
            UIManager.Instance.OpenPopupUI<PopupAchievementView>();
            isAchieve = true;
        }
    }

    public override void SetActiveUI(MainUIType activeUIType)
    {
        gameObject.SetActive(UIType == activeUIType);
    }

    private void OnClickStartButton()
    {

    }
}
