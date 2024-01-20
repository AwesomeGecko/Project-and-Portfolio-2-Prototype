using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTrap : MonoBehaviour
{
    [Header("Enter Traps To Turn Off Here")]
    [SerializeField] List<GameObject> trapList = new List<GameObject>();

    [Header("Audio")]
    [SerializeField] AudioSource aud;
    [SerializeField] public AudioClip plateActivated;
    
    private void OnTriggerEnter(Collider other)
    {
        TurnOffTraps();
    }

    private void TurnOffTraps()
    {
        aud.PlayOneShot(plateActivated);
        foreach(GameObject trap in trapList)
        {
            SpikeTrap spike = trap.GetComponent<SpikeTrap>();
            if(spike != null)
            {
                spike.Deactivate();
            }
            LaserTrap laser = trap.GetComponent<LaserTrap>();
            if(laser != null)
            {
                laser.Deactivate();
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
