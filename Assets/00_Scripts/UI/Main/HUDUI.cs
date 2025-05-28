
using DataDeclaration;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDUI : MainUI
{
    #region UI Object

    [Header("스코어 관련")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] public SpawnScoreItem spawnScoreItem;

    [SerializeField] private TextMeshProUGUI remainTimeText;
    [SerializeField] private TextMeshProUGUI curStageText;

    [SerializeField] private Image[] stageIndex;
    [Space][SerializeField] private Image equipImage;
    [SerializeField] private TextMeshProUGUI ammoText;
    [Space][SerializeField] private Image spdGauge;
    [SerializeField] private TextMeshProUGUI spdText;
    [SerializeField] private Image hdlGauge;
    [SerializeField] private TextMeshProUGUI hdlText;
    [SerializeField] private Image rclGauge;
    [SerializeField] private TextMeshProUGUI rclText;
    [SerializeField] private Image stpGauge;
    [SerializeField] private TextMeshProUGUI stpText;


    #endregion

    private Color32 originalColor;
    private Color32 currentColor;

    public override MainUIType UIType { get; protected set; }

    private void Awake()
    {
        UIType = MainUIType.HUD;

        originalColor = new Color32(221, 234, 249, 255);
        currentColor = new Color32(252, 192, 1, 255);
    }

    private void Start()
    {
        RoomManager.Instance.RoomChangedAction += UpdateStageInfo;
        UpdateStageInfo();
        UpdateStatValue();

        var weapon = GameManager.Instance.selectedWeapon;
        var image = ResourceManager.Instance.Load<Sprite>($"Sprites/{weapon.ID}");
        SetEquipImage(image);
    }

    private void Update()
    {
        if (StageManager.Instance.Player)
        {
            UpdateRealTimeChanges();
        }
    }

    public override void SetActiveUI(MainUIType activeUIType)
    {
        gameObject.SetActive(UIType == activeUIType);
    }

    /// <summary>
    /// 실시간의로 변경되는 UI 갱신 메서드
    /// </summary>
    private void UpdateRealTimeChanges()
    {
        scoreText.text = $"<size=36>Score</size>\n<size=80>{StageManager.Instance.GameScore:D6}</size>";
        remainTimeText.text = $"<size=120>{StageManager.Instance.RemainTime:N2}</size>";
        ammoText.text = $"{StageManager.Instance.Player.Weapon.CurAmmo} / {StageManager.Instance.Player.Weapon.MaxAmmo}";
        comboText.text = $"<size=36>Combo</size>\n<size=80>{StageManager.Instance.DestroyTargetCombo}</size>";
        if (StageManager.Instance.DestroyTargetCombo >= 10)
        {
            AchievementManager.Instance.SpawnAchivement("A0012");
        }
    }

    /// <summary>
    /// Stage 정보 UI 갱신 메서드
    /// </summary>
    private void UpdateStageInfo()
    {
        curStageText.text = $"Stage {RoomManager.Instance.CurStageIndex}";

        for (var i = 1; i <= stageIndex.Length; i++)
        {
            if (RoomManager.Instance.CurRoomIndex == 0)
            {
                stageIndex[i - 1].color = originalColor;
            }
            else
            {
                stageIndex[i - 1].color = currentColor;
                stageIndex[i - 1].color = (i == RoomManager.Instance.CurRoomIndex) ? currentColor : originalColor;
            }
        }
    }

    /// <summary>
    /// 캐릭터 스탯 UI 갱신 메서드
    /// </summary>
    public void UpdateStatValue()
    {
        var stat = StageManager.Instance.Player.Stat;

        rclGauge.fillAmount = stat.RCL / Constants.MAX_STAT;
        rclText.text = stat.RCL.ToString("N0");
        if (stat.RCL >= 99)
        {
            Debug.Log("도전과제 달성: A0001 - RCL 99");
            AchievementManager.Instance.SpawnAchivement("A0001");
        }
        hdlGauge.fillAmount = stat.HDL / Constants.MAX_STAT;
        hdlText.text = stat.HDL.ToString("N0");
        if (stat.HDL >= 99)
        {
            Debug.Log("도전과제 달성: A0002 - HDL 99");
            AchievementManager.Instance.SpawnAchivement("A0002");
        }

        stpGauge.fillAmount = stat.STP / Constants.MAX_STAT;
        stpText.text = stat.STP.ToString("N0");
        if (stat.STP >= 99)
        {
            Debug.Log("도전과제 달성: A0003 - STP 99");
            AchievementManager.Instance.SpawnAchivement("A0003");
        }

        spdGauge.fillAmount = stat.SPD / Constants.MAX_STAT;
        spdText.text = stat.SPD.ToString("N0");
         if (stat.SPD >= 99)
        {
            Debug.Log("도전과제 달성: A0003 - SPD 99");
            AchievementManager.Instance.SpawnAchivement("A0003");
        }

    }

    /// <summary>
    /// 캐릭터가 장착 중인 무기 이미지 UI 갱신 메서드
    /// </summary>
    public void SetEquipImage(Sprite weaponImage)
    {
        equipImage.sprite = weaponImage;
    }
}