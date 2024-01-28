using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudioManager : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] randomClips;
    [SerializeField] AudioClip[] fightClips;
    [SerializeField] AudioClip[] deathClips;

    float minTime = 6f;
    float maxTime = 11f;
    float clipDelay = 3f;
    bool isRandomClipPlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        if (aud == null)
        {
            return;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!aud.isPlaying && !isRandomClipPlaying)
        {
            isRandomClipPlaying = true;
            ScheduleNextClip();
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
