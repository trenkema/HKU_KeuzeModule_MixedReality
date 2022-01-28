using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerVelocity : MonoBehaviour
{
    [SerializeField] InputActionProperty velocityProperty;

    public Vector3 velocity { get; private set; } = Vector3.zero;

    private void Update()
    {
        velocity = velocityProperty.action.ReadValue<Vector3>();
        int velocityX = Mathf.RoundToInt(velocity.x);
    }
}
