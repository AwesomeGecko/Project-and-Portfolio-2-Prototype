
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{


    [Header("Components")]
    [SerializeField] CharacterController controller;

    [Header("Player Stats")]
    [SerializeField] public int HP;
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
    [SerializeField] public int ammoCounter;
    private int gameManagerAmmo;

    private Vector3 playerVelocity;
    private Vector3 move;
    private bool groundedPlayer;
    private int jumpCount;
    private Vector3 crouchCameraDist;
    
    private bool interactPickup;

    //Z- added HP Stamina and bools for running and stamina restoring
    private int HPOriginal;
    private float StaminaOrig;
    public float staminaRunCost;
    public float staminaRestoreSpeed;
    private bool isRunning;
    private bool isStaminaRestore;
    //Z- a way to store the initial speed can make it easier later
    private float initialSpeed;

    //Gun logic
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] int shootDist;
    [SerializeField] GameObject gunModel;
    int selectedGun;
    private bool isShooting;


    // Start is called before the first frame update
    void Start()
    {
        //EquipGun(currentGunIndex);

        ammoCounter = 10;
        crouchCameraDist = new Vector3(0, crouchDist / 2, 0);
        //Z- Set all placeholders and updating the UI
        HPOriginal = HP;
        StaminaOrig = Stamina;
        initialSpeed = playerSpeed;
        UpdatePlayerUI();
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
                    StartCoroutine(Shoot());

                selectGun();
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

        gunList[selectedGun].ammoCur--;


        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
            Instantiate(gunList[selectedGun].hitEffect, hit.point, transform.rotation);

            Debug.Log(hit.transform.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (hit.transform != transform && dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        

        ammoCounter -= 1;


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
        yield return new WaitForSeconds(0.8f);
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
        gameManager.instance.ammoCounter.text = ammoCounter.ToString("0");
    }

    
    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);

        shootDamage = gun.shootDamage;
        shootDist = gun.shootDist;
        shootRate = gun.shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.model.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void selectGun()
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

    void changeGun()
    {
        shootDamage = gunList[selectedGun].shootDamage;
        shootDist = gunList[selectedGun].shootDist;
        shootRate = gunList[selectedGun].shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;
        isShooting = false;
    }
}
