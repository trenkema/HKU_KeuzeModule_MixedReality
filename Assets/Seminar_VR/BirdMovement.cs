using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class BirdMovement : MonoBehaviour
{
    [SerializeField] float defaultAngle = 25f;
    [SerializeField] float minAngle = 50f;
    [SerializeField] float maxAngle = 90f;
    [SerializeField] float upForce = 5f;

    [SerializeField] float maxSpeedSideways = 10f;

    [SerializeField] float forwardForce = 10f;
    [SerializeField] float steerForce = 10f;
    [SerializeField] float steerSpeedRig = 2f;
    [SerializeField] float upGlideForce = 10f;

    [SerializeField] float maxGlideVelocity;

    [SerializeField] private CapsuleCollider bodyCollider;
    [SerializeField] private CapsuleCollider grabBodyCollider;

    [SerializeField] Transform leftController;
    [SerializeField] Transform rightController;

    [SerializeField] ControllerVelocity controllerVelocity;

    [SerializeField] Transform wingMiddle;

    [SerializeField] Transform ownCamera;

    [SerializeField] Rigidbody rb;

    XRRig xrRig;

    int previousDirection = -1;

    private void Awake()
    {
        xrRig = GetComponent<XRRig>();
        //rb = GetComponent<Rigidbody>();

        rb.centerOfMass = Vector3.zero;
        rb.inertiaTensorRotation = Quaternion.identity;
    }

    private void Update()
    {
        if (controllerVelocity.velocity.x < 0)
        {
            float appliedForce = -controllerVelocity.velocity.x;
            rb.AddForce(Vector3.up * appliedForce * upForce);
            rb.AddForce(transform.forward * appliedForce * forwardForce);
        }

        Debug.Log(Mathf.Abs(controllerVelocity.velocity.x));

        if (Mathf.Abs(controllerVelocity.velocity.x) < maxGlideVelocity)
        {
            float difference = leftController.position.y - rightController.position.y;

            Debug.Log("Difference: " + difference);

            if (difference > 0)
            {
                //if (previousDirection != 0)
                //{
                //    rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);

                //    previousDirection = 0;
                //}

                rb.AddTorque(Vector3.up * steerForce);

                //rb.velocity = new Vector3((transform.right * steerForce * Time.deltaTime).x, rb.velocity.y, rb.velocity.z);

                //rb.AddForce(transform.right * steerForce);
                //xrRig.transform.Rotate(0, steerSpeedRig, 0);
                //rb.rotation = (Quaternion.Euler(rb.rotation.eulerAngles.x, rb.rotation.eulerAngles.y + steerForce, rb.rotation.eulerAngles.z));
                rb.AddForce(Vector3.up * upGlideForce);
            }
            else
            {
                //if (previousDirection != 1)
                //{
                //    rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);

                //    previousDirection = 1;
                //}

                rb.AddTorque(Vector3.up * -steerForce);

                //rb.velocity = new Vector3((-transform.right * steerForce * Time.deltaTime).x, rb.velocity.y, rb.velocity.z);
                //rb.AddForce(-transform.right * steerForce);
                //xrRig.transform.Rotate(0, -steerSpeedRig, 0);
                //rb.rotation = (Quaternion.Euler(rb.rotation.eulerAngles.x, rb.rotation.eulerAngles.y + -steerForce, rb.rotation.eulerAngles.z));
                rb.AddForce(Vector3.up * upGlideForce);
            }
        }

        //if (controllerVelocity.velocity.x < 0f && controllerVelocity.velocity.x > maxGlideVelocity)
        //{
        //    rb.AddForce(ownCamera.forward * 1f);
        //    rb.AddForce(Vector3.up * 1f);
        //    Debug.Log("Flying Forward");
        //}
    }

    private void FixedUpdate()
    {
        bodyCollider.height = xrRig.cameraInRigSpaceHeight;

        if (grabBodyCollider.gameObject.activeInHierarchy)
        {
            grabBodyCollider.height = xrRig.cameraInRigSpaceHeight;
        }
    }
}
