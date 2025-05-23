using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float verticalVelocity;
    public Vector3 YMovement => Vector3.up * verticalVelocity;

    public PlayerInputs playerInputs { get; private set; }
    public PlayerInputs.PlayerActions playerActions { get; private set; }

    public CharacterController CharacterController { get; private set; }

    private void Awake()
    {
        playerInputs = new PlayerInputs();
        playerActions = playerInputs.Player;
        CharacterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        playerInputs.Enable();
    }

    private void Update()
    {
        if (CharacterController.isGrounded)
        {
            verticalVelocity = Physics.gravity.y * Time.deltaTime; // 땅에 붙어있으면 유지
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime; // 아니면 중력 더해주기
        }
    }

    private void OnDisable()
    {
        playerInputs.Disable();
    }
    public void EnablePlayerInput(bool isEnable) //플레이어 인풋 정지(옵션창 띄울떄 마우스 회전 x)
    {
        if (isEnable)
        {
            playerInputs?.Enable();
        }
        else
        {
            playerInputs?.Disable();
        }
    }
}