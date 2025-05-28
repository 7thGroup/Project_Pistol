using System.Collections;
using Unity.Services.Authentication;
using UnityEngine;

public class StartScene : BaseScene
{
    public override void EnterScene()
    {
        UIManager.ToggleMouseCursor(true);
        UIManager.Instance.InitMainUI<StartUI>();
        var bgm = ResourceManager.Instance.Load<AudioClip>("Audio/BGM/Let's Rock_demo");
        SoundManager.Instance.PlayBackgroundMusic(bgm);
        CoroutineRunner.Run(OpenPopupAuth());
    }

    private IEnumerator OpenPopupAuth()
    {
        if (FirebaseManager.Instance == null)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1f);
        UIManager.Instance.OpenPopupUI<PopupAuth>();
    }

    public override void ExitScene()
    {
        base.ExitScene();
    }
}