using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class gunStats : ScriptableObject
{
    public int shootDamage;
    public float shootRate;
    public int shootDist;
    public int ammoCur;
    public int ammoMax;

    public GameObject model;
    public ParticleSystem hitEffect;
    public AudioClip shootSound;
    [Range(0, 1)] public float shootSoundVol;

    public bool shouldUseScope;
    [Range(1, 120)] public float fieldOfView = 60f;

}
