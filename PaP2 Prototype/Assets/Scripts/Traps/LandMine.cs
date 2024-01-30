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

    [SerializeField] private ParticleSystem explode;
    [SerializeField] BoxCollider col;

    private bool isPlayerNear;
    private float timer;
    private static List<LandMine> mineTraps = new List<LandMine>();
    private ParticleSystem expInstance;
    
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
        if (other.CompareTag("Player") || other.CompareTag("Enemies"))
        {
            isPlayerNear = true;
            aud.PlayOneShot(mineBeep);
            aud.PlayOneShot(expSound);
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("PlayerBullet"))
        {
            isPlayerNear = false;
            if (aud != null)
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
        if(explode != null)
        {
            expInstance = Instantiate(explode, transform.position, Quaternion.identity);
        }
        gameObject.SetActive(false);
    }

    private IEnumerator DeactivateAfterAudio()
    {
        yield return new WaitForSeconds(0.5f);
        if (explode != null)
        {
            expInstance = Instantiate(explode, transform.position, Quaternion.identity);
        }
        gameObject.SetActive(false);
    }
}
