using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    [Header("Gun Stats")]
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject Playerbullet;
    [SerializeField] int bulletDestroyTime;
    [SerializeField] float shootRate;
    [SerializeField] int PlayerBulletDamage;
    [SerializeField] int PlayerBulletDestroyTime;
    [SerializeField] int PlayerBulletSpeed;
    [SerializeField] public int ammoCounter;
    [SerializeField] public int maxAmmo;
    [SerializeField] int shootDist;
    [SerializeField] GameObject gunModel;
    [SerializeField] gunStats defaultPistol;
    private bool isAiming;
    private float defaultFOV;
    int selectedGun;
    public Camera scopeIn;
    private int gameManagerAmmo;
    private ParticleSystem currentMuzzleFlash;


    void Reload()
    {
        // Check if the gun is not already full
        if (gunList[selectedGun].ammoCur < gunList[selectedGun].magSize)
        {
            // Calculate the number of bullets needed to fill the magazine
            int bulletsNeeded = gunList[selectedGun].magSize - gunList[selectedGun].ammoCur;

            // Check if the player has enough bullets to reload
            if (gunList[selectedGun].totalAmmo >= bulletsNeeded)
            {
                // Subtract the bullets needed from player's total ammo
                gunList[selectedGun].totalAmmo -= bulletsNeeded;

                // Fill the gun's magazine with the remaining bullets in the total ammo
                gunList[selectedGun].ammoCur = gunList[selectedGun].magSize;

                // Update the UI
                UpdatePlayerUI();
            }
            else
            {
                // Check if there is any ammo left to reload
                if (gunList[selectedGun].totalAmmo > 0)
                {
                    // Reload with the remaining ammo
                    gunList[selectedGun].ammoCur += gunList[selectedGun].totalAmmo;

                    // Reset total ammo to 0
                    gunList[selectedGun].totalAmmo = 0;

                    // Update the UI
                    UpdatePlayerUI();
                    
                }
            }
        }
    }
    void ToggleAimDownSights()
    {
        isAiming = !isAiming;
        gunStats currentGun = gunList[selectedGun];

        //Adjust the camera properties
        if (isAiming)
        {

            //Deactivate the main crosshairs
            gameManager.instance.Crosshair.gameObject.SetActive(false);

            //If the current gun wants the scope
            if (currentGun.shouldUseScope)
            {

                //Enable the scope image overlay ontop of the main camera
                gameManager.instance.Scope.gameObject.SetActive(true);

                //Cull the gun out of screen
                scopeIn.cullingMask = scopeIn.cullingMask & ~(1 << 7);

                //Adjust the scope cameras FOV
                Camera.main.fieldOfView = currentGun.fieldOfView;
            }
            else
            {
                //Deactivate the Scope image
                gameManager.instance.Scope.gameObject.SetActive(false);



                //Cull the gun back onto screen
                scopeIn.cullingMask = scopeIn.cullingMask | (1 << 7);
                //Adjust the main cameras FOV
                Camera.main.fieldOfView = currentGun.fieldOfView;
            }
        }
        else
        {
            //Enable the main crosshairs
            gameManager.instance.Crosshair.gameObject.SetActive(true);

            //Cull the gun onto screen
            scopeIn.cullingMask = scopeIn.cullingMask | (1 << 7);

            //Disable the scope camera
            gameManager.instance.Scope.gameObject.SetActive(false);

            //Re-enable the main camera and set it to the default value
            Camera.main.fieldOfView = defaultFOV;
        }
    }

    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);
        selectedGun = gunList.Count - 1;

        shootDist = gun.shootDist;
        shootRate = gun.shootRate;
        PlayerBulletDamage = gun.PlayerBulletDamage;
        PlayerBulletDestroyTime = gun.PlayerBulletDestroyTime;
        PlayerBulletSpeed = gun.PlayerBulletSpeed;
        ammoCounter = gun.magSize;
        maxAmmo = gun.ammoMax;
        gun.totalAmmo = ammoCounter;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.model.GetComponent<MeshRenderer>().sharedMaterial;

        UpdatePlayerUI();
    }

    void selectGun()
    {
        if (!isAiming)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1)
            {
                selectedGun++;
                changeGun();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
            {
                selectedGun--;
                changeGun();
            }
        }

    }

    void changeGun()
    {
        shootDist = gunList[selectedGun].shootDist;
        shootRate = gunList[selectedGun].shootRate;
        PlayerBulletDamage = gunList[selectedGun].PlayerBulletDamage;
        PlayerBulletDestroyTime = gunList[selectedGun].PlayerBulletDestroyTime;
        PlayerBulletSpeed = gunList[selectedGun].PlayerBulletSpeed;

        ammoCounter = gunList[selectedGun].totalAmmo;

        maxAmmo = gunList[selectedGun].ammoMax;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;
        isShooting = false;
    }
    IEnumerator Shoot()
    {

        isShooting = true;

        if (gunList[selectedGun].ammoCur <= 0)
        {
            isShooting = false;
            yield break;
        }

        gunList[selectedGun].ammoCur--;

        aud.PlayOneShot(gunList[selectedGun].shootSound, gunList[selectedGun].shootSoundVol);

        gunStats currentGun = gunList[selectedGun];


        GameObject PlayerBullet;

        Vector3 spawnPos = gunModel.transform.TransformPoint(currentGun.barrelTip.localPosition);
        Vector3 spawnScopedPos = isAiming? gunModel.transform.TransformPoint(currentGun.barrelTip.localPosition) : spawnPos;
        Quaternion spawnRotation = isAiming? gunModel.transform.rotation : gunModel.transform.rotation;

        //Get the center of the screen in viewport cooridinates (normalized)
        Vector3 screenCenter = new Vector3(0.5f, 0.5f, 0f);

        //Raycast from the camera center into the scene
        Ray ray = Camera.main.ViewportPointToRay(screenCenter);
        RaycastHit hit;

        //Set the bullet direction
        Vector3 bulletDirection = ray.direction;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 targetPoint = hit.point;
            bulletDirection = (targetPoint - spawnPos).normalized;
        }

        //Instantiate the player bullet with its adjusted locations based off of the gun model transform location
        PlayerBullet = Instantiate(Playerbullet, isAiming ? spawnScopedPos : spawnPos, spawnRotation);

        //Instantiate the muzzle flash particle effect system
        currentMuzzleFlash = Instantiate(currentGun.muzzleFlashPrefab, spawnPos, spawnRotation);

        PlayerBullet.GetComponent<Playerbullet>().SetBulletProperties(currentGun.PlayerBulletDamage, currentGun.PlayerBulletDestroyTime, currentGun.PlayerBulletSpeed, bulletDirection);

        yield return new WaitForSeconds(shootRate);



        isShooting = false;
        Destroy(currentMuzzleFlash.gameObject);
        UpdatePlayerUI();
    }
}
