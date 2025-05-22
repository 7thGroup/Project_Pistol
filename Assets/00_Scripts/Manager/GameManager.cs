#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public sealed class GameManager : SingletonBehaviour<GameManager>
{
    public CharacterSO selectedCharacter;
    public WeaponSO selectedWeapon;
    public bool isOption = false;

    /// <summary>
    /// PopupOption 여닫기 
    /// TimeScale기본값 false
    /// </summary>
    /// <param name="puaseTime"></param>
    public void TogglePopup(bool puaseTime = false)
    {
         var controller = StageManager.Instance.Player?.Controller;

        if (isOption)
        {
            isOption = false;
            UIManager.Instance.ClosePopUpUI();
            Time.timeScale = 1f;

            if (controller != null)
            {
                controller.EnablePlayerInput(true);//플레이어 마우스 회전 ㅇ
            }
        }
        else
        {
            isOption = true;
            UIManager.Instance.OpenPopupUI<PopupOption>();
            Time.timeScale = puaseTime ? 0f : 1f;

            if (controller != null)
            {
                controller.EnablePlayerInput(false);//플레이어 마우스 회전 x
            }
        }
    }

    public static void GameQuit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
