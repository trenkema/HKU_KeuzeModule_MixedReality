using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController_NonVR : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float moveSpeed;

    [SerializeField] private float jumpForce;

    [Header("Player Look")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float minXLook;
    [SerializeField] private float maxXLook;
    [SerializeField] private float lookSensitivity;

    [SerializeField] private float smoothInputSpeed = 0.2f;

    [SerializeField] private float flyUpTime = 0.25f;

    private float curCamRotX;
    private Vector2 mouseDelta;

    [SerializeField] Rigidbody rb;

    public bool canLook = true;
    private Vector2 curMovementInput;
    Vector2 smoothInputVelocity;

    private bool isMoving = false;
    private bool isScaling = false;
    private bool isRotating = false;
    private bool isFlying = false;
    private bool canFlyUpDown = true;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        EventSystem<bool>.Subscribe(EventType.TOGGLE_CURSOR, ToggleCursor);
    }

    private void LateUpdate()
    {
        if (canLook && !isScaling && !isRotating)
        {
            CameraLook();
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (!isMoving)
        {
            curMovementInput = Vector2.SmoothDamp(curMovementInput, Vector2.zero, ref smoothInputVelocity, smoothInputSpeed);
        }

        Vector3 direction = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        direction *= moveSpeed;
        direction.y = rb.velocity.y;

        rb.velocity = direction;
    }

    private void CameraLook()
    {
        curCamRotX += mouseDelta.y * lookSensitivity;
        curCamRotX = Mathf.Clamp(curCamRotX, minXLook, maxXLook);
        cameraHolder.localEulerAngles = new Vector3(-curCamRotX, 0, 0);

        playerTransform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            isMoving = true;
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isMoving = false;
            //curMovementInput = Vector2.SmoothDamp(curMovementInput, Vector2.zero, ref smoothInputVelocity, smoothInputSpeed);
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (!isFlying)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            else if (isFlying && canFlyUpDown)
            {
                canFlyUpDown = false;
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                StartCoroutine(FlyUpDown());
            }
        }
    }

    public void OnFlyDownInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (isFlying && canFlyUpDown)
            {
                canFlyUpDown = false;
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                rb.AddForce(Vector3.down * (jumpForce / 2f), ForceMode.Impulse);
                StartCoroutine(FlyUpDown());
            }
        }
    }

    IEnumerator FlyUpDown()
    {
        yield return new WaitForSeconds(flyUpTime);
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        canFlyUpDown = true;
    }

    public bool IsCursorActive()
    {
        return canLook;
    }

    public void ToggleCursor(bool _toggle)
    {
        Cursor.lockState = _toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !_toggle;
    }

    public void ToggleScaling(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            EventSystem<bool>.RaiseEvent(EventType.IS_SCALING, true);
            isScaling = true;
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            EventSystem<bool>.RaiseEvent(EventType.IS_SCALING, false);
            isScaling = false;
        }
    }

    public void ToggleRotating(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            EventSystem<bool>.RaiseEvent(EventType.IS_ROTATING, true);
            isRotating = true;
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            EventSystem<bool>.RaiseEvent(EventType.IS_ROTATING, false);
            isRotating = false;
        }
    }

    public void ToggleFlight(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            isFlying = !isFlying;

            if (isFlying)
            {
                rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            }
            else
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
    }
}
