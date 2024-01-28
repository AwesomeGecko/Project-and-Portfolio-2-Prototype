using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]

public class AudioData
{
    [Header("Audio")]
    public float mainSlider;
    public float musicSlider;
    public float sfxSlider;
    public bool isMuted;


    //values defined here is default values
    //what the game starts with when there is no data
    public AudioData()
    {
        //Audio
        mainSlider = 1;
        musicSlider = 0.5f;
        sfxSlider = 0.5f;
        isMuted = false;
    }
}
