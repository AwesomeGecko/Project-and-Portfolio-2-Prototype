using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    [Header("Gun Stats")]
    [SerializeField] public List<GunSettings> gunList = new List<GunSettings>();
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
    [SerializeField] GunSettings defaultPistol;
    private bool isAiming;
    private float defaultFOV;
    public int selectedGun;
    public Camera scopeIn;
    private int gameManagerAmmo;
    private ParticleSystem currentMuzzleFlash;


    void Reload()
    {
        InteractSound();
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
        GunSettings currentGun = gunList[selectedGun];

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

    private void CombineMeshes(List<CombinedMeshInfo> combinedMeshInfos)
    {
        // Combine the meshes into a single mesh
        Mesh combinedMesh = CombineSubMeshes(combinedMeshInfos);

        // Assign the combined mesh to the gun model's MeshFilter
        gunModel.GetComponent<MeshFilter>().sharedMesh = combinedMesh;

        // Assign a single material to MeshRenderer
        Material combinedMaterial = combinedMeshInfos[0].materials[0]; // Assuming there's one material per mesh
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = combinedMaterial;

    }

    private Mesh CombineSubMeshes(List<CombinedMeshInfo> combinedMeshInfos)
    {
        CombineInstance[] combine = new CombineInstance[combinedMeshInfos.Count];

        for (int i = 0; i < combinedMeshInfos.Count; i++)
        {
            combine[i].mesh = CombineMeshes(combinedMeshInfos[i].meshes);
            combine[i].transform = Matrix4x4.identity;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine, true, true);

        return combinedMesh;
    }

    private Mesh CombineMeshes(List<Mesh> meshes)
    {
        CombineInstance[] combine = new CombineInstance[meshes.Count];

        for (int i = 0; i < meshes.Count; i++)
        {
            combine[i].mesh = meshes[i];
            combine[i].transform = Matrix4x4.identity;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine, true, true);

        return combinedMesh;
    }


    public void getGunStats(GunSettings gun)
    {
        // Add the new gun to the gunList
        gunList.Add(gun);

        // Set the selectedGun index to the newly added gun
        selectedGun = gunList.Count - 1;

        // Assign gun stats to player variables
        shootDist = gun.shootDist;
        shootRate = gun.shootRate;
        PlayerBulletDamage = gun.PlayerBulletDamage;
        PlayerBulletDestroyTime = gun.PlayerBulletDestroyTime;
        PlayerBulletSpeed = gun.PlayerBulletSpeed;

        // Initialize ammo variables
        ammoCounter = gun.magSize;
        maxAmmo = gun.ammoMax;
        gun.totalAmmo = ammoCounter;

        // Check if the gun has a valid model
        if (gun.model != null)
        {
            //// Instantiate the gun prefab from the scriptable object
            //GameObject gunPrefab = Instantiate(gun.model, gunModel.transform.position, gunModel.transform.rotation, gunModel.transform);

            //// Adjust the gun's local position and rotation based on default values in the scriptable object
            //gunPrefab.transform.localPosition = gun.defaultPositionOffset;
            //gunPrefab.transform.localRotation = gun.defaultRotation;

            // Check if the gun has multiple meshes
            if (gun.combinedMeshes != null && gun.combinedMeshes.Count > 0)
            {
                CombineMeshes(gun.combinedMeshes);

                // Assign the combined mesh to the MeshFilter
                gunModel.GetComponent<MeshFilter>().sharedMesh = CombineSubMeshes(gun.combinedMeshes);
            }
            else
            {
                // Assign gun model mesh and material to the player's gunModel
                gunModel.GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
                gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.model.GetComponent<MeshRenderer>().sharedMaterial;
            }
        }
        // Set local rotation
        gunModel.transform.localRotation = gun.defaultRotation;

        // Set local position using the selected gun's offset
        gunModel.transform.localPosition = gun.defaultGunPositionOffset;

        // Update the player's UI
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

        // Check if the gun has multiple meshes
        if (gunList[selectedGun].combinedMeshes != null && gunList[selectedGun].combinedMeshes.Count > 0)
        {
            CombineMeshes(gunList[selectedGun].combinedMeshes);
        }
        else
        {
            // Assign gun model mesh and material to the player's gunModel
            gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
            gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;
        }
        //// Instantiate the gun prefab from the scriptable object
        //GameObject gunPrefab = Instantiate(gunList[selectedGun].model, gunModel.transform.position, gunModel.transform.rotation, gunModel.transform);

        //// Adjust the gun's local position and rotation based on default values in the scriptable object
        //gunPrefab.transform.localPosition = gunList[selectedGun].defaultPositionOffset;
        //gunPrefab.transform.localRotation = gunList[selectedGun].defaultRotation;


        // Set local rotation
        gunModel.transform.localRotation = gunList[selectedGun].defaultRotation;

        // Set local position using the selected gun's offset
        gunModel.transform.localPosition = gunList[selectedGun].defaultGunPositionOffset;

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

        GunSettings currentGun = gunList[selectedGun];


        GameObject PlayerBullet;

        Vector3 spawnPos = gunModel.transform.TransformPoint(currentGun.barrelTip.localPosition);
        Vector3 spawnScopedPos = isAiming? gunModel.transform.TransformPoint(currentGun.barrelTip.localPosition) : spawnPos;
        Quaternion spawnRotation = isAiming? gunModel.transform.rotation : gunModel.transform.rotation;


        //Have the ray cast ignore the player
        int playerLayer = LayerMask.NameToLayer("Player"); //Get the name of player 

        //Create the layer mask to ignore
        int layerMask = ~(1 << playerLayer);

        //Get the center of the screen in viewport cooridinates (normalized)
        Vector3 screenCenter = new Vector3(0.5f, 0.5f, 0f);

        //Raycast from the camera center into the scene
        Ray ray = Camera.main.ViewportPointToRay(screenCenter);
        RaycastHit hit;

        //Set the bullet direction
        Vector3 bulletDirection = ray.direction;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Vector3 targetPoint = hit.point;
            bulletDirection = (targetPoint - spawnPos).normalized;
        }

        // Check if the currently selected gun is the shotgun
        if (currentGun.isShotgun)
        { 
            for (int i = 0; i < currentGun.shotgunPelletCount; i++)
            {
                Vector3 spreadDirection = Quaternion.Euler(Random.Range(-currentGun.shotgunPelletSpread, currentGun.shotgunPelletSpread), Random.Range(-currentGun.shotgunPelletSpread, currentGun.shotgunPelletSpread),0f) * bulletDirection;
                Quaternion spreadRotation = Quaternion.LookRotation(spreadDirection);
                
                PlayerBullet = Instantiate(Playerbullet, isAiming ? spawnScopedPos : spawnPos, spreadRotation);

                PlayerBullet.GetComponent<Playerbullet>().SetBulletProperties(currentGun.PlayerBulletDamage, currentGun.PlayerBulletDestroyTime, currentGun.PlayerBulletSpeed, spreadDirection);
            }
        }
        else
        {
            PlayerBullet = Instantiate(Playerbullet, isAiming ? spawnScopedPos : spawnPos, isAiming ? gunModel.transform.rotation : gunModel.transform.rotation);

            PlayerBullet.GetComponent<Playerbullet>().SetBulletProperties(currentGun.PlayerBulletDamage, currentGun.PlayerBulletDestroyTime, currentGun.PlayerBulletSpeed, bulletDirection);
        }

        //Instantiate the muzzle flash particle effect system
        currentMuzzleFlash = Instantiate(currentGun.muzzleFlashPrefab, spawnPos, spawnRotation);

        yield return new WaitForSeconds(shootRate);

        isShooting = false;
        Destroy(currentMuzzleFlash.gameObject);
        UpdatePlayerUI();
    }

    void InteractSound()
    {
        GunSettings currentGun = gunList[selectedGun];

        if (aud && currentGun.reloadSound != null)
        {
            float adjustedVolume = currentGun.reloadSoundVol * gameManager.instance.aud.volume;
            aud.PlayOneShot(currentGun.reloadSound, adjustedVolume);
        }
    }
}
