using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class ButtonSelect : MonoBehaviour
{
    public void SelectButton(Button _button)
    {
        _button.Select();
    }

    public void SelectSlider(Slider _slider)
    {
        _slider.Select();
    }

    public void SelectInputField(TMP_InputField _inputField)
    {
        _inputField.Select();
    }
}
