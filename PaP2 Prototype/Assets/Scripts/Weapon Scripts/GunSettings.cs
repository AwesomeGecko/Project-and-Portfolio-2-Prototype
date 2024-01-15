using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Jobs;
using UnityEngine.UIElements;
using static CombinedMeshInfo;

[CreateAssetMenu(fileName = "GunSettings", menuName = "Guns/GunSettings", order = 0)]
public class GunSettings : ScriptableObject
{
    [Header("Basic Gun Information")]
    public GameObject model;
    public float shootRate;
    public int shootDist;
    public int PlayerBulletDamage;
    public int PlayerBulletDestroyTime;
    public int PlayerBulletSpeed;
    public int ammoCur;
    public int ammoMax;
    public int magSize;
    public int totalAmmo;

    [Header("Sounds")]
    public AudioClip shootSound;
    [Range(0, 10)] public float shootSoundVol;
    [Space]
    public AudioClip reloadSound;
    [Range(0, 10)] public float reloadSoundVol;

    [Header("Particle Effects")]
    public ParticleSystem hitEffect;
    public ParticleSystem muzzleFlashPrefab;
    

    [Header("Field of View")]
    [Range(1, 120)] public float fieldOfView = 60f;

    

    [Header("Gun specific data")]
    public List<CombinedMeshInfo> combinedMeshes;
    [Space]
    public bool shouldUseScope;
    [Space]
    public bool isShotgun;
    [Space]
    public int shotgunPelletCount;
    public int shotgunPelletSpread;
    [Space]
    public TransformData barrelTip;

    public Vector3 defaultGunRotation = new Vector3(0f, 0f, 0f);

    public Vector3 defaultGunPositionOffset = new Vector3(0f, 0f, 0f);
    public Quaternion defaultRotation => Quaternion.Euler(defaultGunRotation);
}
