using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataDeclaration;

public sealed class UIManager : SingletonBehaviour<UIManager>
{
    private GameObject mainCanvas; // 메인 캔버스 게임오브젝트
    private GameObject popupCanvas; // 팝업 캔버스 게임오브젝트
    private CanvasGroup fader; // 페이드 연출

    private readonly List<MainUI> mainUIList = new(); // MainUI 관리용 리스트

    private readonly Stack<PopupUI> curPopupUIStack = new(); // Popup UI 관리용 Stack
    private readonly Dictionary<string, PopupUI> popupUIPool = new(); // 비활성화 된 Popup UI Pool

    public MainUI CurMainUI { get; private set; } // 현재 활성화 된 Main UI

    protected override void Awake()
    {
        base.Awake();
        InitMainCanvas();
        InitPopupCanvas();
        InitFader();
    }

    #region Public Method

    #region Main UI

    /// <summary>
    /// ScreenUI를 상속받은 UI 클래스 생성 및 초기화
    /// </summary>
    /// <typeparam name="T">ScreenUI 클래스</typeparam>
    public void InitMainUI<T>() where T : MainUI
    {
        var mainUI = mainUIList.Find(ui => ui.GetType() == typeof(T));
        if (mainUI == null)
        {
            //TODO: Resources.Load 나중에 Addressable로 바꾸기
            var resource = ResourceManager.Instance.Load<T>($"Prefabs/UI/Main/{typeof(T).Name}");
            mainUI = Instantiate(resource, mainCanvas.transform, false);
            mainUIList.Add(mainUI);
        }

        ChangeMainUI(mainUI.UIType);
    }

    /// <summary>
    /// UI찾기
    /// </summary>
    /// <typeparam name="T">ScreenUI 클래스</typeparam>
    public T GetMainUI<T>() where T : MainUI
    {
        return mainUIList.Find(ui => ui is T) as T;
    }

    #endregion

    #region Popup UI

    /// <summary>
    /// Resources/Prefabs/UI/PopUp/ 경로에 있는 Popup UI 리소스 생성
    /// </summary>
    /// <typeparam name="T">PopupUI 클래스</typeparam>
    public T OpenPopupUI<T>() where T : PopupUI
    {
        var openUI = FindPopupUIInPool<T>();
        if (openUI == null)
        {
            var resource = ResourceManager.Instance.Load<T>($"Prefabs/UI/PopUp/{typeof(T).Name}");
            openUI = Instantiate(resource, popupCanvas.transform, false);
        }

        openUI.gameObject.SetActive(true);
        // 하이라키 창에서 제일 마지막 오브젝트로 배치(렌더링 순서)
        openUI.transform.SetAsLastSibling();
        curPopupUIStack.Push(openUI);

        return openUI;
    }

    /// <summary>
    /// 같은 타입의 Popup UI를 중복으로 띄울 수 있는 메서드 (새 인스턴스 생성)
    /// </summary>
    /// <typeparam name="T">PopupUI 클래스</typeparam>
    public T OpenPopUpUIMultiple<T>() where T : PopupUI
    {
        string key = typeof(T).Name;
        T popup = null;

        // 비활성화된 팝업이 있다면 재사용
        if (popupUIPool.TryGetValue(key, out var pooledUI) && pooledUI != null)
        {
            popup = pooledUI as T;
            popupUIPool.Remove(key);//재사용 했으면 풀에서 제거
        }

        if (popup == null)
        {
            var resource = ResourceManager.Instance.Load<T>($"Prefabs/UI/PopUp/{key}");
            popup = Instantiate(resource, popupCanvas.transform, false);

        }

        popup.gameObject.SetActive(true);
        popup.transform.SetAsLastSibling();
        curPopupUIStack.Push(popup);

        return popup;
    }


    /// <summary>
    /// Popup 창 닫기
    /// </summary>
    public void ClosePopUpUI()
    {
        if (curPopupUIStack.Count == 0)
        {
            Debug.LogWarning("팝 할거 없음");
            return;
        }

        var curPopup = curPopupUIStack.Pop();
        curPopup.gameObject.SetActive(false);

        var key = curPopup.GetType().Name;
        popupUIPool[key] = curPopup;
    }

    public void ClosePopUpUI(PopupUI targetPopup)
    {
        if (targetPopup == null || !targetPopup.gameObject.activeSelf)
        {
            Debug.LogWarning("ClosePopUpUI: 이미 닫혀 있거나 null임.");
            return;
        }

        // Stack에서 제거
        Stack<PopupUI> tempStack = new Stack<PopupUI>();
        bool removed = false;

        while (curPopupUIStack.Count > 0)
        {
            var popup = curPopupUIStack.Pop();
            if (popup == targetPopup && !removed)
            {
                popup.gameObject.SetActive(false);
                popupUIPool[popup.GetType().Name] = popup;
                removed = true;
            }
            else
            {
                tempStack.Push(popup);
            }
        }

        // 나머지 다시 복구
        while (tempStack.Count > 0)
        {
            curPopupUIStack.Push(tempStack.Pop());
        }

        if (!removed)
        {
            Debug.LogWarning("ClosePopUpUI: 해당 팝업이 Stack에 없었음.");
        }
    }

