using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    [Header("Gun Stats")]
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject Playerbullet;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform scopedShootPos;
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


    void Reload()
    {
        // Check if the gun is not already full
        if (gunList[selectedGun].ammoCur < gunList[selectedGun].magSize)
        {
            // Calculate the number of bullets needed to fill the magazine
            int bulletsNeeded = gunList[selectedGun].magSize - gunList[selectedGun].ammoCur;

            // Check if the player has enough bullets to reload
            if (ammoCounter >= bulletsNeeded)
            {
                // Subtract the bullets needed from player's total ammo
                ammoCounter -= bulletsNeeded;

                // Fill the gun's magazine with the remaining bullets in the total ammo
                gunList[selectedGun].ammoCur = gunList[selectedGun].magSize;

                // Update the UI
                UpdatePlayerUI();
            }
            else
            {
                // Check if there is any ammo left to reload
                if (ammoCounter > 0)
                {
                    // Reload with the remaining ammo
                    gunList[selectedGun].ammoCur += ammoCounter;

                    // Reset total ammo to 0
                    ammoCounter = 0;

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
    void changeGun()
    {
        shootDist = gunList[selectedGun].shootDist;
        shootRate = gunList[selectedGun].shootRate;
        PlayerBulletDamage = gunList[selectedGun].PlayerBulletDamage;
        PlayerBulletDestroyTime = gunList[selectedGun].PlayerBulletDestroyTime;
        PlayerBulletSpeed = gunList[selectedGun].PlayerBulletSpeed;
        ammoCounter = gunList[selectedGun].ammoCur;
        maxAmmo = gunList[selectedGun].ammoMax;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;
        isShooting = false;
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
    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);
        selectedGun = gunList.Count - 1;
        gun.ammoCur = gun.magSize;
        shootDist = gun.shootDist;
        shootRate = gun.shootRate;
        PlayerBulletDamage = gun.PlayerBulletDamage;
        PlayerBulletDestroyTime = gun.PlayerBulletDestroyTime;
        PlayerBulletSpeed = gun.PlayerBulletSpeed;
        ammoCounter = gun.magSize;
        maxAmmo = gun.ammoMax;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.model.GetComponent<MeshRenderer>().sharedMaterial;

        UpdatePlayerUI();
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

        if (isAiming)
        {
            PlayerBullet = Instantiate(Playerbullet, scopedShootPos.position, transform.rotation);

        }
        else
        {
            PlayerBullet = Instantiate(Playerbullet, shootPos.position, transform.rotation);

        }
        PlayerBullet.GetComponent<Playerbullet>().SetBulletProperties(PlayerBulletDamage, PlayerBulletDestroyTime, PlayerBulletSpeed);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
        UpdatePlayerUI();
    }
}
