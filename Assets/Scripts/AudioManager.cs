using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioMixer mixer;
    public AudioSetting[] audioSettings;
    private enum AudioGroups { Music, SFX };

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Initialize all the Parameters
        for (int i = 0; i < audioSettings.Length; i++)
        {
            audioSettings[i].Initialize();
        }
    }

    // Call SetExposedParam to set the volume of Music and save it in a PlayerPref
    public void SetMusicLevel(float value)
    {
        audioSettings[(int)AudioGroups.Music].SetExposedParam(value);
        PlayerPrefs.Save();
    }

    // Call SetExposedParam to set the volume of SFX and save it in a PlayerPref
    public void SetSFXVolume(float value)
    {
        audioSettings[(int)AudioGroups.SFX].SetExposedParam(value);
        PlayerPrefs.Save();
    }
}

[System.Serializable]
public class AudioSetting
{
    public Slider slider;
    public string ExposedParameter;

    public void Initialize()
    {
        // Get and Set value to the Parameter Value, if it doesn't exist set it to 0.25f
        slider.value = PlayerPrefs.GetFloat(ExposedParameter, 0.25f);
    }

    // Set the volume of the Parameter and save it in a PlayerPref
    public void SetExposedParam(float value) // 1
    {
        AudioManager.instance.mixer.SetFloat(ExposedParameter, Mathf.Log10(value) * 20); // 3
        PlayerPrefs.SetFloat(ExposedParameter, value); // 4
    }
}