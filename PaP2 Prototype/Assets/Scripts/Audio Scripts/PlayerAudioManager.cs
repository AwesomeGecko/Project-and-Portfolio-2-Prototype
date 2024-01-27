using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAudioManager : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip sceneStart;
    [SerializeField] AudioClip instructionClip;
    [SerializeField] AudioClip[] randomClips; 

    float minTime = 6f;
    float maxTime = 11f;
    float clipDelay = 3f;
    bool isRandomClipPlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        if(aud == null)
        {
            //Debug.Log("AudioSource component not assigned in the PlayerAudioManager script.");
            return;
        }
        if(sceneStart != null)
        {
            
            SceneStartAudio();
            
        }
        else
        {
            //Debug.Log("Start Scene Audio Clip is not assigned in the PlayerAudioManager Script");
        }
    }

    
    // Update is called once per frame
    void Update()
    {
        if(!aud.isPlaying && !isRandomClipPlaying)
        {
            isRandomClipPlaying = true;
            ScheduleNextClip();
        }
        
    }
    

    void SceneStartAudio()
    {
        aud.PlayOneShot(sceneStart);
        Invoke("InstructionAudio", sceneStart.length);
    }
    
    void InstructionAudio()
    {
        if(instructionClip != null)
        {
            aud.clip = instructionClip;
            aud.Play();
        }
        else
        {
            //Debug.Log("No instruction voice line audio clips assigned in the PlayerAudioManager script.");
        }
    }

    void PlayRandomAudio()
    {
        AudioClip randomClip = randomClips[Random.Range(0, randomClips.Length)];
        aud.PlayOneShot(randomClip);
        Invoke("RandomClipFinished", randomClip.length);
    }

    void ScheduleNextClip()
    {
        float delay = Random.Range(minTime, maxTime);
        Invoke("PlayRandomAudio", delay);
    }
    
    void RandomClipFinished()
    {
        isRandomClipPlaying = false;
    }
}
