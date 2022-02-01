using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    [SerializeField] InputActionProperty velocityProperty;
    [SerializeField] InputActionProperty positionProperty;

    public Vector3 velocity { get; private set; } = Vector3.zero;
    public Vector3 position { get; private set; }

    private void Update()
    {
        velocity = velocityProperty.action.ReadValue<Vector3>();
        position = positionProperty.action.ReadValue<Vector3>();
    }
}
