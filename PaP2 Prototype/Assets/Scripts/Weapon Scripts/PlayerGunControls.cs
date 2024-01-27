using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerGunControls : MonoBehaviour
{

    [Header("Gun Stats")]
    [SerializeField] public List<GunSettings> gunList = new List<GunSettings>();
    [SerializeField] GameObject Playerbullet;
    [SerializeField] Transform BackPack;
    [SerializeField] float shootRate;
    [SerializeField] int PlayerBulletDamage;

    [SerializeField] int PlayerBulletSpeed;
    [SerializeField] public int ammoCounter;
    [SerializeField] public int maxAmmo;
    [SerializeField] int shootDist;
    [SerializeField] Transform gunLocation;
    [SerializeField] Transform gunRotation;
    [SerializeField] public GunSettings defaultPistol;
    public bool isAiming;
    public float defaultFOV;
    public int selectedGun;
    private int InBackPack;
    public bool isShooting;
    public Camera scopeIn;
    public int gameManagerAmmo;
    private PlayerController playerController;
    [SerializeField] AudioSource aud;
    private float reloadDuration;
    private GameObject gunPrefab;
    private ParticleSystem MuzzleFlash;
    private Animator animator;
   
    public bool IsIdle;
    public Bullet BulletPrefab;
   
    private BulletPool BulletPool;
    public bool IsReloading;
    public Vector3 spawnPos;
    public Vector3 spawnScopedPos;
    public Quaternion spawnRotation;

    GunAnimations GunAnimations;
    private bool IsAutoFiring;
    private int BackPackGun;

    void Start()
    {
        //Default field of view for the player
        defaultFOV = Camera.main.fieldOfView;

        getGunStats(defaultPistol);

        int.TryParse(gameManager.instance.ammoCounter.text, out gameManagerAmmo);
        ammoCounter = gameManagerAmmo;

        BulletPool = GetComponent<BulletPool>();

        
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.instance.isPaused)
        {
            if (gunList.Count > 0)
            {
                if (Input.GetButton("Fire1") && !isShooting && !IsReloading)
                {
                    StartCoroutine(Shoot());
                }

                selectGun();

                if (Input.GetButtonDown("AimDownSight"))
                {
                    ToggleAimDownSights();
                }

                if (Input.GetButtonDown("Reload") && !isShooting && !IsReloading && !isAiming)
                {
                    StartCoroutine(ReloadingDuration());
                }

                //if (Input.GetButtonDown("Inspect"))
                //{

                //    WeaponInspect();
                //}
            }
           
        }
    }
    

    //Inspect animation for guns


    //public void WeaponInspect()
    //{
    //    GunSettings currentGun = gunList[selectedGun];
    //    GunAnimator.Play(currentGun.InspectAnimName);
    //}

    //IEnumerator InspectAnimDuration()
    //{

    //    yield return new WaitForSeconds(gunList[selectedGun].InspectDuration);
    //}
    
    IEnumerator ReloadingDuration()
    {
        IsIdle = false;
        IsReloading = true;
        

        GunSettings currentGun = gunList[selectedGun];
        // Check if the gun is not already full
        if (gunList[selectedGun].AmmoInMag < gunList[selectedGun].MagSize && gunList[selectedGun].PlayerTotalAmmo > 0)
        {
            //GunAnimator.SetBool("IsReloading", true);
            CallAnimation();
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

                
                aud.PlayOneShot(gunList[selectedGun].reloadSound, gunList[selectedGun].reloadSoundVol);
            }
            else if (gunList[selectedGun].PlayerTotalAmmo > 0)
            {

                // Reload with the remaining ammo
                gunList[selectedGun].AmmoInMag += gunList[selectedGun].PlayerTotalAmmo;

                // Reset total ammo to 0
                gunList[selectedGun].PlayerTotalAmmo = 0;

                // Update the UI
                UpdatePlayerUI();
                
                aud.PlayOneShot(gunList[selectedGun].reloadSound, gunList[selectedGun].reloadSoundVol);
            }
            else if (gunList[selectedGun].AmmoInMag == 0 && gunList[selectedGun].PlayerTotalAmmo == 0)
            {
                
                Debug.Log("Show out of ammo UI for a few seconds");
            }
            
        }

        yield return new WaitForSeconds(1f);

        IsReloading = false;
        IsIdle = true;
        CallAnimation();

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
            else if (currentGun.isAssaultRifle)
            {
                Debug.Log("Entering Scar case");
                //Using Invoke enables the assault rifle sight ontop of the main camera through a time delay
                Invoke("ActivateAssaultRifleSight", 0.3f);
                //Adjust the scope cameras FOV
                Camera.main.fieldOfView = currentGun.fieldOfView;
            }
            else if (currentGun.GunName == "AK-101")
            {
                //Using Invoke enables the assault rifle sight ontop of the main camera through a time delay      
                Invoke("ActivateAKSight", 0.3f);
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

            gunLocation.transform.localPosition = Vector3.zero;

            currentGun.model.transform.localPosition = currentGun.defaultGunPositionOffset;
            currentGun.model.transform.localRotation = currentGun.defaultRotation;

            //Enable the main crosshairs
            gameManager.instance.Crosshair.gameObject.SetActive(true);

            //Disable the scope camera
            gameManager.instance.Scope.gameObject.SetActive(false);

            //Disable the shotgun sight
            gameManager.instance.ShotgunSight.gameObject.SetActive(false);

            //Disable the Assualt rifle sight
            gameManager.instance.AssaultRifleSight.gameObject.SetActive(false);
            //Disable the AK Rifle Sight

            gameManager.instance.AKSight.gameObject.SetActive(false);

            //Cull the gun onto screen
            scopeIn.cullingMask = scopeIn.cullingMask | (1 << 7);

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

    //This method is to use the assault rifle UI sight.
    void ActivateAssaultRifleSight()
    {
        // Set local position using the selected gun's offset
        gunLocation.localPosition = gunList[selectedGun].ADSGunPositionOffset;

        gameManager.instance.AssaultRifleSight.gameObject.SetActive(true);
        scopeIn.cullingMask = scopeIn.cullingMask & ~(1 << 7);
    }

    void ActivateAKSight()
    {
        // Set local position using the selected gun's offset
        gunLocation.transform.localPosition = gunList[selectedGun].ADSGunPositionOffset;

        gameManager.instance.AKSight.gameObject.SetActive(true);
        scopeIn.cullingMask = scopeIn.cullingMask & ~(1 << 7);
    }


    public void getGunStats(GunSettings gun)
    {
        if (gunList.Count < 2)
        {
            int previousSelectedGun = selectedGun;

            // Check if there is an existing gunPrefab
            if (gunLocation.childCount > 0)
            {
                // Move the existing gun to the backpack
                

                Transform existingGun = gunLocation.GetChild(0);
                existingGun.SetParent(BackPack);
                //existingGun.gameObject.SetActive(false);
                existingGun.localRotation = Quaternion.identity;
                existingGun.localPosition = Vector3.zero;
                shootDist = gun.shootDist;
                shootRate = gun.shootRate;
                PlayerBulletDamage = gun.PlayerBulletDamage;

                PlayerBulletSpeed = gun.PlayerBulletSpeed;
              
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
           
            // Initialize ammo variables
            gun.AmmoInMag = gun.MagSize;

            if (gun.isdefaultPistol)
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
                gunPrefab = Instantiate(gun.model, gunLocation.transform.position, gunRotation.transform.rotation, gunLocation.transform);
                gunPrefab.name = gun.model.name;

                
                gunPrefab.transform.localPosition = gun.defaultGunPositionOffset;
                gunPrefab.transform.localRotation = gun.defaultRotation;
                animator = gunPrefab.GetComponentInChildren<Animator>();

                MuzzleFlash = gunPrefab.GetComponentInChildren<ParticleSystem>();




                
                // Set local rotation
                gunRotation.transform.localRotation = gun.defaultRotation;

                // Set local position using the selected gun's offset
                gunRotation.transform.localPosition = gun.defaultGunPositionOffset;

                
               
                // Update the player's UI
                UpdatePlayerUI();
                IsIdle = true;
                CallAnimation();
            }
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
        GunSettings gun = gunList[selectedGun];

        // Store current gun's Animator controller and parameters
        Animator currentGunAnimator = null;
        isShooting = false;  
        IsReloading = false;  
                                          

        // Deactivate the current gun
        if (gunLocation.childCount > 0)
        {
            Transform currentGun = gunLocation.GetChild(0);
            currentGun.gameObject.SetActive(true);

            // Store current gun's Animator controller
            currentGunAnimator = currentGun.GetComponentInChildren<Animator>();
          

            // Move the current gun to the backpack
            currentGun.SetParent(BackPack);
            currentGun.localRotation = Quaternion.identity;
            currentGun.localPosition = Vector3.zero;
            shootDist = gun.shootDist;
            shootRate = gun.shootRate;
            PlayerBulletDamage = gun.PlayerBulletDamage;
            PlayerBulletSpeed = gun.PlayerBulletSpeed;
        }

        // Use selectedGunIndex to access the gun in gunList
        shootDist = gun.shootDist;
        shootRate = gun.shootRate;
        PlayerBulletDamage = gun.PlayerBulletDamage;
        PlayerBulletSpeed = gun.PlayerBulletSpeed;

        MuzzleFlash = gunPrefab.GetComponentInChildren<ParticleSystem>();

        // Move the gun from the backpack to the player's hands
        if (BackPack.childCount > 0)
        {
            Transform nextGun = BackPack.GetChild(0);
            nextGun.SetParent(gunLocation);
            CallAnimation();

            // Use selectedGunIndex to access the gun in gunList
            shootDist = gun.shootDist;
            shootRate = gun.shootRate;
            PlayerBulletDamage = gun.PlayerBulletDamage;
            PlayerBulletSpeed = gun.PlayerBulletSpeed;

            MuzzleFlash = gunPrefab.GetComponentInChildren<ParticleSystem>();

            // Restore Animator controller and parameters
            animator = nextGun.GetComponentInChildren<Animator>();

            // Set more parameters as needed
            gunRotation.transform.localRotation = gun.defaultRotation;
            nextGun.localPosition = gun.defaultGunPositionOffset;
            nextGun.localRotation = gun.defaultRotation;

            
        }

        UpdatePlayerUI();
    }
    public void SwapGuns()
    {

        if (gunList.Count > 1 && selectedGun >= 0 && selectedGun < gunList.Count)
        {
           
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
            float bounceForceMagnitude = 50f; // Adjust the magnitude as needed
            Vector3 bounceForce = (bounceForceMagnitude * bounceDirection) * Time.deltaTime;

            // Apply the force to the Rigidbody
            gunRigidbody.AddForce(bounceForce, ForceMode.Impulse);
        }

    }

    IEnumerator Shoot()
    {
        IsIdle = false;
        isShooting = true;
        
        if (gunList[selectedGun].AmmoInMag <= 0)
        {
            IsIdle = true;
            isShooting = false;
            yield break;
        }

        CallAnimation();

        aud.PlayOneShot(gunList[selectedGun].shootSound, gunList[selectedGun].shootSoundVol);

        GunSettings currentGun = gunList[selectedGun];
        //GunAnimations.CallAnimation();
        

        spawnPos = gunLocation.transform.TransformPoint(currentGun.barrelTip.localPosition);
        spawnScopedPos = isAiming ? gunLocation.transform.TransformPoint(currentGun.barrelTip.localPosition) : spawnPos;
        spawnRotation = isAiming ? gunLocation.transform.rotation : gunLocation.transform.rotation;

        Vector3 bulletDirection = CalculateBulletDirection();

        // Check if the currently selected gun is the shotgun
        if (currentGun.isShotgun)
        {
            for (int i = 0; i < currentGun.shotgunPelletCount; i++)
            {
                // Apply shotgun spread
                Vector3 spreadDirection = Quaternion.Euler(Random.Range(-currentGun.shotgunPelletSpread, currentGun.shotgunPelletSpread), Random.Range(-currentGun.shotgunPelletSpread, currentGun.shotgunPelletSpread), 0f) * bulletDirection;

                Quaternion spreadRotation = Quaternion.LookRotation(bulletDirection);

                //Get the bullets from the pool
                Bullet bullet = BulletPool.pool.Get();

                // Set Bullet properties
                bullet.Spawn(spreadDirection * currentGun.PlayerBulletSpeed, currentGun.PlayerBulletDamage);
                TrailRenderer bulletTrail = bullet.GetComponent<TrailRenderer>();
                if (bulletTrail != null)
                {
                    bulletTrail.enabled = true;
                }
            }
        }
        else
        {

            //Get the bullet from the pool
            Bullet bullet = BulletPool.pool.Get();

            //// Set Bullet properties
            bullet.Spawn(bulletDirection * currentGun.PlayerBulletSpeed, currentGun.PlayerBulletDamage);
            TrailRenderer bulletTrail = bullet.GetComponent<TrailRenderer>();
            if (bulletTrail != null)
            {
                bulletTrail.enabled = true;
            }
        }

        gunList[selectedGun].AmmoInMag--;

        UpdatePlayerUI();
        // Instantiate the muzzle flash particle effect system
        MuzzleFlash.Play();
        
        yield return new WaitForSeconds(shootRate);
        
        isShooting = false;
        IsIdle = true;
        //GunAnimations.CallAnimation();
        CallAnimation();
        //GunAnimator.Play(currentGun.IdleAnimName);

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
        gameManager.instance.gunName.text = gunList[selectedGun].GunName.ToString();
        if (gunList.Count > 0)
        {
            if (gunLocation.childCount > 0)
            {
                gameManager.instance.GunIconHandsBackground.gameObject.SetActive(true);
                gameManager.instance.UpdateGunIcon(gunList[selectedGun].gunIcon, gameManager.instance.GunIconHands);
            }

            if (BackPack.childCount > 0)
            {
                int backpackIndex = (selectedGun + 1) % gunList.Count;
                gameManager.instance.GunIconBackPackBackground.gameObject.SetActive(true);
                gameManager.instance.UpdateGunIcon(gunList[backpackIndex].gunIcon, gameManager.instance.GunIconBackPack);
            }
            else if (BackPack.childCount < 0)
            {
                gameManager.instance.GunIconBackPackBackground.gameObject.SetActive(false);
                gameManager.instance.GunIconBackPack.enabled = false;
            }
        }
    }

    public void CallAnimation()
    {
        GunSettings currentGun = gunList[selectedGun];

        if (currentGun.GunName == "9MM")
        {
            animator.SetBool("Shotgun", false);
            animator.SetBool("AK-101", false);
            animator.SetBool("UpgradedPistol", false);
            animator.SetBool("M4A1", false);
            animator.SetBool("AssaultRifle", false);
            animator.SetBool("Is9MM", true);
            
            
            if (isShooting)
            {
                animator.SetTrigger("Shoot9MM");
                
            }

            else if (IsReloading)
            {
                animator.SetTrigger("Reload");
            }

            else if (IsIdle)
            {
                animator.SetTrigger("Idle");
            }
        }

        else if (currentGun.GunName == "Upgraded Pistol")
        {
            animator.SetBool("Shotgun", false);
            animator.SetBool("AK-101", false);
            animator.SetBool("Is9MM", false);
            animator.SetBool("M4A1", false);
            animator.SetBool("AssaultRifle", false);
            animator.SetBool("UpgradedPistol", true);
           
            if (isShooting)
            {
                animator.SetTrigger("ShootUpgradedPistol");
                
            }

            else if (IsReloading)
            {
                animator.SetTrigger("Reload");
            }

            else if (IsIdle)
            {
                animator.SetTrigger("Idle");
            }
        }

        else if (currentGun.GunName == "AK-101")
        {
            animator.SetBool("Shotgun", false);
            animator.SetBool("Is9MM", false);
            animator.SetBool("UpgradedPistol", false);
            animator.SetBool("M4A1", false);
            animator.SetBool("AssaultRifle", false);
            animator.SetBool("AK-101", true);

            if (isShooting)
            {
                
                animator.SetTrigger("Shoot");
            }

            else if (IsReloading)
            {
                animator.SetTrigger("Reload");
            }

            else if (IsIdle)
            {
                animator.SetTrigger("Idle");
            }
        }
        else if (currentGun.GunName == "Shotgun")
        {
            animator.SetBool("AK-101", false);
            animator.SetBool("Is9MM", false);
            animator.SetBool("UpgradedPistol", false);
            animator.SetBool("M4A1", false);
            animator.SetBool("AssaultRifle", false);
            animator.SetBool("Shotgun", true);

            if (isShooting)
            {
                animator.SetTrigger("Shoot");

            }

            else if (IsReloading)
            {
                animator.SetTrigger("Reload");
            }

            else if (IsIdle)
            {
                animator.SetTrigger("Idle");
            }
        }
        else if (currentGun.GunName == "M4A1")
        {
            animator.SetBool("AK-101", false);
            animator.SetBool("Is9MM", false);
            animator.SetBool("UpgradedPistol", false);
            animator.SetBool("Shotgun", false);
            animator.SetBool("AssaultRifle", false);
            animator.SetBool("M4A1", true);
            

            if (isShooting)
            {
                animator.SetTrigger("Shoot");

            }

            else if (IsReloading)
            {
                animator.SetTrigger("Reload");
            }

            else if (IsIdle)
            {
                animator.SetTrigger("Idle");
            }
        }
        else if (currentGun.GunName == "AssaultRifle")
        {
            animator.SetBool("AK-101", false);
            animator.SetBool("Is9MM", false);
            animator.SetBool("UpgradedPistol", false);
            animator.SetBool("Shotgun", false);
            animator.SetBool("M4A1", false);
            animator.SetBool("AssaultRifle", true);

            if (isShooting)
            {
                animator.SetTrigger("Shoot");

            }

            else if (IsReloading)
            {
                animator.SetTrigger("Reload");
            }

            else if (IsIdle)
            {
                animator.SetTrigger("Idle");
            }
        }
    }
}