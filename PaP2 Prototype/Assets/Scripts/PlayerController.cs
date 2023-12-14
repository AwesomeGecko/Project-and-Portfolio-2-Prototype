
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{


    [Header("Components")]
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource aud;

    [Header("Player Stats")]
    [SerializeField] public int HP;
    [SerializeField] float Stamina;
    [SerializeField] float playerSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;
    [SerializeField] private float sprintSpeed;
    [SerializeField] float crouchMod;
    [SerializeField] float crouchDist;

    [Header("Gun Stats")]
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform scopedShootPos;
    [SerializeField] int shootDamage;
    [SerializeField] int bulletDestroyTime;
    [SerializeField] float shootRate;
    [SerializeField] public int ammoCounter;
    [SerializeField] public int maxAmmo;
    private int gameManagerAmmo;

    [Header("Audio")]
    [SerializeField] AudioClip[] soundSteps;
    [Range(0f, 1f)][SerializeField] float soundStepsVol;
    [SerializeField] AudioClip playerHurt;

    private Vector3 playerVelocity;
    private Vector3 move;
    private bool groundedPlayer;
    private int jumpCount;
    private Vector3 crouchCameraDist;
    private bool isShooting;
    bool isPlayingSteps;

    [Header("Gameplay Info")]
    public int HPOriginal;
    private float StaminaOrig;
    public float staminaRunCost;
    public float staminaRestoreSpeed;
    private bool isRunning;
    private bool isStaminaRestore;
    private float initialSpeed;


    //Gun logic
   
    [SerializeField] int shootDist;
    [SerializeField] GameObject gunModel;
    [SerializeField] gunStats defaultPistol;
    private bool isAiming;
    private float defaultFOV;
    int selectedGun;
    public Camera scopeIn;

    // Start is called before the first frame update
    void Start()
    {
        //EquipGun(currentGunIndex);

        crouchCameraDist = new Vector3(0, crouchDist / 2, 0);
        HPOriginal = HP;
        StaminaOrig = Stamina;
        initialSpeed = playerSpeed;

        //Default field of view for the player
        defaultFOV = Camera.main.fieldOfView;

        if (defaultPistol != null)
        {
            getGunStats(defaultPistol);
        }
        else
        {
            Debug.LogError("Default pistol scriptable object is not assigned in the Unity Editor.");
        }

        playerRespawn();
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
            Movement();
       
    }
    
    void Movement()
    {
        RunCode();
        Crouch();

        //Identical movement code in the lectures
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && move.normalized.magnitude > 0.3f && !isPlayingSteps)
        {
            StartCoroutine(PlaySteps());
        }

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            jumpCount = 0;
        }

        move = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;

        controller.Move(move * playerSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && jumpCount < 1)
        {
            playerVelocity.y = jumpHeight;
            jumpCount++;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (!isStaminaRestore && !isRunning && Stamina < StaminaOrig)
        {
            StartCoroutine(RestoreStamina());
        }
    }

    IEnumerator PlaySteps()
    {
        isPlayingSteps = true;
        aud.PlayOneShot(soundSteps[Random.Range(0, soundSteps.Length - 1)], soundStepsVol);
        if(!isRunning)
        {
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
        }
        isPlayingSteps = false;
    }


    public void teleportPlayer()
    {
        controller.enabled = false;
        transform.position = gameManager.instance.TeleportPos.transform.position;
        controller.enabled = true;
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

        if (isAiming)
        {
            Instantiate(bullet, scopedShootPos.position, transform.rotation);
        }
        else
        {
            Instantiate(bullet, shootPos.position, transform.rotation);
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
        UpdatePlayerUI();
    }

    void RunCode()
    {
        if (Input.GetButton("Sprint") && Stamina > 0.2f)
        {
            if (!isRunning)
            {
                StartCoroutine(Sprint());
                playerSpeed = sprintSpeed;
            }
        }
        else
        {
            playerSpeed = initialSpeed;
        }

        UpdatePlayerUI();
    }

    void Crouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            controller.height -= crouchDist;
            playerSpeed *= crouchMod;
            Camera.main.transform.localPosition -= crouchCameraDist;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            controller.height += crouchDist;
            playerSpeed /= crouchMod;
            Camera.main.transform.localPosition += crouchCameraDist;
        }
    }
    public void takeDamage(int amount)
    {
        aud.PlayOneShot(playerHurt); // Plays sound effect immediately upon taking damage
        HP -= amount;
        UpdatePlayerUI();
        if (HP <= 0)
        {
            gameManager.instance.youLose();
        }
        if (amount >= 1)
        { 
            gameManager.instance.damageIndicator();
        }
    }

    IEnumerator Sprint()
    { 
        isRunning = true;
        Stamina -= 1;
        yield return new WaitForSeconds(0.8f);
        isRunning = false;
    }

    IEnumerator RestoreStamina()
    {
        isStaminaRestore = true;
        Stamina += 1;
        yield return new WaitForSeconds(staminaRestoreSpeed);
        isStaminaRestore = false;
    }

    public void playerRespawn()
    {
        HP = HPOriginal;
        UpdatePlayerUI();

        controller.enabled = false;
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;
    }

    public void UpdatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOriginal;
        gameManager.instance.playerStaminaBar.fillAmount = Stamina / StaminaOrig;
        gameManager.instance.ammoCounter.text = gunList[selectedGun].ammoCur.ToString("0");
        gameManager.instance.maxAmmoCounter.text = ammoCounter.ToString("0");
    }

    
    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);
        selectedGun = gunList.Count - 1;
        gun.ammoCur = gun.magSize;
        shootDamage = gun.shootDamage;
        shootDist = gun.shootDist;
        shootRate = gun.shootRate;
        ammoCounter = gun.magSize;
        maxAmmo = gun.ammoMax;

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
        shootDamage = gunList[selectedGun].shootDamage;
        shootDist = gunList[selectedGun].shootDist;
        shootRate = gunList[selectedGun].shootRate;
        ammoCounter = gunList[selectedGun].ammoCur;
        maxAmmo = gunList[selectedGun].ammoMax;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;
        isShooting = false;
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

}
