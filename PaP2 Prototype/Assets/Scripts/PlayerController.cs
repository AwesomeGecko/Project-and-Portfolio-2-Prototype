using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;

    [SerializeField] int HP;
    [SerializeField] float playerSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;
    [SerializeField] float sprintMod;
    [SerializeField] float crouchMod;
    [SerializeField] float crouchDist;
    //[SerializeField] float crouchTransitionSpeed;

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


    // Start is called before the first frame update
    void Start()
    {
        crouchCameraDist = new Vector3(0, crouchDist / 2, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        Sprint();
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
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            playerSpeed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            playerSpeed /= sprintMod;
        }
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
        //UpdatePlayerUI();
        if (HP <= 0)
        {
            //you died
        }
    }

    void UpdatePlayerUI()
    {
        //Update player HP and stamina
    }
}
