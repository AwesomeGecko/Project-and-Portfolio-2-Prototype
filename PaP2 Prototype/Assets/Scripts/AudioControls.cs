using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioControls : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider mainSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else 
        {
            setMusicVolume();
            setSFXVolume();
            setMainVolume();
        }
    }

    public void setMusicVolume()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("Music", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void setSFXVolume()
    {
        float volume = sfxSlider.value;
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    public void setMainVolume()
    {
        float volume = mainSlider.value;
        audioMixer.SetFloat("Main", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("mainVolume", volume);
    }

    private void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        mainSlider.value = PlayerPrefs.GetFloat("mainVolume");

        setMusicVolume();
        setSFXVolume();
        setMainVolume();
    }
    
}
