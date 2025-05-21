using UnityEngine;
using UnityEngine.UI;

public class UIToggleButton : MonoBehaviour
{
    [SerializeField] private string popupUIName; // 토글할 팝업 UI 이름 (예: "SettingPopupUI")

    private static string currentUIName = null;
    private static bool isAnyUIOpen = false;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClickToggleUI);
    }

    private void OnClickToggleUI()
    {
        // 현재 열린 팝업과 같은 이름이면 닫기
        if (isAnyUIOpen && currentUIName == popupUIName)
        {
            UIManager.Instance.ClosePopUpUI();
            currentUIName = null;
            isAnyUIOpen = false;
            return;
        }
        // 다른 팝업이 열려 있으면 기존 팝업 닫기
        if (isAnyUIOpen && currentUIName != popupUIName)
        {
            UIManager.Instance.ClosePopUpUI();
        }

        // 새 팝업 열기
        UIManager.Instance.OpenPopupUI<PopupOption>();

        currentUIName = popupUIName;
        isAnyUIOpen = true;
    }
}

