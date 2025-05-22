using UnityEngine;
using UnityEngine.UI;

//TODO: 정조준 감도 구현해야함
/// <summary>
/// TestUIScene 오픈
/// option 버튼 눌러서 팝업창 켜지는지 확인
/// myRoom 버튼 눌러서 다른 팝업창 교체 되는지 확인(아무 UI나 넣은것임)
/// Option창에서 사운드 슬라이더 바로 소리 잘 조절 되나 확인
/// Player프리펩 하이어라키에 끌어다 놓기
/// OPtion창의 마우스 감도(비 조준만) 조절 잘 되나 확인
/// Player의 FpsCamera클래스의 sensitivity가 잘 바뀌나 확인
/// </summary>
public class PopupOption : PopupUI
{
    [Header("Mouse Sensitivity")]
    public Text hipSensitivityText;
    public Text adsSensitivityText;
    // public float hipSensitivity = 1f;
    // public float adsSensitivity = 1f;

    // 캐싱된 감도 값을 저장할 static 변수
    [SerializeField] public static float cachedHipSensitivity = 1f;
    public static float cachedAdsSensitivity = 1f;

    private const string HipSensitivityKey = "HipSensitivity";
    private const string AdsSensitivityKey = "ADSSensitivity";


    [Header("Sound Sliders")]
    public Slider masterSlider;
    public Slider seSlider;
    public Slider bgmSlider;

    public Text masterValueText;
    public Text seValueText;
    public Text bgmValueText;

    //private FpsCamera camera;
    public GameObject resumeBtn;

    private void Awake()
    {
        //TODO: 게임씬이 아니면 Resume버튼 비활성화

        //슬라이더 최대 최소값
        masterSlider.minValue = 0;
        masterSlider.maxValue = 100;
        seSlider.minValue = 0;
        seSlider.maxValue = 100;
        bgmSlider.minValue = 0;
        bgmSlider.maxValue = 100;

        // masterSlider.value = SoundManager.Instance.GetMasterVolume() * 100f;

        UpdateSensitivityTexts();

        // 사운드 슬라이더 값 변경 리스너 등록
        masterSlider.onValueChanged.AddListener((v) =>
        {
            masterValueText.text = v.ToString("0");
            SoundManager.Instance.SetMasterVolume(v / 100f);
        });

        seSlider.onValueChanged.AddListener((v) =>
        {
            seValueText.text = v.ToString("0");
            SoundManager.Instance.SetSFXVolume(v / 100f);
        });

        bgmSlider.onValueChanged.AddListener((v) =>
        {
            bgmValueText.text = v.ToString("0");
            SoundManager.Instance.SetMusicVolume(v / 100f);
        });
    }
    private void OnEnable()
    {

        //camera = FindObjectOfType<FpsCamera>();
        // PlayerPrefs 값 다시 불러오기
        LoadSensitivity();

        // 사운드 슬라이더 값 갱신
        masterSlider.value = SoundManager.Instance.MasterVol * 100f;
        seSlider.value = SoundManager.Instance.SFXVol * 100f;
        bgmSlider.value = SoundManager.Instance.BGMVol * 100f;

        UpdateSoundTexts();

        // if(GetComponent<Camera>() == null)
        // {
        //     resumeBtn.SetActive(false);
        // }else
        // {
        //     resumeBtn.SetActive(true);
        // }
    }


    #region Sensitivity Control
    public void ChangeHipSensitivity(float delta)
    {
        cachedHipSensitivity = Mathf.Clamp(cachedHipSensitivity + delta, 0.1f, 9.9f);

        PlayerPrefs.SetFloat(HipSensitivityKey, cachedHipSensitivity);
        UpdateSensitivityTexts();
    }

    public void ChangeADSSensitivity(float delta)//정조준 민감도 아직 구현x
    {
        cachedAdsSensitivity = Mathf.Clamp(cachedAdsSensitivity + delta, 0.1f, 9.9f);
        //cachedAdsSensitivity = adsSensitivity;  // 캐시된 값 갱신
        PlayerPrefs.SetFloat(AdsSensitivityKey, cachedAdsSensitivity);
        UpdateSensitivityTexts();
    }

    private void UpdateSensitivityTexts()// 민감도 텍스트 갱신
    {
        hipSensitivityText.text = cachedHipSensitivity.ToString("0.0");
        adsSensitivityText.text = cachedAdsSensitivity.ToString("0.0");
    }
    #endregion

    private void UpdateSoundTexts()
    {
        masterValueText.text = masterSlider.value.ToString("0");
        seValueText.text = seSlider.value.ToString("0");
        bgmValueText.text = bgmSlider.value.ToString("0");
    }

    public void LoadSensitivity()
    {
        // PlayerPrefs에서 감도 값 불러오기
        cachedHipSensitivity = PlayerPrefs.GetFloat(HipSensitivityKey, 1f);
        cachedAdsSensitivity = PlayerPrefs.GetFloat(AdsSensitivityKey, 1f);

        UpdateSensitivityTexts();
    }

    public void OnClickExitBtn()
    {
        GameManager.GameQuit();
    }

    public void OnClickResumeBtn()
    {
        GameManager.Instance.TogglePopup(false);
    }

    public static void InitSensitivity() //플레이어 생성시 호출할 함수
    {
        cachedAdsSensitivity = PlayerPrefs.GetFloat(AdsSensitivityKey, 1f);
        cachedHipSensitivity = PlayerPrefs.GetFloat(HipSensitivityKey, 1f);
    }

}
