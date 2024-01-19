using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LandMine : MonoBehaviour
{
    [Header("Time and Damage")]
    [SerializeField] public float boomTimer;
    [SerializeField] public int dmgAmount;

    [Header("Audio")]
    [SerializeField] public AudioSource aud;
    [SerializeField] public AudioClip mineBeep;
    [SerializeField] public AudioClip expSound;

    private bool isPlayerNear;
    private float timer;
    private static List<LandMine> mineTraps = new List<LandMine>();
    
    // Update is called once per frame
    void Update()
    {
        if(isPlayerNear)
        {
            timer += Time.deltaTime;
            if (timer >= boomTimer)
            {
                // Mine explodes here
                Detonate();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            aud.PlayOneShot(mineBeep);
            aud.PlayOneShot(expSound);
        }
        else if(other.CompareTag("PlayerBullet"))
        {
            isPlayerNear = false;
            if(aud != null)
            {
                aud.PlayOneShot(expSound);
            }
            StartCoroutine(DeactivateAfterAudio());
        }
    }

    private void OnEnable()
    {
        mineTraps.Add(this);
    }

    private void OnDisable()
    {
        mineTraps.Remove(this);
    }

    private void Detonate()
    {
        PlayerController HP = FindObjectOfType<PlayerController>();
        if(HP != null)
        {
            HP.takeDamage(dmgAmount);
        }
        gameObject.SetActive(false);
    }

    private IEnumerator DeactivateAfterAudio()
    {
        yield return new WaitForSeconds(1.0f);
        gameObject.SetActive(false);
    }
    
    public static void SetListVolume(float volume)
    {
        foreach(LandMine mines in mineTraps)
        {
            if(mines != null)
            {
                mines.SetVolume(volume);
            }
        }
    }

    public void SetVolume(float volume)
    {
        if(aud != null)
        {
            aud.volume = volume;
        }
    }
}
