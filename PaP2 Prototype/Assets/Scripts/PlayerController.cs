using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    [Header("Components")]
    [SerializeField] CharacterController controller;

    [Header("Player Stats")]
    [SerializeField] int HP;
    [SerializeField] float Stamina;
    [SerializeField] float playerSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;
    [SerializeField] private float sprintSpeed;  //Z- changed the sprintMod as the sprint stoped working because of it
    [SerializeField] float crouchMod;
    [SerializeField] float crouchDist;
    //[SerializeField] float crouchTransitionSpeed;

    [Header("Gun Stats")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] int shootDamage;
    [SerializeField] int bulletDestroyTime;
    [SerializeField] float shootRate;

    private Vector3 playerVelocity;
    private Vector3 move;
    private bool groundedPlayer;
    private int jumpCount;
    private Vector3 crouchCameraDist;
    private bool isShooting;

    //Z- added HP Stamina and bools for running and stamina restoring
    private int HPOriginal;
    private float StaminaOrig;
    public float staminaRunCost;
    public float staminaRestoreSpeed;
    private bool isRunning;
    private bool isStaminaRestore;
    //Z- a way to store the initial speed can make it easier later
    private float initialSpeed;


    // Start is called before the first frame update
    void Start()
    {
        crouchCameraDist = new Vector3(0, crouchDist / 2, 0);
        //Z- Set all placeholders and updating the UI
        HPOriginal = HP;
        StaminaOrig = Stamina;
        initialSpeed = playerSpeed;
        UpdatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        RunCode();
        Crouch();

        

        if (Input.GetButtonDown("Fire1") && !isShooting && !gameManager.instance.isPaused)
        {
            StartCoroutine(Shoot());
        }

        //Identical movement code in the lectures
        groundedPlayer = controller.isGrounded;
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

        //Z- added a way for stamina to restore
        if (!isStaminaRestore && !isRunning && Stamina < StaminaOrig)
        {
            StartCoroutine(RestoreStamina());
        }
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    //Z- Changed the Sprint() to RunCode because of the IEnumerator wasnt nessesary TBH
    void RunCode()
    {
        //Z- Changed the GetButtonDown to GetButton so when the user holds down the key it changed the UI
        //i tried to keep the original but it only changed things once, this way calles it multiple times
        if (Input.GetButton("Sprint") && Stamina > 0.2f)
        {
            if (!isRunning)
            {
                StartCoroutine(Sprint());
                playerSpeed = sprintSpeed;
            }
        }
        else //Z- once the user is no longer holding the button it resets the speed, again i will try to make this better
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

    //Z- Sets the running bool takes one from stamina 
    IEnumerator Sprint()
    { 
        isRunning = true;
        Stamina -= 1;
        yield return new WaitForSeconds(staminaRunCost);
        isRunning = false;
    }

    //Z- Sets the Restoring bool and Adds one to the Stamina
    IEnumerator RestoreStamina()
    {
        isStaminaRestore = true;
        Stamina += 1;
        yield return new WaitForSeconds(staminaRestoreSpeed);
        isStaminaRestore = false;
    }

    //Z- Added UI so health and Stamina works
    void UpdatePlayerUI()
    {
        //Update player HP and stamina
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOriginal;
        gameManager.instance.playerStaminaBar.fillAmount = Stamina / StaminaOrig;
    }
}
