using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public partial class PlayerController : MonoBehaviour, IDamage
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
    [SerializeField] float leanDist;
    [SerializeField] float leanSpeed;

    [Header("Audio")]
    [SerializeField] AudioClip[] soundSteps;
    [Range(0f, 1f)][SerializeField] float soundStepsVol;
    [SerializeField] AudioClip playerHurt;
    [SerializeField] AudioClip reloadSound;
    [Range(0f, 1f)][SerializeField] float reloadSoundVol;

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
        Lean();

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

    public void Lean()
    {
        Quaternion initialRot = transform.localRotation;
        if (Input.GetButton("LeanLeft"))
        {
            Debug.Log("Leaning left");
            Quaternion newRot = Quaternion.Euler(transform.localRotation.x * leanDist, transform.localRotation.y, transform.localRotation.z * leanDist); 
            transform.localRotation = Quaternion.Slerp(transform.localRotation, newRot, Time.deltaTime * leanSpeed);
        }
        else if (Input.GetButton("LeanRight"))
        {
            Debug.Log("Leaning right");
            Quaternion newRot = Quaternion.Euler(transform.localRotation.x * -leanDist, transform.localRotation.y, transform.localRotation.z * -leanDist);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, newRot, Time.deltaTime * leanSpeed);
        }
        else
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, initialRot, Time.deltaTime * leanSpeed);
        }
    }

    IEnumerator Sprint()
    { 
        isRunning = true;
        if(move != Vector3.zero)
        {
            Stamina -= 1;
        }
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
        gameManager.instance.maxAmmoCounter.text = gunList[selectedGun].totalAmmo.ToString("0");
    }

}
