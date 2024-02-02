using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IDamage, IDataPersistence
{
    [Header("Components")]
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource aud;

    [Header("Player Stats")]
    [SerializeField] public int HP;
    [SerializeField] public int SavedHP;
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
    [SerializeField] float cameraMoveDist;

    [Header("Audio")]
    [SerializeField] AudioClip[] soundSteps;
    [Range(0f, 1f)][SerializeField] float soundStepsVol;
    [SerializeField] AudioClip playerHurt;
    [SerializeField] AudioClip playerDies;
    [SerializeField] AudioClip lowHealth;
    [SerializeField] AudioClip staminaRestore;
    [Range(0f, 1f)][SerializeField] float staminaRestoreVol;
    [SerializeField] AudioClip playerJump;
    [SerializeField] AudioClip playerLand;
    [Range(0f, 1f)][SerializeField] float playerLandVol;

    private Vector3 playerVelocity;
    private Vector3 move;
    private bool groundedPlayer;
    private int jumpCount;
    private Vector3 crouchCameraDist;
    bool isPlayingSteps;
    Quaternion initialRotation;
    bool isCrouching;
    bool isSliding;
    float slideMod;
    bool isLowHealth;
    bool isLanded = false;
    

    [Header("Gameplay Info")]
    public int HPOriginal;
    private float StaminaOrig;
    public float staminaRunCost;
    public float staminaRestoreSpeed;
    private bool isRunning;
    private bool isStaminaRestore;

    //Gun logic
    private float initialSpeed;
    public bool isDead = false;
    private PlayerGunControls playerGunControls;
    private Animator gunAnim;
    // Start is called before the first frame update
    void Start()
    {
        gunAnim = gameManager.instance.playerGunControls.gunHolder.GetComponent<Animator>();
        crouchCameraDist = new Vector3(0, crouchDist / 2, 0);
        HPOriginal = HP;
        StaminaOrig = Stamina;
        initialSpeed = playerSpeed;

        playerGunControls = GetComponent<PlayerGunControls>();


        playerRespawn();
    }

    // Update is called once per frame
    void Update()
    {    

        Movement();
        if (HP <= lowHP && !isLowHealth)
        {
            StartCoroutine(PlayHeartbeat());
        }
    }

    public void LoadData(GameData data)
    {
        SavedHP = data.Health;
        //Stamina = data.Stamina;
        HP = SavedHP;
    }

    public void SaveData(GameData data)
    {
        data.Health = SavedHP;
        //data.Stamina = Stamina;
        SavedHP = HP;
    }


    void Movement()
    {
        StartCoroutine(Sprint());
        Crouch();
        StartCoroutine(Slide());
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
            if (!isLanded)
            {
                if (playerLand != null)
                {
                    aud.PlayOneShot(playerLand, playerLandVol);
                }
                isLanded = true;
            }
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
            isLanded = false;
            if (playerJump != null)
            {
                aud.PlayOneShot(playerJump);
            }
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
        aud.PlayOneShot(soundSteps[UnityEngine.Random.Range(0, soundSteps.Length - 1)], soundStepsVol);
        if (!isRunning)
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

    void Crouch()
    {
        if (!gameManager.instance.isPaused)
        {
            if (Input.GetButtonDown("Crouch"))
            {
                isCrouching = true;
                Debug.Log(isCrouching);
                controller.height -= crouchDist;
                playerSpeed = crouchSpeed;
                Camera.main.transform.localPosition -= crouchCameraDist;

                // CR: Adjusts player footsteps louder/faster
                soundStepsVol = 0.2f;
            }
            else if (Input.GetButtonUp("Crouch"))
            {
                isCrouching = false;
                Debug.Log(isCrouching);
                controller.height += crouchDist;
                playerSpeed = initialSpeed;
                Camera.main.transform.localPosition += crouchCameraDist;

                // CR: Reset back to original volume value
                soundStepsVol = 0.5f;
            }

        }

    }

    IEnumerator Slide()
    {

        if (Input.GetButton("Crouch"))
        {
            if (Input.GetButtonDown("Sprint"))
            {
                if (Stamina >= 1)
                {
                    slideMod = 1;
                    Stamina -= 1;
                }
            }
            if (slideMod > 0)
            {
                isSliding = true;
                controller.Move(transform.forward * slideSpeed * Time.deltaTime * slideMod);
                slideMod -= Time.deltaTime;
                if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
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
        else
        {
            slideMod = 0;
            isSliding = false;
        }
        yield return null;
    }

    void Lean()
    {
        if (Input.GetButtonDown("LeanLeft") && !gameManager.instance.onTarget)
        {
            initialRotation = transform.rotation;
            // Calculate the lean rotation on the local Z-axis
            Camera.main.transform.position -= Camera.main.transform.right * cameraMoveDist;
            Quaternion leanRotation = Quaternion.Euler(0, 0, leanDist);

            // Smoothly interpolate between the initial rotation and the lean rotation
            transform.rotation = Quaternion.Lerp(initialRotation, initialRotation * leanRotation, leanSpeed);
        }
        if(Input.GetButtonDown("LeanRight") && !gameManager.instance.onTarget)
        {
            initialRotation = transform.rotation;
            // Calculate the lean rotation on the local Z-axis
            Camera.main.transform.position += Camera.main.transform.right * cameraMoveDist;
            Quaternion leanRotation = Quaternion.Euler(0, 0, -leanDist);

            // Smoothly interpolate between the initial rotation and the lean rotation
            transform.rotation = Quaternion.Lerp(initialRotation, initialRotation * leanRotation, leanSpeed);
        }

        if (Input.GetButtonUp("LeanLeft") || Input.GetButtonUp("LeanRight"))
        {
            Quaternion leanRotation = Quaternion.Euler(0, 0, leanDist);
            transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            Camera.main.transform.position = transform.position + Vector3.up;
        }
    }

    IEnumerator Sprint()
    {
        if (Input.GetButton("Sprint") && Stamina > 0.2f)
        {
            if (!isRunning)
            {
                isRunning = true;
                playerSpeed = sprintSpeed;
                if (move != Vector3.zero || isSliding)
                {
                    Stamina -= 1;
                }
                yield return new WaitForSeconds(0.8f);
                isRunning = false;
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

        UpdatePlayerUI();
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
    public void teleportToSpawn()
    {
        controller.enabled = false;
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;
    }

    public void playerRespawn()
    {
        isDead = false;
        HP = HPOriginal;
        UpdatePlayerUI();
        
        controller.enabled = false;
        playerGunControls.isAiming = false;
        gunAnim.SetTrigger("NotAiming");
        //if (gameManager.instance.playerStats.Chapter == 0)
        //{
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
       // }
       // else 
       // {
           // transform.position = gameManager.instance.Checkpoint_Alpha.transform.position;
       // }
        controller.enabled = true;

        if(isCrouching && !Input.GetButton("Crouch"))
        {
            isCrouching = false;
            Debug.Log(isCrouching);
            controller.height += crouchDist;
            playerSpeed = initialSpeed;
            Camera.main.transform.localPosition += crouchCameraDist;
            soundStepsVol = 0.5f;
        }
    }

    public void UpdatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOriginal;
        gameManager.instance.playerStaminaBar.fillAmount = Stamina / StaminaOrig;
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
