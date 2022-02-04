using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class BirdMovement : MonoBehaviour
{
    public bool canFlyForwards = false;
    public bool hasStarted = false;
    public bool canMove = false;

    [Header("Settings")]
    [SerializeField] Transform resetPoint;
    [SerializeField] InputActionReference resetPointReference;

    [SerializeField] float delayAtStart = 5f;

    [SerializeField] float minFlapControllerVelocity = -0.6f;
    [SerializeField] float groundDistance;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayers;
    [SerializeField] float timeToReachGroundVelocity = 3f;

    [SerializeField] float upForce = 5f;
    [SerializeField] float forwardForce = 10f;
    [SerializeField] float steerForce = 10f;
    [SerializeField] float maxUpVelocity = 25f;

    [Space(5)]
    [SerializeField] float maxBrakeRotation = 0.25f;
    [SerializeField] float brakeSpeed = 1f;

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
    [SerializeField] private SphereCollider bodyCollider;

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

    bool isBraking = false;

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

        canMove = false;
    }

    private void Start()
    {
        resetPointReference.action.performed += ResetPoint;

        Invoke("CanMove", delayAtStart);
    }

    private void Update()
    {
        if (!canMove)
        {
            rb.velocity = Vector3.zero;

            return;
        }

        // Braking Check
        if (Mathf.Abs(rightControllerVelocity.rotation.x) < maxBrakeRotation)
        {
            isBraking = true;
        }
        else
            isBraking = false;

        // Flapping Up
        if (rightControllerVelocity.velocity.y < minFlapControllerVelocity)
        {
            float appliedForce = -rightControllerVelocity.velocity.y;

            rb.AddForce(Vector3.up * upForce * appliedForce);

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
                Debug.Log("Steering: " + difference.y);

                steerDirection = difference.y;

                isSteering = true;
            }
            else
                isSteering = false;
        }
        else
        {
            steerDirection = 0f;
            isSteering = false;
        }

        // Forward Movement
        if (isGrounded)
        {
            if (groundLerpTimeElapsed < 3f)
            {
                groundLerpedValue = Vector3.Lerp(birdMovementInput, Vector3.zero, glideLerpTimeElapsed / timeToReachGroundVelocity);
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
            if (!isBraking)
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

                currentGlidingVelocity = glideLerpedValue;

                birdMovementInput = input;
            }
            else
            {
                // Braking
                groundLerpTimeElapsed = 0f;
                glideLerpTimeElapsed = 0f;

                currentGlidingVelocity -= currentGlidingVelocity / brakeSpeed;

                Vector3 input = (transform.forward * 1f + transform.right * 0f).normalized * currentGlidingVelocity;

                birdMovementInput = input;
            }

            // Only add Force when falling
            if (rb.velocity.y < -0.5f && !isBraking)
                rb.AddForce(Vector3.up * upGlideForce);
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayers);

        MoveBird();

        //Rotate();
    }

    private void LateUpdate()
    {
        Rotate();
    }

    private void CanMove()
    {
        canMove = true;
    }

    private void MoveBird()
    {
        if (!canMove)
            return;

        Vector3 up = new Vector3(0f, rb.velocity.y, 0f);

        if (canFlyForwards)
            rb.velocity = birdMovementInput + up;
        else
            rb.velocity = up;
    }

    private void Rotate()
    {
        if (isSteering)
        {
            Quaternion curRot = transform.rotation;
            Quaternion newRot = curRot *= Quaternion.Euler(0f, steerForce * steerDirection, 0f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, newRot, Time.fixedDeltaTime * steerSmooth);
            //transform.rotation = Quaternion.Slerp(transform.rotation, newRot, Time.fixedDeltaTime);
        }
    }

    public void ResetPoint(InputAction.CallbackContext _context)
    {
        if (canFlyForwards)
        {
            transform.position = resetPoint.position;
        }    
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!hasStarted)
            return;

        if (collision.gameObject.tag == "Ground")
        {
            Debug.Log("HIT");

            canFlyForwards = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!hasStarted)
            return;

        if (collision.gameObject.tag == "Ground")
        {
            canFlyForwards = true;
        }
    }
}
