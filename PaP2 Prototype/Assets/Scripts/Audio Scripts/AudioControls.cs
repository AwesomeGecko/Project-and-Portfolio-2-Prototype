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
    public bool hasVol;

    private void Start()
    {
        LoadSliders();
        LoadVolume();
        mainSlider.onValueChanged.AddListener(setMainVolume);
        musicSlider.onValueChanged.AddListener(setMusicVolume);
        sfxSlider.onValueChanged.AddListener(setSFXVolume);
        //Debug.Log($"am i muted? (Loaded) {isMuted}");
        if (isMuted)
        {
            AudioMuted();
            image.sprite = Muted;
            isMuted = true;
            SaveBool();
            SaveVolume();
            //Debug.Log("Muted");
        }
        else
        {
            image.sprite = Unmuted;
            LoadVolume();
            isMuted = false;
            SaveBool();
            //Debug.Log("Unmuted");
        }
    }
    public void OnDisable()
    {
        SaveVolume();
        //Debug.Log($"am i muted? (Saved) {isMuted}");
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
        isMuted = (PlayerPrefs.GetInt("IsMuted") != 0);
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
        SaveBool();
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
        AdjustObjectSounds();
    }

    public void SaveBool()
    { 
        PlayerPrefs.SetInt("IsMuted", (isMuted ? 1 : 0));
        isMuted = (PlayerPrefs.GetInt("IsMuted") != 0);
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

    public void MutedSound()
    {
        AudioMuted();
        image.sprite = Muted;
        isMuted = true;
        SaveBool();
        SaveVolume();
    }
    public void UnmutedSound()
    {
        image.sprite = Unmuted;
        LoadVolume();
        isMuted = false;
        hasVol = true;
        SaveBool();
    }

    public void muteSounds()
    {
        //Debug.Log("Clicked");
        if (!isMuted)
        {
            MutedSound();
        }
        else if (isMuted)
        {
            UnmutedSound();
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
