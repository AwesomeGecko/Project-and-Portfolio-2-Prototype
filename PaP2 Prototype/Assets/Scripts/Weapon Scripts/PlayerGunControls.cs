using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGunControls : MonoBehaviour
{

    [Header("Gun Stats")]
    [SerializeField] public List<GunSettings> gunList = new List<GunSettings>();
    private GameObject droppedGunPrefab;
    [SerializeField] GameObject Playerbullet;
    [SerializeField] Transform BackPack;
    [SerializeField] float shootRate;
    [SerializeField] int PlayerBulletDamage;

    [SerializeField] int PlayerBulletSpeed;
    [SerializeField] public int ammoCounter;
    [SerializeField] public int maxAmmo;
    [SerializeField] int shootDist;
    [SerializeField] Transform gunLocation;
    [SerializeField] public GunSettings defaultPistol;
    private bool isAiming;
    public float defaultFOV;
    public int selectedGun;
    private bool isShooting;
    public Camera scopeIn;
    public int gameManagerAmmo;
    private ParticleSystem currentMuzzleFlash;
    private PlayerController playerController;
    [SerializeField] AudioSource aud;

    void Start()
    {
        //Default field of view for the player
        defaultFOV = Camera.main.fieldOfView;

        getGunStats(defaultPistol);

        int.TryParse(gameManager.instance.ammoCounter.text, out gameManagerAmmo);
        ammoCounter = gameManagerAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.instance.isPaused)
        {
            if (gunList.Count > 0)
            {
                if (Input.GetButton("Fire1") && !isShooting)
                {
                    StartCoroutine(Shoot());
                }

                selectGun();

                if (Input.GetButtonDown("AimDownSight"))
                {
                    ToggleAimDownSights();
                }

                if (Input.GetButtonDown("Reload"))
                {
                    Reload();
                }

            }
        }
    }




    public void Reload()
    {

        // Check if the gun is not already full
        if (gunList[selectedGun].AmmoInMag < gunList[selectedGun].MagSize)
        {
            // Calculate the number of bullets needed to fill the magazine
            int bulletsNeeded = gunList[selectedGun].MagSize - gunList[selectedGun].AmmoInMag;

            // Check if the player has enough bullets to reload
            if (gunList[selectedGun].PlayerTotalAmmo >= bulletsNeeded)
            {
                // Subtract the bullets needed from player's total ammo
                gunList[selectedGun].PlayerTotalAmmo -= bulletsNeeded;

                // Fill the gun's magazine with the remaining bullets in the total ammo
                gunList[selectedGun].AmmoInMag = gunList[selectedGun].MagSize;

                // Update the UI
                UpdatePlayerUI();
            }
            else
            {
                // Check if there is any ammo left to reload
                if (gunList[selectedGun].PlayerTotalAmmo > 0)
                {
                    // Reload with the remaining ammo
                    gunList[selectedGun].AmmoInMag += gunList[selectedGun].PlayerTotalAmmo;

                    // Reset total ammo to 0
                    gunList[selectedGun].PlayerTotalAmmo = 0;

                    // Update the UI
                   UpdatePlayerUI();

                }
            }
            aud.PlayOneShot(gunList[selectedGun].reloadSound, gunList[selectedGun].reloadSoundVol);
        }
    }
    public void ToggleAimDownSights()
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
                //Using Invoke enables the scope image overlay ontop of the main camera through a time delay
                Invoke("ActivateM4Sight", 0.3f);
                //Adjust the scope cameras FOV
                Camera.main.fieldOfView = currentGun.fieldOfView;
            }

            //If the current gun is the shotgun use the shotgun sight
            else if (currentGun.isShotgun)
            {
                //Using Invoke enables the shotgun sight ontop of the main camera through a time delay
                Invoke("ActivateShotgunSight", 0.4f);
                //Adjust the shotgun camera FOV
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

            //Disable the shotgun sight
            gameManager.instance.ShotgunSight.gameObject.SetActive(false);

            //Re-enable the main camera and set it to the default value
            Camera.main.fieldOfView = defaultFOV;
        }
    }

    //This method simply calls the UI image of the scope and sets it to true
    void ActivateM4Sight()
    {
        gameManager.instance.Scope.gameObject.SetActive(true);
        //Cull the gun out of screen by setting the gun model on a layer called weapon. Then the m4 will not be shown when the scope image is overlayed on the main camera.
        scopeIn.cullingMask = scopeIn.cullingMask & ~(1 << 7);
    }

    //This method simply calls the UI image of the shotgun sight and sets it to true
    void ActivateShotgunSight()
    {
        gameManager.instance.ShotgunSight.gameObject.SetActive(true);
    }

    //private void CombineMeshes(List<CombinedMeshInfo> combinedMeshInfos)
    //{
    //    // Combine the meshes into a single mesh
    //    Mesh combinedMesh = CombineSubMeshes(combinedMeshInfos);

    //    // Assign the combined mesh to the gun model's MeshFilter
    //    gunModel.GetComponent<MeshFilter>().sharedMesh = combinedMesh;

    //    // Assign a single material to MeshRenderer
    //    Material combinedMaterial = combinedMeshInfos[0].materials[0]; // Assuming there's one material per mesh
    //    gunModel.GetComponent<MeshRenderer>().sharedMaterial = combinedMaterial;

    //}

    //private Mesh CombineSubMeshes(List<CombinedMeshInfo> combinedMeshInfos)
    //{
    //    CombineInstance[] combine = new CombineInstance[combinedMeshInfos.Count];

    //    for (int i = 0; i < combinedMeshInfos.Count; i++)
    //    {
    //        combine[i].mesh = CombineMeshes(combinedMeshInfos[i].meshes);
    //        combine[i].transform = Matrix4x4.identity;
    //    }

    //    Mesh combinedMesh = new Mesh();
    //    combinedMesh.CombineMeshes(combine, true, true);

    //    return combinedMesh;
    //}

    //private Mesh CombineMeshes(List<Mesh> meshes)
    //{
    //    CombineInstance[] combine = new CombineInstance[meshes.Count];

    //    for (int i = 0; i < meshes.Count; i++)
    //    {
    //        combine[i].mesh = meshes[i];
    //        combine[i].transform = Matrix4x4.identity;
    //    }

    //    Mesh combinedMesh = new Mesh();
    //    combinedMesh.CombineMeshes(combine, true, true);

    //    return combinedMesh;
    //}

    public void getGunStats(GunSettings gun)
    {  
        if (gunList.Count < 2)
        {
            // Check if there is an existing gunPrefab
            if (gunLocation.childCount > 0)
            {
                // Move the existing gun to the backpack
                Transform existingGun = gunLocation.GetChild(0);
                existingGun.SetParent(BackPack);
                existingGun.gameObject.SetActive(false);
            }
            // Add the new gun to the gunList
            gunList.Add(gun);

            // Set the selectedGun index to the newly added gun
            selectedGun = gunList.Count - 1;

            // Assign gun stats to player variables
            shootDist = gun.shootDist;
            shootRate = gun.shootRate;
            PlayerBulletDamage = gun.PlayerBulletDamage;

            PlayerBulletSpeed = gun.PlayerBulletSpeed;

            //// Initialize ammo variables
            ammoCounter = gun.MagSize;

            if(gun.isdefaultPistol)
            {
                
                gunList[selectedGun].PlayerTotalAmmo = gunList[selectedGun].MagSize;
            }
            else
            {
                gunList[selectedGun].PlayerTotalAmmo = 0;
            }

            // Check if the gun has a valid model
            if (gun.model != null)
            {
                // Instantiate the gun prefab from the scriptable object
                GameObject gunPrefab = Instantiate(gun.model, gunLocation.position, gunLocation.rotation, gunLocation);

                // Adjust the gun's local position and rotation based on default values in the scriptable object
                gunPrefab.transform.localPosition = gun.defaultGunPositionOffset;
                gunPrefab.transform.localRotation = gun.defaultRotation;

                //// Check if the gun has multiple meshes
                //if (gun.combinedMeshes != null && gun.combinedMeshes.Count > 0)
                //{
                //    CombineMeshes(gun.combinedMeshes);

                //    // Assign the combined mesh to the MeshFilter
                //    gunModel.GetComponent<MeshFilter>().sharedMesh = CombineSubMeshes(gun.combinedMeshes);
                //}
                //else
                //{
                //    // Assign gun model mesh and material to the player's gunModel
                //    gunModel.GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
                //    gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.model.GetComponent<MeshRenderer>().sharedMaterial;
                //}
            }
            // Set local rotation
            gunLocation.localRotation = gun.defaultRotation;

            // Set local position using the selected gun's offset
            gunLocation.localPosition = gun.defaultGunPositionOffset;

            // Update the player's UI
            UpdatePlayerUI();
        }
        else
        {
            Debug.Log("Inventory is full");
        }
       
    }

    public void selectGun()
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
        
        // Deactivate the current gun
        if (gunLocation.childCount > 0)
        {
            Transform currentGun = gunLocation.GetChild(0);
            currentGun.gameObject.SetActive(false);
            // Move the current gun to the backpack
            currentGun.SetParent(BackPack);
        }

        // Use selectedGunIndex to access the gun in gunList
        shootDist = gunList[selectedGun].shootDist;
        shootRate = gunList[selectedGun].shootRate;
        PlayerBulletDamage = gunList[selectedGun].PlayerBulletDamage;

        //PlayerBulletSpeed = gunList[selectedGun].PlayerBulletSpeed;

        //ammoCounter = gunList[selectedGun].PlayerTotalAmmo;

        //maxAmmo = gunList[selectedGun].MaxGunAmmo;

        // Move the gun from the backpack to the player's hands
        if (BackPack.childCount > 0)
        {
            Transform nextGun = BackPack.GetChild(0);
            nextGun.SetParent(gunLocation);
            nextGun.gameObject.SetActive(true);
            // Adjust the gun's local position and rotation based on default values in the scriptable object
            nextGun.localPosition = gunList[selectedGun].defaultGunPositionOffset;
            nextGun.localRotation = gunList[selectedGun].defaultRotation;

            // Set local rotation and position of gunLocation
            gunLocation.localRotation = gunList[selectedGun].defaultRotation;
            gunLocation.localPosition = gunList[selectedGun].defaultGunPositionOffset;
        }
        UpdatePlayerUI();

        isShooting = false;
    }
    public void SwapGuns()
    {
        Debug.Log("Player gun controls script swap guns called");

        if (gunList.Count > 1 && selectedGun >= 0 && selectedGun < gunList.Count)
        {
            Debug.Log("prompt player to swap");
            // Drop the current gun
            DropGun(gunList[selectedGun]);
        }
    }

    private void DropGun(GunSettings gun)
    {
           
            Transform hands = gunLocation.GetChild(0);
        

        UpdatePlayerUI();

        // Instantiate a dropped version of the gun prefab slightly above the ground
        Vector3 dropPosition = gunLocation.position; // You can adjust Vector3.up as needed
        GameObject droppedGun = Instantiate(gunList[selectedGun].GunPickupPrefab, dropPosition, Quaternion.identity);
        gunList.RemoveAt(selectedGun);
        Destroy(hands.gameObject);
        // Set the droppedGun to active
        droppedGun.SetActive(true);

        // Optional: Apply force to simulate the gun falling
        Rigidbody gunRigidbody = droppedGun.GetComponent<Rigidbody>();
        if (gunRigidbody != null)
        {
            // Get the player's rotation
            Quaternion playerRotation = gunLocation.rotation;
           
            // Use Quaternion.Euler to convert euler angles to a quaternion
            Quaternion forwardRotation = Quaternion.Euler(0f, playerRotation.eulerAngles.y, 0f);

            //Set the bounce direction <><> Dont forget to set the rigid body to interpolate otherwise no bounce in Unity
            Vector3 bounceDirection = forwardRotation * Vector3.forward;
           
            // Adjust the force as needed
            float bounceForceMagnitude = 100f; // Adjust the magnitude as needed
            Vector3 bounceForce = (bounceForceMagnitude * bounceDirection) * Time.deltaTime;

            // Apply the force to the Rigidbody
            gunRigidbody.AddForce(bounceForce, ForceMode.Impulse);
        }

    }

    IEnumerator Shoot()
    {
        isShooting = true;

        if (gunList[selectedGun].AmmoInMag <= 0)
        {
            isShooting = false;
            yield break;
        }

        gunList[selectedGun].AmmoInMag--;

        aud.PlayOneShot(gunList[selectedGun].shootSound, gunList[selectedGun].shootSoundVol);

        GunSettings currentGun = gunList[selectedGun];

        Vector3 spawnPos = gunLocation.transform.TransformPoint(currentGun.barrelTip.localPosition);
        Vector3 spawnScopedPos = isAiming ? gunLocation.transform.TransformPoint(currentGun.barrelTip.localPosition) : spawnPos;
        Quaternion spawnRotation = isAiming ? gunLocation.transform.rotation : gunLocation.transform.rotation;

        Vector3 bulletDirection = CalculateBulletDirection();

        // Check if the currently selected gun is the shotgun
        if (currentGun.isShotgun)
        {
            for (int i = 0; i < currentGun.shotgunPelletCount; i++)
            {
                // Apply shotgun spread
                Vector3 spreadDirection = Quaternion.Euler(Random.Range(-currentGun.shotgunPelletSpread, currentGun.shotgunPelletSpread), Random.Range(-currentGun.shotgunPelletSpread, currentGun.shotgunPelletSpread), 0f) * bulletDirection;

                Quaternion spreadRotation = Quaternion.LookRotation(bulletDirection);

                // Instantiate the Bullet instance
                Bullet bulletInstance = Instantiate(Playerbullet, isAiming ? spawnScopedPos : spawnPos, spreadRotation).GetComponent<Bullet>();

                // Set Bullet properties
                bulletInstance.Spawn(spreadDirection * currentGun.PlayerBulletSpeed, currentGun.PlayerBulletDamage);
            }
        }
        else
        {
            // Create a new Bullet instance
            Bullet bulletInstance = Instantiate(Playerbullet, spawnScopedPos, spawnRotation).GetComponent<Bullet>();

            // Set Bullet properties
            bulletInstance.Spawn(bulletDirection * currentGun.PlayerBulletSpeed, currentGun.PlayerBulletDamage);
        }
        UpdatePlayerUI();
        // Instantiate the muzzle flash particle effect system
        currentMuzzleFlash = Instantiate(currentGun.muzzleFlash, spawnPos, spawnRotation);

        yield return new WaitForSeconds(shootRate);

        isShooting = false;
        Destroy(currentMuzzleFlash.gameObject);
        

        //For Future Implementation:

        //This code could be used in a bool and toggle set by the player for automatic fire for guns with the automatic fire bool
        // This code will instantiate the bullets at random transforms mimicking the recoil of automatic fire


        //Vector3 bulletDirection = CalculateBulletDirection();
        //Vector3 bulletDirection = Quaternion.Euler(Random.Range(-currentGun.bulletSpread, currentGun.bulletSpread), Random.Range(-currentGun.bulletSpread, currentGun.bulletSpread), 0f) * bulletDirection;
        //Quaternion spreadRotation = Quaternion.LookRotation(bulletDirection);

        //// Instantiate the Bullet instance
        //Bullet bulletInstance = Instantiate(Playerbullet, isAiming ? spawnScopedPos : spawnPos, spreadRotation).GetComponent<Bullet>();

        //// Set Bullet properties
        //bulletInstance.Spawn(spreadDirection * currentGun.PlayerBulletSpeed, currentGun.PlayerBulletDamage);
    }


    // Calculate bullet direction based on the center of the screen
    private Vector3 CalculateBulletDirection()
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        int layerMask = ~(1 << playerLayer);

        Vector3 screenCenter = new Vector3(0.5f, 0.5f, 0f);
        Ray ray = Camera.main.ViewportPointToRay(screenCenter);
        RaycastHit hit;

        Vector3 bulletDirection = ray.direction;
        //(ray, out hit, Mathf.Infinity, layerMask)
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
        {
            Vector3 targetPoint = hit.point;
            bulletDirection = (targetPoint - gunLocation.transform.position).normalized;
        }

        return bulletDirection;
    }

    public void UpdatePlayerUI()
    {
        gameManager.instance.ammoCounter.text = gunList[selectedGun].AmmoInMag.ToString("0");
        gameManager.instance.maxAmmoCounter.text = gunList[selectedGun].PlayerTotalAmmo.ToString("0");
    }
}
