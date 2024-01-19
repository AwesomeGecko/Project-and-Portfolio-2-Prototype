using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioControls : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Button muteButtons;

    [Header("-----Sliders-----")]
    [SerializeField] Slider mainSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    
    [SerializeField] ToggleTrap plate;

    private float volume;

    public void Start()
    {
        startMusic();
    }


    public void setMainVolume()
    {
        volume = mainSlider.value;
        audioMixer.SetFloat("Main", Mathf.Log10(volume) * 20);
        gameManager.instance.aud.volume = volume;
        LandMine.SetListVolume(volume);
        AdjustPlateSound();
        PlayerPrefs.SetFloat("mainVolume", volume);
    }

    public void setMusicVolume()
    {
        volume = musicSlider.value;
        audioMixer.SetFloat("Music", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("musicVolume", volume);
        
    }

    public void setSFXVolume()
    {
        volume = sfxSlider.value;
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        gameManager.instance.aud.volume = volume;
        LandMine.SetListVolume(volume);
        AdjustPlateSound();
        PlayerPrefs.SetFloat("sfxVolume", volume);

    }


    public void LoadVolume()
    {
        mainSlider.value = 1;
        musicSlider.value = 0.5f;
        sfxSlider.value = 0.5f;

        SetVolume();
    }

    public void SetVolume()
    {
        setMusicVolume();
        setSFXVolume();
        setMainVolume();
    }

    public void startMusic()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetVolume();
        }
    }

    public void checkMuted()
    {
        if (gameManager.instance.isMuted)
        {
            mainSlider.value = 0;
            musicSlider.value = 0;
            sfxSlider.value = 0;

            SetVolume();
        }
        else
        {
            LoadVolume();
        }
    }

    public void AdjustPlateSound()
    {
        if(plate != null)
        {
            plate.SetVolume(volume);
        }
    }
}
