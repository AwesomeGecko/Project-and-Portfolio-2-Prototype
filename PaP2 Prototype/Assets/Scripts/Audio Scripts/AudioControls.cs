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
    public int muteBoolAsInt;
    public bool hasVol;
    public int hasVolBoolAsInt;

    private void Start()
    {
        LoadVolume();
        mainSlider.onValueChanged.AddListener(setMainVolume);
        musicSlider.onValueChanged.AddListener(setMusicVolume);
        sfxSlider.onValueChanged.AddListener(setSFXVolume);
        LoadSliders();
        Debug.Log($"can i hear volume?, {hasVol}");
        Debug.Log($"am i muted?, {isMuted}");

        //loads with no volume? set the UI to mute
        if (hasVol == false)
        {
            //if no volume and is muted set the correct sprite
            if (isMuted == true)
            {
                image.sprite = Muted;
            }
        }
        else if(hasVol == true)
        {
            image.sprite = Unmuted;
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

        //Mute area
        isMuted = LoadBool("IsMuted");
        hasVol = LoadBool("HasVolume");
        SaveVolume();
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

    public void SaveBool(string key, bool value)
    { 
        int intValue = value ? 1 : 0;
        PlayerPrefs.SetInt(key, intValue);
        PlayerPrefs.Save();
    }

    public bool LoadBool(string key) 
    {
        int intValue = PlayerPrefs.GetInt(key);
        bool loadedBool = intValue != 0;
        return loadedBool;
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
            image.sprite = Muted;
            isMuted = true;
            hasVol = false;
            SaveBool("IsMuted", true);
            SaveBool("HasVolume", false);
        }
        else if (isMuted)
        {
            image.sprite = Unmuted;
            LoadVolume();
            isMuted = false;
            hasVol = true;
            SaveBool("IsMuted", false);
            SaveBool("HasVolume", true);
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
}
