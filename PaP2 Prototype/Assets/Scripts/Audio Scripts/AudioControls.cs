using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;

public class AudioControls : MonoBehaviour
{

    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Button muteButton;
    [SerializeField] Slider mainSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    [Header("Object Audio")]
    [SerializeField] List<ToggleTrap> plates;
    [SerializeField] List<autoDoors> doors;
    [SerializeField] teleporterScript teleporter;

    private bool isMuted;
    public float mainVol;
    public float mainVolume;
    public float musicVol;
    public float musicVolume;
    public float sfxVol;
    public float sfxVolume;

    private void Start()
    {
        DataPersistenceManager.instance.LoadAudio();
        LoadVolume();
        mainSlider.onValueChanged.AddListener(setMainVolume);
        musicSlider.onValueChanged.AddListener(setMusicVolume);
        sfxSlider.onValueChanged.AddListener(setSFXVolume);
        mainSlider.value = PlayerPrefs.GetFloat("mainVolume", mainVol);
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume", musicVol);
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume", sfxVol);
    }

    public void LoadVolume()
    {
        mainVol = PlayerPrefs.GetFloat("mainVolume", mainVolume);
        musicVol = PlayerPrefs.GetFloat("musicVolume", musicVolume);
        sfxVol = PlayerPrefs.GetFloat("sfxVolume", sfxVolume);
        audioMixer.SetFloat("Main", Mathf.Log10(mainVol) * 20);
        audioMixer.SetFloat("Music", Mathf.Log10(musicVol) * 20);
        audioMixer.SetFloat("SFX", Mathf.Log10(sfxVol) * 20);
    }

    #region SetVolume

    public void OnDisable()
    {
        PlayerPrefs.SetFloat("mainVolume", mainSlider.value);
        PlayerPrefs.SetFloat("musicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("sfxVolume", sfxSlider.value);
        PlayerPrefs.Save();
    }


    public void setMainVolume(float value)
    {
        audioMixer.SetFloat("Main", Mathf.Log10(value) * 20);
    }

    public void setMusicVolume(float value)
    {
        audioMixer.SetFloat("Music", Mathf.Log10(value) * 20);
    }

    public void setSFXVolume(float value)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(value) * 20);
        LandMine.SetListVolume(sfxVol);
        AdjustObjectSounds();
    }

    #endregion

    #region SFXVolume
    private void AdjustObjectSounds()
    {
        foreach (ToggleTrap plate in plates)
        {
            if (plate != null)
            {
                plate.SetVolume(sfxVol);
            }
        }

        foreach (autoDoors door in doors)
        {
            if (door != null)
            {
                door.SetVolume(sfxVol);
            }
        }

        if (teleporter != null)
        {
            teleporter.SetVolume(sfxVol);
        }
    }
    #endregion


    public void LoadData(AudioData data)
    {
        mainVolume = data.mainSlider;
        musicVolume = data.musicSlider;
        sfxVolume = data.sfxSlider;
        isMuted = data.isMuted;

    }
    public void SaveData(AudioData data)
    {
        LoadVolume();
        data.mainSlider = mainVolume;
        data.musicSlider = musicVolume;
        data.sfxSlider = sfxVolume;
        data.isMuted = isMuted;
    }
}
