using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class autoDoors : MonoBehaviour
{

    bool triggerSet;
    [SerializeField] Animator anim;

    [Header("Audio")]
    [SerializeField]  AudioSource aud;
    [SerializeField]  AudioClip doorSound;
    private void Update()
    {
        if(gameManager.instance.playerScript.isDead)
        {
            aud.PlayOneShot(doorSound);
            triggerSet = false;
            //give stats to player
            anim.SetBool("character_nearby", false);

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggerSet)
        {
            aud.PlayOneShot(doorSound);
            triggerSet = true;
            //give stats to player
            anim.SetBool("character_nearby", true);
            
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && triggerSet)
        {
            aud.PlayOneShot(doorSound);
            triggerSet = false;
            //give stats to player
            anim.SetBool("character_nearby", false);

           
        }
    }
    
    public void SetVolume(float volume)
    {
        if (aud != null)
        {
            aud.volume = volume;
        }
    }
    
}
