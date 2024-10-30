using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float initialSpeed = 5f;
    public float moveSpeed = 5f;
    public float jumpPower = 80f;
    public float rollSpeed = 8f;
    public float rollStamina = 40f;
    private Vector2 curMovementInput;
    private bool isRolled = false;
    private float rollingTime;

    [Header("Look")]
    public Transform cameraContainer;
    [SerializeField] private float minXLook;
    [SerializeField] private float maxXLook;
    [SerializeField] private float lookSensitivity;
    private float camCurXRot;
    private Vector2 mouseDelta;
    public bool canLook = true;

    [SerializeField] private LayerMask groundLayerMask;
    private Rigidbody _rigidbody;

    public event Action inventory;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (rollingTime == 0)
        {
            Move(moveSpeed);
        }
        else
        {
            rollingTime -= Time.deltaTime;
            if(rollingTime <= 0) rollingTime = 0;
        }
        if (IsGrounded())
        {
            rollingTime = 0;
            isRolled = false;
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void LateUpdate()
    {
        if (canLook)
        {
            Look();
        }
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    private void Move(float speed)
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= speed;
        dir.y = _rigidbody.velocity.y;

        _rigidbody.velocity = dir;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    private void Look()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            _rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }

        if (context.phase == InputActionPhase.Started && !IsGrounded() && !isRolled && curMovementInput != Vector2.zero)        // 2단 점프시 구르기
        {
            Move(rollSpeed);
            CharacterManager.Instance.Player.condition.UseStamina(rollStamina);
            rollingTime = 3;
            isRolled = true;
        }
    }

    private bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + transform.forward * 0.2f + transform.up * 0.01f, Vector3.down),
            new Ray(transform.position - transform.forward * 0.2f + transform.up * 0.01f, Vector3.down),
            new Ray(transform.position + transform.right * 0.2f + transform.up * 0.01f, Vector3.down),
            new Ray(transform.position - transform.right * 0.2f + transform.up * 0.01f, Vector3.down)
        };

        for(int i=0; i<rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            Toggle();   
        }
    }

    public void OnQuickslotInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            float value = context.ReadValue<float>();
            Debug.Log($"Value : {value}");
        }
    }
    private void Toggle()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }

    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
    }
    public void SetOriginalSpeed()
    {
        moveSpeed = initialSpeed;
    }
}
