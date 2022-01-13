using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class CheckForInput : MonoBehaviour
{ 
    [SerializeField] TMP_InputField inputField;
    [SerializeField] ScrollRect scrollRect;

    public void DeselectInputField(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (inputField.isFocused)
            {
                inputField.DeactivateInputField(false);
            }
        }
    }

    public void NagivateController(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (inputField.isFocused)
            {
                inputField.DeactivateInputField(false);
            }
        }
    }
}
