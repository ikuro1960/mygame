using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("レシート連携")]
    [Tooltip("レシート管理スクリプト（開いている間は操作を停止）")]
    [SerializeField] private ReceiptController receiptController;

    [Header("移動設定")]
    [Tooltip("通常の移動速度 (m/s)")]
    [SerializeField] private float moveSpeed = 5.0f;

    [Tooltip("ダッシュ時の移動速度 (m/s)")]
    [SerializeField] private float sprintSpeed = 8.0f;

    [Header("ジャンプ・重力設定")]
    [Tooltip("ジャンプの高さ (m)")]
    [SerializeField] private float jumpHeight = 1.2f;

    [Tooltip("重力加速度")]
    [SerializeField] private float gravity = -15.0f;

    [Header("視点操作 (一人称カメラ)")]
    [Tooltip("プレイヤーの子オブジェクトとして配置する一人称カメラ")]
    [SerializeField] private Transform cameraTransform;

    [Tooltip("マウス感度")]
    [SerializeField] private float mouseSensitivity = 0.15f;

    [Tooltip("ゲームパッド視点感度")]
    [SerializeField] private float gamepadLookSensitivity = 120.0f;

    [Tooltip("上下の視点回転制限（度）")]
    [SerializeField] private float verticalLookLimit = 85.0f;

    private CharacterController characterController;
    private Vector3 verticalVelocity;
    private bool isGrounded;
    private float cameraPitch = 0.0f;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction sprintAction;
    private InputAction jumpAction;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (cameraTransform == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
            {
                cameraTransform = cam.transform;
            }
        }

        SetupInputs();
    }

    private void Start()
    {
        Vector3 euler = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, euler.y, 0f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void SetupInputs()
    {
        moveAction = new InputAction("Move", InputActionType.Value, expectedControlType: "Vector2");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
        moveAction.AddBinding("<Gamepad>/leftStick");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Gamepad>/dpad/up")
            .With("Down", "<Gamepad>/dpad/down")
            .With("Left", "<Gamepad>/dpad/left")
            .With("Right", "<Gamepad>/dpad/right");

        lookAction = new InputAction("Look", InputActionType.Value, expectedControlType: "Vector2");
        lookAction.AddBinding("<Mouse>/delta");
        lookAction.AddBinding("<Gamepad>/rightStick");

        sprintAction = new InputAction("Sprint", InputActionType.Button);
        sprintAction.AddBinding("<Keyboard>/leftShift");
        sprintAction.AddBinding("<Gamepad>/leftStickPress");

        jumpAction = new InputAction("Jump", InputActionType.Button);
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");
    }

    private void OnEnable()
    {
        moveAction?.Enable();
        lookAction?.Enable();
        sprintAction?.Enable();
        jumpAction?.Enable();
    }

    private void OnDisable()
    {
        moveAction?.Disable();
        lookAction?.Disable();
        sprintAction?.Disable();
        jumpAction?.Disable();
    }

    private void OnDestroy()
    {
        moveAction?.Dispose();
        lookAction?.Dispose();
        sprintAction?.Dispose();
        jumpAction?.Dispose();
    }

    private void Update()
    {
        // レシートが開いている間は操作を停止
        if (receiptController != null && receiptController.IsReceiptOpen)
        {
            HandleGrounded();
            ApplyGravity();
            return;
        }

        HandleLook();
        HandleGrounded();
        HandleMovement();
        HandleJump();
        ApplyGravity();

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked) ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = (Cursor.lockState != CursorLockMode.Locked);
        }
    }

    private void HandleLook()
    {
        if (lookAction == null) return;

        Vector2 lookDelta = lookAction.ReadValue<Vector2>();

        float sens = mouseSensitivity;
        if (Gamepad.current != null && Gamepad.current.rightStick.ReadValue().sqrMagnitude > 0.01f)
        {
            sens = gamepadLookSensitivity * Time.deltaTime;
        }

        float mouseX = lookDelta.x * sens;
        float mouseY = lookDelta.y * sens;

        transform.Rotate(Vector3.up * mouseX);

        if (cameraTransform != null)
        {
            cameraPitch -= mouseY;
            cameraPitch = Mathf.Clamp(cameraPitch, -verticalLookLimit, verticalLookLimit);
            cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        }
    }

    private void HandleGrounded()
    {
        isGrounded = characterController.isGrounded;

        if (isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2.0f;
        }
    }

    private void HandleMovement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        Vector3 moveDirection = transform.forward * input.y + transform.right * input.x;
        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        bool isSprinting = sprintAction.IsPressed();
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        characterController.Move(moveDirection * (currentSpeed * Time.deltaTime));
    }

    private void HandleJump()
    {
        if (isGrounded && jumpAction.WasPressedThisFrame())
        {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
        }
    }

    private void ApplyGravity()
    {
        verticalVelocity.y += gravity * Time.deltaTime;
        characterController.Move(verticalVelocity * Time.deltaTime);
    }
}