    #endregion

    #region Utility

    public void ClearCanvas()
    {
        ClearMainCanvas();
        ClearPopupCanvas();
    }

    /// <summary>
    /// 마우스 커서 On/Off
    /// </summary>
    /// <param name="isActivation">True: 마우스 커서 활성화
    /// <para>False: 마우스 커서 비활성화</para></param>
    public static void ToggleMouseCursor(bool isActivation)
    {
        Cursor.lockState = isActivation ? CursorLockMode.None : CursorLockMode.Confined;
        Cursor.visible = isActivation;
    }

    /// <summary>
    /// Fade In/Out 효과
    /// </summary>
    /// <param name="startAlpha">시작 알파값</param>
    /// <param name="endAlpha">최종 알파값</param>
    /// <param name="duration">지속 시간</param>
    /// <param name="isReset">알파값 0 초기화 여부</param>
    public IEnumerator FadeEffect(float startAlpha, float endAlpha, float duration, bool isReset = false)
    {
        var elapsedTime = 0f;
        fader.alpha = startAlpha;
        while (elapsedTime <= duration)
        {
            elapsedTime += Time.deltaTime;
            fader.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            yield return null;
        }

        fader.alpha = endAlpha;

        if (isReset)
        {
            yield return new WaitForEndOfFrame();
            fader.alpha = 0;
        }
    }

    #endregion

    #endregion

    #region Private Method

    /// <summary>
    /// Main UI 전환
    /// </summary>
    /// <param name="activeUIType">전환하려고 하는 Main UI 타입</param>
    private void ChangeMainUI(MainUIType activeUIType)
    {
        foreach (var mainUI in mainUIList)
        {
            mainUI.SetActiveUI(activeUIType);
            if (mainUI.gameObject.activeSelf)
            {
                CurMainUI = mainUI;
            }
        }
    }

    /// <summary>
    /// 비활성화 UI Pool에서 UI 검색
    /// </summary>
    /// <typeparam name="T">PopupUI 클래스</typeparam>
    /// <returns>비활성화된 Popup UI</returns>
    private T FindPopupUIInPool<T>() where T : PopupUI
    {
        return popupUIPool.GetValueOrDefault(typeof(T).Name) as T;
    }

    /// <summary>
    /// Main Canvas 초기화
    /// </summary>
    private void InitMainCanvas()
    {
        mainCanvas = GameObject.Find("MainCanvas"); // Main Canvas 찾기

        if (mainCanvas == null) // Main Canvas가 없을 경우 새로 생성
        {
            //TODO: Resources.Load 나중에 Addressable로 바꾸기
            var resource = ResourceManager.Instance.Load<GameObject>("Prefabs/UI/MainCanvas");
            mainCanvas = Instantiate(resource);
        }

        DontDestroyOnLoad(mainCanvas);
    }

    private void InitPopupCanvas()
    {
        popupCanvas = GameObject.Find("PopupCanvas");

        if (popupCanvas == null)
        {
            //TODO: Resources.Load 나중에 Addressable로 바꾸기
            var resource = ResourceManager.Instance.Load<GameObject>("Prefabs/UI/PopupCanvas");
            popupCanvas = Instantiate(resource);
        }

        DontDestroyOnLoad(popupCanvas);
    }

    /// <summary>
    /// Fader 초기화
    /// </summary>
    private void InitFader()
    {
        fader = (CanvasGroup)FindAnyObjectByType(typeof(CanvasGroup)); // Fader 찾기

        if (fader == null) // Fader가 없을 경우 새로 생성
        {
            //TODO: Resources.Load 나중에 Addressable롤 바꾸기
            var resource = ResourceManager.Instance.Load<GameObject>("Prefabs/UI/Fader");
            fader = Instantiate(resource).GetComponent<CanvasGroup>();
        }

        DontDestroyOnLoad(fader.gameObject);
    }

    /// <summary>
    /// Main Canvas에 있는 UI 정리
    /// </summary>
    private void ClearMainCanvas()
    {
        for (var i = 0; i < mainUIList.Count; i++)
        {
            if (mainUIList[i] != null)
            {
                Destroy(mainUIList[i].gameObject);
            }
        }
        mainUIList.Clear();
    }

    /// <summary>
    /// Popup Canvas 정리
    /// </summary>
    private void ClearPopupCanvas()
    {
        curPopupUIStack.Clear();
        popupUIPool.Clear();
        for (var i = 0; i < popupCanvas.transform.childCount; i++)
        {
            Destroy(popupCanvas.transform.GetChild(i).gameObject);
        }
    }

    #endregion
}