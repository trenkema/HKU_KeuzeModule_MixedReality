using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference jumpActionReference;
    [SerializeField] private float jumpForce = 500.0f;

    [SerializeField] private CapsuleCollider bodyCollider;
    [SerializeField] private CapsuleCollider grabBodyCollider;

    Rigidbody rb;
    private XRRig xrRig;

    private void Start()
    {
        xrRig = GetComponent<XRRig>();
        rb = GetComponent<Rigidbody>();
        jumpActionReference.action.performed += OnJump;
    }

    private void FixedUpdate()
    {
        bodyCollider.height = xrRig.cameraInRigSpaceHeight;

        if (grabBodyCollider.gameObject.activeInHierarchy)
        {
            grabBodyCollider.height = xrRig.cameraInRigSpaceHeight;
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        rb.AddForce(Vector3.up * jumpForce);
    }
}
