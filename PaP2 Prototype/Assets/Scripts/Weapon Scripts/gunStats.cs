using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Jobs;
using UnityEngine.UIElements;
using static CombinedMeshInfo;

[CreateAssetMenu(fileName = "GunStats", menuName = "Guns/GunStats", order = 0)]
public class GunStats : ScriptableObject
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
    [Range(0, 1)] public float shootSoundVol;

    [Header("Particle Effects")]
    public ParticleSystem hitEffect;
    public ParticleSystem muzzleFlashPrefab;
    

    [Header("Field of View")]
    [Range(1, 120)] public float fieldOfView = 60f;

    

    [Header("Gun specific data")]
    public List<CombinedMeshInfo> combinedMeshes;
    public bool shouldUseScope;
    public bool isShotgun;
    public TransformData barrelTip;
    public int shotgunPelletCount;
    public int shotgunPelletSpread;

    public Vector3 defaultGunRotation = new Vector3(0f, 0f, 0f);

    public Vector3 defaultGunPositionOffset = new Vector3(0f, 0f, 0f);
    public Quaternion defaultRotation => Quaternion.Euler(defaultGunRotation);
}
