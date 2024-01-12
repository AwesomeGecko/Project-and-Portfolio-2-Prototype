using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Jobs;
using UnityEngine.UIElements;
using static CombinedMeshInfo;

[CreateAssetMenu]
public class gunStats : ScriptableObject
{
    public float shootRate;
    public int shootDist;
    public int PlayerBulletDamage;
    public int PlayerBulletDestroyTime;
    public int PlayerBulletSpeed;
    public int ammoCur;
    public int ammoMax;
    public int magSize;
    public int totalAmmo;

    public GameObject model;
    public ParticleSystem hitEffect;

    public ParticleSystem muzzleFlashPrefab;

    public AudioClip shootSound;
    [Range(0, 1)] public float shootSoundVol;

    public bool shouldUseScope;
    [Range(1, 120)] public float fieldOfView = 60f;

    public Vector3 defaultRotationEulerAngles = new Vector3(0f, 0f, 0f);
    public Vector3 defaultPositionOffset = new Vector3(0f, 0f, 0f);
    public Quaternion defaultRotation => Quaternion.Euler(defaultRotationEulerAngles);

    

    public TransformData barrelTip;

    public List<CombinedMeshInfo> combinedMeshes;
}
