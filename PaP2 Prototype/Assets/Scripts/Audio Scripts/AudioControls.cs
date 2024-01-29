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
    [SerializeField] List<LaserTrap> lasers;
    [SerializeField] List<SpikeTrap> spikes;

    [Header("-----Mute Images-----")]
    [SerializeField] public Sprite Muted;
    [SerializeField] public Sprite Unmuted;
    [SerializeField] public Image image;

    private bool isMuted;
    public float mainVol;
    public float mainVolume;
    public float musicVol;
    public float musicVolume;
    public float sfxVol;
    public float sfxVolume;
    public int boolAsInt;
    public int noVol;

    private void Start()
    {
        DataPersistenceManager.instance.LoadAudio();
        LoadVolume();
        mainSlider.onValueChanged.AddListener(setMainVolume);
        musicSlider.onValueChanged.AddListener(setMusicVolume);
        sfxSlider.onValueChanged.AddListener(setSFXVolume);
        LoadSliders();
        isMuted = boolAsInt == 1;
        if (isMuted == false)
        {
            return;
        }
        else 
        {
            muteSounds();
        }
    }
    public void OnDisable()
    {
        SaveVolume();
    }


    #region SetVolume
    public void LoadVolume()
    {
        mainVol = PlayerPrefs.GetFloat("mainVolume", mainVolume);
        musicVol = PlayerPrefs.GetFloat("musicVolume", musicVolume);
        sfxVol = PlayerPrefs.GetFloat("sfxVolume", sfxVolume);
        audioMixer.SetFloat("Main", Mathf.Log10(mainVol) * 20);
        audioMixer.SetFloat("Music", Mathf.Log10(musicVol) * 20);
        audioMixer.SetFloat("SFX", Mathf.Log10(sfxVol) * 20);
        boolAsInt = PlayerPrefs.GetInt("Muted", 0);
        isMuted = boolAsInt == 1;
    }

    public void LoadSliders()
    {
        mainSlider.value = PlayerPrefs.GetFloat("mainVolume", mainVol);
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume", musicVol);
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume", sfxVol);
    }


    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("mainVolume", mainSlider.value);
        PlayerPrefs.SetFloat("musicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("sfxVolume", sfxSlider.value);
        saveBool();
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

    public void saveBool()
    { 
        boolAsInt = isMuted ? 1 : 0;
        PlayerPrefs.SetInt("Muted", boolAsInt);
        PlayerPrefs.Save();
    }

    public void AudioMuted()
    {
        audioMixer.SetFloat("Main", Mathf.Log10(0.0001f) * 20);
        audioMixer.SetFloat("Music", Mathf.Log10(0.0001f) * 20);
        audioMixer.SetFloat("SFX", Mathf.Log10(0.0001f) * 20);
    }

    public void muteSounds()
    {
        if (!isMuted)
        {
            SaveVolume();
            AudioMuted();
            saveBool();
            image.sprite = Muted;
            isMuted = true;
        }
        else
        {
            image.sprite = Unmuted;
            LoadVolume();
            isMuted = false;
        }
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

        foreach (LaserTrap laser in lasers)
        {
            if (laser != null)
            {
                laser.SetVolume(sfxVol);
            }
        }

        foreach (SpikeTrap spike in spikes)
        {
            if (spike != null)
            {
                spike.SetVolume(sfxVol);
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
