using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keys : MonoBehaviour
{
    public static Keys instance;

    [Header("Audio")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip pickup;

    public void KeyPickUp()
    {
        aud.clip = pickup;
        aud.loop = false;
        aud.Play();
    }
}
