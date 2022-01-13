using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetSensitivity : MonoBehaviour
{
    private float mouseSensitivityX, mouseSensitivityY;
    public Slider sliderX;
    public Slider sliderY;

    private void Awake()
    {
        mouseSensitivityX = PlayerPrefs.GetFloat("MouseSensitivityX", 175f);
        mouseSensitivityY = PlayerPrefs.GetFloat("MouseSensitivityY", 175f);
        sliderX.value = mouseSensitivityX;
        sliderY.value = mouseSensitivityY;
    }

    public void SetSensitivityX(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivityX", value);
        PlayerPrefs.Save();
    }

    public void SetSensitivityY(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivityY", value);
        PlayerPrefs.Save();
    }
}
