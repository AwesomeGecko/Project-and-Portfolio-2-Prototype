using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMine : MonoBehaviour
{
    [Header("Time and Damage")]
    [SerializeField] public float boomTimer;
    [SerializeField] public int dmgAmount;

    [Header("Audio")]
    [SerializeField] private AudioSource aud;
    [SerializeField] private AudioClip mineBeep;

    private bool isPlayerNear;
    private float timer;

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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isPlayerNear = false;
            timer = 0f;
        }
    }

    private void Detonate()
    {
        PlayerController HP = FindObjectOfType<PlayerController>();
        if (HP != null)
        {
            HP.takeDamage(dmgAmount);
        }
        Destroy(gameObject);
    }
}
