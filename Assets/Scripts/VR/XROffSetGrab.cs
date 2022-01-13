using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XROffSetGrab : XRGrabInteractable
{
    private Vector3 interactorPosition = Vector3.zero;
    private Quaternion interactorRotation = Quaternion.identity;
    private Collider grabBodyCollider;

    private void Start()
    {
        grabBodyCollider = GameManager.Instance.grabBodyCollider;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if (args.interactor.GetType() != typeof(XRSocketInteractorTag))
        {
            StoreInteractor(args.interactor);
            MatchAttachmentPoints(args.interactor);
            Rigidbody rb = args.interactable.gameObject.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;
            //Physics.IgnoreCollision(grabBodyCollider, args.interactable.gameObject.GetComponent<Collider>(), !GameManager.Instance.GetShapeColliding());
            grabBodyCollider.gameObject.SetActive(false);
        }
    }

    private void StoreInteractor(XRBaseInteractor interactor)
    {
        interactorPosition = interactor.attachTransform.localPosition;
        interactorRotation = interactor.attachTransform.localRotation;
    }

    private void MatchAttachmentPoints(XRBaseInteractor interactor)
    {
        bool hasAttach = attachTransform != null;
        interactor.attachTransform.position = hasAttach ? attachTransform.position : transform.position;
        interactor.attachTransform.rotation = hasAttach ? attachTransform.rotation : transform.rotation;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (args.interactor.GetType() != typeof(XRSocketInteractorTag))
        {
            ResetAttachmentPoints(args.interactor);
            ClearInteractor(args.interactor);
            Rigidbody rb = args.interactable.gameObject.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            grabBodyCollider.gameObject.SetActive(true);
        }
    }

    private void ResetAttachmentPoints(XRBaseInteractor interactor)
    {
        interactor.attachTransform.localPosition = interactorPosition;
        interactor.attachTransform.localRotation = interactorRotation;
    }

    private void ClearInteractor(XRBaseInteractor interactor)
    {
        interactorPosition = Vector3.zero;
        interactorRotation = Quaternion.identity;
    }
}
