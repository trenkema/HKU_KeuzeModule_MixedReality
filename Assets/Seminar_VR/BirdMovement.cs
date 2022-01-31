using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class BirdMovement : MonoBehaviour
{
    [Header("Settings")]
    [Range(0, 360)] [SerializeField] float defaultAngle = 25f;
    [Range(0, 360)] [SerializeField] float minAngle = 50f;
    [Range(0, 360)] [SerializeField] float maxAngle = 90f;

    [Space(5)]

    [SerializeField] float minFlapControllerVelocity = -0.6f;

    [SerializeField] float upForce = 5f;
    [SerializeField] float forwardForce = 10f;
    [SerializeField] float steerForce = 10f;

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

    [Header("References")]
    [SerializeField] InputActionReference torqueRotationRightReference;

    [Space(5)]

    [SerializeField] private CapsuleCollider bodyCollider;
    [SerializeField] private CapsuleCollider grabBodyCollider;

    [Space(5)]

    [SerializeField] Transform leftController;
    [SerializeField] Transform rightController;
    [SerializeField] ControllerVelocity rightControllerVelocity;

    [Space(5)]

    [SerializeField] Transform ownCamera;

    Vector3 birdMovementInput;

    float currentVelocity = 0f;

    float currentGlidingVelocity = 0;

    bool isGrounded = false;

    bool isGliding = false;

    bool isSteering = false;

    Rigidbody rb;

    XRRig xrRig;

    float glideLerpTimeElapsed;
    float glideLerpedValue;

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
        if (rightControllerVelocity.velocity.x < minFlapControllerVelocity)
        {
            float appliedForce = -rightControllerVelocity.velocity.x;

            currentVelocity = forwardForce * appliedForce;

            rb.AddForce(Vector3.up * upForce * appliedForce);
        }

        // Gliding Check
        if (Mathf.Abs(rightControllerVelocity.velocity.x) <= maxGlideControllerVelocity)
        {
            float difference = leftController.position.z - rightController.position.z;

            if (Mathf.Abs(difference) < maxGlideControllerDifference)
            {
                isGliding = true;
            }
            else
                isGliding = false;
        }
        else
            isGliding = false;

        // Steering Check
        if (Mathf.Abs(rightControllerVelocity.velocity.x) < maxSteerControllerVelocity)
        {
            Vector3 difference = leftController.position - rightController.position;

            Debug.Log("Difference: " + difference);

            //if (Mathf.Abs(difference) > minSteerControllerDifference)
            //{
            //    isSteering = true;
            //}
            //else
            //    isSteering = false;
        }
        else
            isSteering = false;

        // Forward Movement
        if (isGrounded)
        {
            birdMovementInput = Vector3.Lerp(birdMovementInput, Vector3.zero, 0.5f * Time.deltaTime);
            currentVelocity = 0f;
            currentGlidingVelocity = minGlidingVelocity;
            glideLerpTimeElapsed = 0f;
        }
        else if (isGliding && !isGrounded) // Gliding
        {
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

            birdMovementInput = Vector3.Lerp(birdMovementInput, input, 3.0f * Time.deltaTime);

            // Only add Force when falling
            if (rb.velocity.y < 0f)
                rb.AddForce(Vector3.up * upGlideForce);
        }
        else if (isSteering && !isGrounded) // Steering
        {

        }
        else // Diving
        {
            Vector3 input = (transform.forward * 1f + transform.right * 0f).normalized * currentVelocity;
            birdMovementInput = Vector3.Lerp(birdMovementInput, input, 1.0f * Time.deltaTime);

            currentGlidingVelocity = minGlidingVelocity;
            glideLerpTimeElapsed = 0f;
        }

        #region oldCode
        //if (Mathf.Abs(controllerVelocity.velocity.x) < maxGlideVelocity)
        //{
        //    float difference = leftController.position.y - rightController.position.y;

        //    Debug.Log("Difference: " + difference);

        //    if (difference > 0)
        //    {
        //        //if (previousDirection != 0)
        //        //{
        //        //    rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);

        //        //    previousDirection = 0;
        //        //}

        //        //rb.AddTorque(rb.transform.right * steerForce);

        //        //rb.velocity = new Vector3((transform.right * steerForce * Time.deltaTime).x, rb.velocity.y, rb.velocity.z);

        //        //rb.AddForce(transform.right * steerForce);
        //        //xrRig.transform.Rotate(0, steerSpeedRig, 0);
        //        //rb.rotation = (Quaternion.Euler(rb.rotation.eulerAngles.x, rb.rotation.eulerAngles.y + steerForce, rb.rotation.eulerAngles.z));
        //        rb.AddForce(Vector3.up * upGlideForce);
        //    }
        //    else
        //    {
        //        //if (previousDirection != 1)
        //        //{
        //        //    rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);

        //        //    previousDirection = 1;
        //        //}

        //        //rb.AddTorque(rb.transform.right * -steerForce);

        //        //rb.velocity = new Vector3((-transform.right * steerForce * Time.deltaTime).x, rb.velocity.y, rb.velocity.z);
        //        //rb.AddForce(-transform.right * steerForce);
        //        //xrRig.transform.Rotate(0, -steerSpeedRig, 0);
        //        //rb.rotation = (Quaternion.Euler(rb.rotation.eulerAngles.x, rb.rotation.eulerAngles.y + -steerForce, rb.rotation.eulerAngles.z));
        //        rb.AddForce(Vector3.up * upGlideForce);
        //    }
        //}

        #endregion
    }

    private void FixedUpdate()
    {
        // Set height of Player to the actual height of Headset.
        bodyCollider.height = xrRig.cameraInRigSpaceHeight;

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
            transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * 2f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}
