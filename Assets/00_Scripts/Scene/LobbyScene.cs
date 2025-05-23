using UnityEngine;

public class LobbyScene : BaseScene
{
    public override void EnterScene()
    {
        Debug.Log("로딩 엔터");
        GameManager.Instance.selectedCharacter = null;
        GameManager.Instance.selectedWeapon = null;

        var bgm = ResourceManager.Instance.Load<AudioClip>("Audio/BGM/No Way Back_demo");
        SoundManager.Instance.PlayBackgroundMusic(bgm);
        UIManager.Instance.InitMainUI<LobbyUI>();
    }

    public override void ExitScene()
    {
        base.ExitScene();
    }
}