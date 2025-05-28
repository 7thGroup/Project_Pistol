using DataDeclaration;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : MainUI
{
    public override MainUIType UIType { get; protected set; }

    private string characterText1;
    private string characterText2;
    private string characterText3;
    private Button startBtn;//테스트
 
    private void Awake()
    {
        UIType = MainUIType.Lobby;
    }
    public override void SetActiveUI(MainUIType activeUIType)
    {
        gameObject.SetActive(UIType == activeUIType);
    }

    private void OnClickStartButton()
    {
        
    }
}
