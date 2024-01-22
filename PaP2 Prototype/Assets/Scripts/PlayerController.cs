using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public partial class PlayerController : MonoBehaviour, IDamage, IDataPersistence
{
    [Header("Components")]
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource aud;
    //[SerializeField] Animator animator;

    [Header("Player Stats")]
    [SerializeField] public int HP;
    [SerializeField] public int lowHP; // CR
    [SerializeField] public float Stamina;
    [SerializeField] float playerSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;
    [SerializeField] private float sprintSpeed;
    [SerializeField] float crouchSpeed;
    [SerializeField] float crouchDist;
    [SerializeField] float slideSpeed;
    [SerializeField] float leanDist;
    [SerializeField] float leanSpeed;

    [Header("Audio")]
    [SerializeField] AudioClip[] soundSteps;
    [Range(0f, 1f)][SerializeField] float soundStepsVol;
    [SerializeField] AudioClip playerHurt;
    [SerializeField] AudioClip playerDies; // CR
    [SerializeField] AudioClip lowHealth; // CR
    [SerializeField] AudioClip staminaRestore; // CR
    [Range(0f, 1f)][SerializeField] float staminaRestoreVol; // CR

    private Vector3 playerVelocity;
    private Vector3 move;
    private bool groundedPlayer;
    private int jumpCount;
    private Vector3 crouchCameraDist;
    private bool isShooting;
    bool isPlayingSteps;
    Quaternion initialRotation;
    bool isSliding;
    float slideMod;
    bool isLowHealth; // CR

    [Header("Gameplay Info")]
    public int HPOriginal;
    private float StaminaOrig;
    public float staminaRunCost;
    public float staminaRestoreSpeed;
    private bool isRunning;
    private bool isStaminaRestore;

    public int ammoToSave;
    public int maxAmmoToSave;

    //Gun logic
    private float initialSpeed;
    private bool isDead = false;
    //
    public void LoadData(GameData data)
    {
        transform.position = data.playerPosition;
        HP = data.Health;
        Stamina = data.Stamina;
        ammoToSave = data.ammo;
        maxAmmoToSave = data.maxAmmo;
}

    public void SaveData(GameData data)
    {
        data.playerPosition = transform.position;
        data.Health = HP;
        data.Stamina = Stamina;
        data.ammo = ammoToSave;
        data.maxAmmo = maxAmmoToSave;
    }


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
            gunList[selectedGun].ammoCur = gunList[selectedGun].magSize;
        }
        else
        {
            Debug.LogError("Default pistol scriptable object is not assigned in the Unity Editor.");
        }

        playerRespawn();
        int.TryParse(gameManager.instance.ammoCounter.text, out gameManagerAmmo);
        ammoCounter = gameManagerAmmo;
        UpdatePlayerUI();
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
       // CR
        if(HP <= lowHP && !isLowHealth)
        {
            StartCoroutine(PlayHeartbeat());
        }
        
    }



    void Movement()
    {
        RunCode();
        Crouch();
        
        //SLIDING YEAH
        if (Input.GetButton("Crouch"))
        {
            if (Input.GetButtonDown("Sprint"))
            {
                slideMod = 1;
                Stamina -= 1;
            }
            Slide();
        }
        else
        {
            slideMod = 0;
            isSliding = false;
        }

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

        if (!isSliding)
        {
            move = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;

            controller.Move(move * playerSpeed * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump") && jumpCount < 1)
        {
            playerVelocity.y = jumpHeight;
            jumpCount++;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        //LEAN
        if (Input.GetButtonDown("LeanLeft") || (Input.GetButtonDown("LeanRight") && !gameManager.instance.onTarget))
        {
            initialRotation = transform.rotation;
            Lean();
        }

        if (Input.GetButtonUp("LeanLeft") || Input.GetButtonUp("LeanRight"))
        {
            Quaternion leanRotation = Quaternion.Euler(0, 0, leanDist);
            transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        }

        if (!isStaminaRestore && !isRunning && Stamina < StaminaOrig)
        {
            StartCoroutine(RestoreStamina());
        }
    }

    IEnumerator PlaySteps()
    {
        isPlayingSteps = true;
        aud.PlayOneShot(soundSteps[UnityEngine.Random.Range(0, soundSteps.Length - 1)], soundStepsVol);
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

    void RunCode()
    {
        if (Input.GetButton("Sprint") && Stamina > 0.2f)
        {
            if (!isRunning)
            {
                StartCoroutine(Sprint());
                playerSpeed = sprintSpeed;

                // CR: Adjusts player footsteps louder/faster
                soundStepsVol = 1.0f;
            }
        }
        else
        {
            playerSpeed = initialSpeed;

            // CR: Reset back to original volume value
            soundStepsVol = 0.5f;
        }
        
        if(Input.GetButtonUp("Sprint"))
        {
            
        }

        UpdatePlayerUI();
    }

    void Crouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            controller.height -= crouchDist;
            playerSpeed = crouchSpeed;
            Camera.main.transform.localPosition -= crouchCameraDist;

            // CR: Adjusts player footsteps louder/faster
            soundStepsVol = 0.2f;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            controller.height += crouchDist;
            playerSpeed = initialSpeed;
            Camera.main.transform.localPosition += crouchCameraDist;

            // CR: Reset back to original volume value
            soundStepsVol = 0.5f;
        }
    }

    void Slide()
    {
        if(slideMod > 0)
        {
            isSliding = true;
            controller.Move(transform.forward * slideSpeed * Time.deltaTime * slideMod);
            slideMod -= Time.deltaTime;
            if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                slideMod = 0;
                isSliding = false;
            }
        }
        else
        {
            isSliding = false;
        }
    }

    void Lean()
    {
        if(Input.GetButtonDown("LeanLeft"))
        {
            // Calculate the lean rotation on the local Z-axis
            Quaternion leanRotation = Quaternion.Euler(0, 0, leanDist);

            // Smoothly interpolate between the initial rotation and the lean rotation
            transform.rotation = Quaternion.Lerp(initialRotation, initialRotation * leanRotation, leanSpeed);
        }
        if(Input.GetButtonDown("LeanRight"))
        {
            // Calculate the lean rotation on the local Z-axis
            Quaternion leanRotation = Quaternion.Euler(0, 0, -leanDist);

            // Smoothly interpolate between the initial rotation and the lean rotation
            transform.rotation = Quaternion.Lerp(initialRotation, initialRotation * leanRotation, leanSpeed);
        }
    }

    IEnumerator Sprint()
    { 
        isRunning = true;
        if(move != Vector3.zero || isSliding)
        {
            Stamina -= 1;
        }
        yield return new WaitForSeconds(0.8f);
        isRunning = false;
    }

    IEnumerator RestoreStamina()
    {
        if (!Input.GetButton("Sprint"))
        {
            isStaminaRestore = true;
            aud.PlayOneShot(staminaRestore, staminaRestoreVol);
            Stamina += 1;
            yield return new WaitForSeconds(staminaRestoreSpeed);
            isStaminaRestore = false;
        }
    }

    public void takeDamage(int amount)
    {
        if (isDead)
            return;

        aud.PlayOneShot(playerHurt); // Plays sound effect immediately upon taking damage
        HP -= amount;
        UpdatePlayerUI();
        if (HP <= 0)
        {
            isDead = true;
            // CR
            StartCoroutine(PlayerDiesAndLoses());
            //gameManager.instance.youLose();
        }
        if (amount >= 1)
        {
            gameManager.instance.damageIndicator();
        }
    }

    public void playerRespawn()
    {
        isDead = false;
        HP = HPOriginal;
        UpdatePlayerUI();

        controller.enabled = false;


        //if (gameManager.instance.playerStats.Chapter == 0)
        //{
            transform.position = gameManager.instance.playerSpawnPos.transform.position;
       // }
       // else 
       // {
           // transform.position = gameManager.instance.Checkpoint_Alpha.transform.position;
       // }
        controller.enabled = true;
    }

    public void UpdatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOriginal;
        gameManager.instance.playerStaminaBar.fillAmount = Stamina / StaminaOrig;
        gameManager.instance.ammoCounter.text = gunList[selectedGun].ammoCur.ToString("0");
        gameManager.instance.maxAmmoCounter.text = gunList[selectedGun].totalAmmo.ToString("0");
    }

    public void UpdateStats()
    {
        
    }

    // CR Method
    public IEnumerator PlayerDiesAndLoses() // Has the player play death sfx first, waits a second and then calls youLose()
    {
        aud.PlayOneShot(playerDies);
        yield return new WaitForSeconds(1.0f);
        gameManager.instance.youLose();
    }

    // CR Method
    public IEnumerator PlayHeartbeat()
    {
        isLowHealth = true;
        while(HP <= lowHP) // Adjust if needed depending on actual Player HP value. lowHealth sfx loops until HP is restored past lowHP limit.
        {
            aud.PlayOneShot(lowHealth);
            yield return new WaitForSeconds(1.0f);
        }
        isLowHealth = false;
    }
}
