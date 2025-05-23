using DataDeclaration;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBaseState : IState
{
    protected PlayerStateMachine stateMachine;

    public PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }
    public virtual void Enter()
    {
        AddInputActionCallbacks();
    }

    public virtual void Exit()
    {
        RemoveInputActionCallbacks();
    }

    protected virtual void AddInputActionCallbacks()
    {
        PlayerController input = stateMachine.Player.Controller;
        input.playerActions.Movement.canceled += OnMovementCanceled;
        input.playerActions.Look.started += OnLookStarted;
        input.playerActions.Attack.started += OnAttack;
        input.playerActions.Reload.started += OnReload;
        input.playerActions.Interact.started += OnInteract;
        input.playerActions.Ads.performed += OnAds;
    }
    protected virtual void RemoveInputActionCallbacks()
    {
        PlayerController input = stateMachine.Player.Controller;
        input.playerActions.Movement.canceled -= OnMovementCanceled;
        input.playerActions.Look.canceled -= OnLookStarted;
        input.playerActions.Attack.started -= OnAttack;
        input.playerActions.Reload.started -= OnReload;
        input.playerActions.Interact.started -= OnInteract;
        input.playerActions.Ads.performed -= OnAds;
    }

    public virtual void HandleInput() // 입력 값
    {
        ReadMovementInput();
    }

    public virtual void PhysicsUpdate()
    {
    }

    public virtual void Update()
    {
        Move();
    }

    protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
    {

    }

    protected virtual void OnLookStarted(InputAction.CallbackContext context)
    {
        Vector2 lookDelta = context.ReadValue<Vector2>();
    }

    protected virtual void OnAttack(InputAction.CallbackContext context)
    {

    }

    protected virtual void OnReload(InputAction.CallbackContext context)
    {

    }

    protected virtual void OnAds(InputAction.CallbackContext context)
    {
        stateMachine.IsAds = !stateMachine.IsAds;
        stateMachine.Player.AdsCamera.gameObject.SetActive(stateMachine.IsAds);
        stateMachine.Player.NonAdsCamera.gameObject.SetActive(!stateMachine.IsAds);
    }

    protected virtual void OnInteract(InputAction.CallbackContext context)
    {
        var ray = new Ray(stateMachine.Player.Controller.transform.position, stateMachine.Player.Controller.transform.forward);
        RaycastHit hit;
        var layerMask = 1 << LayerMask.NameToLayer("Interactable");
        if (Physics.Raycast(ray, out hit, 5f, layerMask))
        {
            var interact = hit.collider.GetComponent<IInteract>();
            if (interact != null)
            {
                interact.Interact();
            }
        }
    }
    protected void StartAnimation(int animatorHash)
    {
        stateMachine.Player.Animator.SetBool(animatorHash, true);
    }

    protected void StopAnimation(int animatorHash)
    {
        stateMachine.Player.Animator.SetBool(animatorHash, false);
    }


    private void ReadMovementInput()
    {
        stateMachine.MovementInput = stateMachine.Player.Controller.playerActions.Movement.ReadValue<Vector2>();
        stateMachine.MouseInput = stateMachine.Player.Controller.playerActions.Look.ReadValue<Vector2>();
    }

    private void Move()
    {
        Vector3 movementDirection = GetMovementDirection();
        float movementSpeed = GetMovementSpeed();
        stateMachine.Player.Controller.CharacterController.Move(((movementDirection * movementSpeed) + stateMachine.Player.Controller.YMovement) * Time.deltaTime);
        
        RotateView();
    }
    
    private Vector3 GetMovementDirection()
    {
        Vector3 forward = stateMachine.Player.transform.forward;
        Vector3 right = stateMachine.Player.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        return forward * stateMachine.MovementInput.y + right * stateMachine.MovementInput.x;
    }

    private float GetMovementSpeed()
    {
        float baseSpeed = stateMachine.Player.Stat.SPD * stateMachine.Player.speedMultiplier;
        float finalSpeed = baseSpeed;

        if (stateMachine.IsAds)
        {
            finalSpeed *= stateMachine.Player.adsSpeedMultiplier;  // 조준 중일 때 속도 감소
        }
        return finalSpeed;
    }
    private void RotateView() //좌우 회전
    {
        float sensitivity = stateMachine.CurrentSensitivity * 0.1f;

        stateMachine.RotationX -= stateMachine.MouseInput.y * sensitivity;

        // 전체 회전 계산 (마우스 + 반동)
        float totalX = stateMachine.RotationX + stateMachine.RecoilOffsetX;

        totalX = Mathf.Clamp(totalX, -70f, 70f);

        // RotationX에서 RecoilOffsetX를 다시 계산해서 RotationX만 조정 (순수 마우스 기준)
        stateMachine.RotationX = totalX - stateMachine.RecoilOffsetX;

        Quaternion offsetRotation = Quaternion.Euler(-90f, 0f, 0f);
        stateMachine.Player.Motion.ArmTransform.localRotation = Quaternion.Euler(totalX, 0, 0) * offsetRotation;

        stateMachine.Player.transform.Rotate(Vector3.up * stateMachine.MouseInput.x * sensitivity);
    }
}
