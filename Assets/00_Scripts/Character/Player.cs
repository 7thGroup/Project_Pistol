using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    [Header("캐릭터 수치"), Tooltip("정조준 시 이동속도 배율"), Range(0f, 1f)]
    public float adsSpeedMultiplier = 0.01f;
    [Tooltip("기본 이동속도 배율"), Range(0f, 1f)] public float speedMultiplier = 0.1f;

    [Space(20), Header("무기"), Tooltip("무기 장착 위치"), SerializeField]
    private Transform weaponPos;

    public PlayerStateMachine stateMachine;

    #region Properties

    public CharacterSO Data { get; private set; }
    public PlayerStatHandler Stat { get; private set; }

    public Weapon Weapon { get; private set; }

    public PlayerController Controller { get; private set; }

    public PlayerAnimationData AnimationData { get; private set; }
    public Animator Animator { get; private set; }
    public PlayerMotion Motion { get; private set; }

    [field: SerializeField] public CinemachineVirtualCamera NonAdsCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCamera AdsCamera { get; private set; }

    [field: SerializeField] public CinemachineImpulseSource impulseSource;

    #endregion

    private void Awake()
    {
        InitPlayer();

        InitWeapon(GameManager.Instance.selectedWeapon.ID);
        

        InitCamera();
    }

    private void Start()
    {
        stateMachine.ChangeState(stateMachine.IdleState);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.TogglePopup(true);
        }

        stateMachine.HandleInput();
        stateMachine.Update();

        if (stateMachine.MovementInput.magnitude > 0)
        {
            Motion.HeadbobUpdate();
        }
    }

    private void FixedUpdate()
    {
        stateMachine.PhysicsUpdate();
    }

    private void InitPlayer()
    {
        if (Data == null)
        {
            Data = GameManager.Instance.selectedCharacter;
        }

        Stat = new PlayerStatHandler(this);
        stateMachine = new PlayerStateMachine(this);
        AnimationData = new PlayerAnimationData();

        Animator = GetComponent<Animator>();

        Controller = GetComponent<PlayerController>();
        Motion = GetComponent<PlayerMotion>();
        PopupOption.InitSensitivity();
    }

    private void InitCamera()
    {
        if (AdsCamera && NonAdsCamera)
        {
            return;
        }

        var virtualCams = GetComponentsInChildren<CinemachineVirtualCamera>();
        if (virtualCams[0].name.Equals("ADSVirtualCam"))
        {
            AdsCamera = virtualCams[0];
            NonAdsCamera = virtualCams[1];
        }
        else
        {
            AdsCamera = virtualCams[1];
            NonAdsCamera = virtualCams[0];
        }
    }

    private void InitWeapon(string weaponID)
    {
        if (weaponPos == null)
        {
            weaponPos = transform.FindDeepChildByName("WeaponPos");
        }

        var resource = ResourceManager.Instance.Load<Weapon>($"Prefabs/Weapon/{weaponID}");
        Weapon = Instantiate(resource, weaponPos); // 프리팹 원본 transform 유지
    }

}