using System;
using UnityEngine;
using UnityEngine.UI;

public class PopupNotice : PopupUI
{
    [SerializeField] private Button xButton;

    [SerializeField] private Text titleText;
    [SerializeField] private Text contentText;

    [SerializeField] private Text leftButtonText;
    [SerializeField] private Button leftButton;
    
    [SerializeField] private Text rightButtonText;
    [SerializeField] private Button rightButton;

    public event Action OnClickLeftButton;
    public event Action OnClickRightButton;

    private void Awake()
    {
        xButton.onClick.AddListener(CloseUI);
        
        leftButton.onClick.AddListener(() => OnClickLeftButton?.Invoke());
        rightButton.onClick.AddListener(() => OnClickRightButton?.Invoke());
    }

    private void OnEnable()
    {
        OnClickLeftButton = null;
        OnClickRightButton = null;
        OnClickLeftButton += CloseUI;
        OnClickRightButton += CloseUI;
    }

    public void SetContentText(string title, string content, string leftBtnTxt, string rightBtnTxt)
    {
        titleText.text = title;
        contentText.text = content;
        leftButtonText.text = leftBtnTxt;
        rightButtonText.text = rightBtnTxt;
    }
}
