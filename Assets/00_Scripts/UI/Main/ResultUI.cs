using System.Collections;
using DataDeclaration;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Scene = DataDeclaration.Scene;

public class ResultUI : MainUI
{
    #region UI Object

    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [Space] [SerializeField] private TextMeshProUGUI remainTimeText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private TextMeshProUGUI headAccuracyText;
    [SerializeField] private TextMeshProUGUI maxComboText;
    [Space] [SerializeField] private Button quitBtn;

    #endregion

    public override MainUIType UIType { get; protected set; }

    private void Awake()
    {
        UIType = MainUIType.Result;

        quitBtn.onClick.AddListener(OnClickQuitButton);
    }

    private void Start()
    {
        var bgm = Resources.Load<AudioClip>("Audio/BGM/The Chase Streets");
        SoundManager.Instance.PlayBackgroundMusic(bgm);
        
        if (RoomManager.Instance.CurStageIndex >= 8)
        {
            AchievementManager.Instance.SpawnAchivement("A0005");
        }
    }

    public override void SetActiveUI(MainUIType activeUIType)
    {
        gameObject.SetActive(UIType == activeUIType);
    }

    /// <summary>
    /// 종료 버튼 기능 할당 메서드
    /// </summary>
    private void OnClickQuitButton()
    {
        SceneLoadManager.Instance.LoadScene(Scene.Lobby);
    }

    /// <summary>
    /// 결과값 갱신
    /// </summary>
    /// <param name="rank">등급</param>
    /// <param name="score">점수</param>
    /// <param name="remainTime">남은 시간</param>
    /// <param name="accuracy">명중률</param>
    /// <param name="headAccuracy">헤드샷 명중률</param>
    /// <param name="maxCombo">최대 콤보</param>
    public void SetResultValue(int score, float remainTime, float accuracy, float headAccuracy,
        int maxCombo)
    {
        RankType rank;
        switch (score)
        {
            case < 70000:
                rank = RankType.F;
                break;
            case < 120000:
                rank = RankType.C;
                break;
            case < 210000:
                rank = RankType.B;
                break;
            case < 250000:
                rank = RankType.A;
                break;
            case < 275000:
                rank = RankType.S;
                AchievementManager.Instance.SpawnAchivement("A0008");
                break;
            case < 290000:
                rank = RankType.SS;
                AchievementManager.Instance.SpawnAchivement("A0009");
                break;
            case < 299999:
                rank = RankType.SSS;
                AchievementManager.Instance.SpawnAchivement("A0010");
                break;
            default:
                rank = RankType.SSSS;
                AchievementManager.Instance.SpawnAchivement("A0011");
                break;
        }
        
        rankText.text = rank.ToString();
        scoreText.text = $"<size=30>Score</size>\n<size=40>{score:D6}</size>";

        remainTimeText.text = $"남은 시간\n{remainTime:N2}초";
        accuracyText.text = $"명중률\n{accuracy:N2}%";
        headAccuracyText.text = $"헤드 명중률\n{headAccuracy:N2}%";
        maxComboText.text = $"최대 콤보\n{maxCombo:D2} Combo";
    }
}