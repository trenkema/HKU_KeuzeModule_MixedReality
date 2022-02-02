using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class BirdMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float minFlapControllerVelocity = -0.6f;
    [SerializeField] float groundDistance;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayers;

    [SerializeField] float upForce = 5f;
    [SerializeField] float forwardForce = 10f;
    [SerializeField] float steerForce = 10f;
    [SerializeField] float maxUpVelocity = 25f;

    [Space(5)]

    [SerializeField] float minGlidingVelocity = 15f;
    [SerializeField] float maxGlidingVelocity = 22.5f;
    [SerializeField] float timeToReachMaxGlideVelocity = 3f;

    [Space(5)]

    [SerializeField] float upGlideForce = 10f;
    [SerializeField] float maxGlideControllerVelocity = 0.25f;
    [SerializeField] float maxGlideControllerDifference = 0.2f;

    [Space(5)]
    [SerializeField] float maxSteerControllerVelocity = 0.25f;
    [SerializeField] float minSteerControllerDifference = 0.6f;
    [SerializeField] float steerSmooth = 5f;

    [Header("References")]
    [SerializeField] private CapsuleCollider bodyCollider;
    [SerializeField] private CapsuleCollider grabBodyCollider;

    [Space(5)]

    [SerializeField] Transform leftController;
    [SerializeField] Transform rightController;
    [SerializeField] Controller leftControllerVelocity;
    [SerializeField] Controller rightControllerVelocity;

    [Space(5)]

    [SerializeField] Transform ownCamera;

    Vector3 birdMovementInput;

    float currentGlidingVelocity = 0;

    bool isGrounded = false;

    bool isGliding = false;

    bool isSteering = false;

    Rigidbody rb;

    XRRig xrRig;

    float glideLerpTimeElapsed;
    float glideLerpedValue;

    float groundLerpTimeElapsed;
    Vector3 groundLerpedValue;

    float steerDirection = 0f;

    private void Awake()
    {
        xrRig = GetComponent<XRRig>();
        rb = GetComponent<Rigidbody>();

        rb.centerOfMass = Vector3.zero;
        rb.inertiaTensorRotation = Quaternion.identity;

        currentGlidingVelocity = minGlidingVelocity;
    }

    private void Update()
    {
        // Move To FixedUpdate When Building
        Rotate();

        // Flapping Up
        if (rightControllerVelocity.velocity.y < minFlapControllerVelocity)
        {
            float appliedForce = -rightControllerVelocity.velocity.y;

            rb.AddForce(Vector3.up * upForce * appliedForce);

            //Debug.Log("Velocity: " + rb.velocity.magnitude);

            if (rb.velocity.magnitude > maxUpVelocity)
                rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxUpVelocity);
        }

        // Gliding Check
        if (Mathf.Abs(rightControllerVelocity.velocity.x) <= maxGlideControllerVelocity && !isGrounded)
        {
            Vector3 difference = leftController.localPosition - rightController.localPosition;

            if (Mathf.Abs(difference.y) <= maxGlideControllerDifference)
            {
                Debug.Log("Gliding");

                isGliding = true;
            }
            else
                isGliding = false;
        }
        else
            isGliding = false;

        // Steering Check
        if (Mathf.Abs(rightControllerVelocity.velocity.x) <= maxSteerControllerVelocity && !isGrounded)
        {
            Vector3 difference = leftController.localPosition - rightController.localPosition;

            if (Mathf.Abs(difference.y) >= minSteerControllerDifference)
            {
                Debug.Log("Steering");

                steerDirection = difference.y;

                isSteering = true;
            }
            else
                isSteering = false;
        }
        else
            isSteering = false;

        // Forward Movement
        if (isGrounded)
        {
            if (groundLerpTimeElapsed < 3f)
            {
                groundLerpedValue = Vector3.Lerp(birdMovementInput, Vector3.zero, glideLerpTimeElapsed / timeToReachMaxGlideVelocity);
                groundLerpTimeElapsed += Time.deltaTime;
            }
            else
            {
                groundLerpedValue = Vector3.zero;
            }

            birdMovementInput = groundLerpedValue;

            currentGlidingVelocity = minGlidingVelocity;
            glideLerpTimeElapsed = 0f;
        }
        else if (isGliding && !isGrounded || isSteering && !isGrounded) // Gliding
        {
            groundLerpTimeElapsed = 0f;

            if (glideLerpTimeElapsed < timeToReachMaxGlideVelocity)
            {
                glideLerpedValue = Mathf.Lerp(currentGlidingVelocity, maxGlidingVelocity, glideLerpTimeElapsed / timeToReachMaxGlideVelocity);
                glideLerpTimeElapsed += Time.deltaTime;
            }
            else
            {
                glideLerpedValue = maxGlidingVelocity;
            }

            Vector3 input = (transform.forward * 1f + transform.right * 0f).normalized * glideLerpedValue;

            birdMovementInput = input;

            // Only add Force when falling
            if (rb.velocity.y < -0.5f)
                rb.AddForce(Vector3.up * upGlideForce);
        }
        //else // Diving
        //{
        //    Vector3 input = (transform.forward * 1f + transform.right * 0f).normalized * currentVelocity;
        //    birdMovementInput = Vector3.Lerp(birdMovementInput, input, 1.0f * Time.deltaTime);

        //    currentGlidingVelocity = minGlidingVelocity;
        //    glideLerpTimeElapsed = 0f;
        //}
    }

    private void FixedUpdate()
    {
        // Set height of Player to the actual height of Headset.
        bodyCollider.height = xrRig.cameraInRigSpaceHeight;
        float groundCheckPosition = 1f - (bodyCollider.height / 2);

        groundCheck.localPosition = new Vector3(0f, groundCheckPosition, 0f);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayers);

        if (grabBodyCollider.gameObject.activeInHierarchy)
        {
            grabBodyCollider.height = xrRig.cameraInRigSpaceHeight;
        }

        MoveBird();
    }

    private void MoveBird()
    {
        Vector3 up = new Vector3(0f, rb.velocity.y, 0f);
        rb.velocity = birdMovementInput + up;
    }

    private void Rotate()
    {
        if (isSteering)
        {
            Quaternion curRot = transform.rotation;
            Quaternion newRot = curRot *= Quaternion.Euler(0f, steerForce * steerDirection, 0f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, newRot, Time.fixedDeltaTime * steerSmooth);
        }
    }
}
