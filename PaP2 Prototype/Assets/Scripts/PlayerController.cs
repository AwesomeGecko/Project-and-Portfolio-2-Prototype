using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;

    [SerializeField] float playerSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;
    [SerializeField] float sprintMod;
    [SerializeField] float crouchMod;
    [SerializeField] float crouchDist;
    //[SerializeField] float crouchTransitionSpeed;

    private Vector3 playerVelocity;
    private Vector3 move;
    private bool groundedPlayer;
    private int jumpCount;
    private Vector3 crouchCameraDist;


    // Start is called before the first frame update
    void Start()
    {
        crouchCameraDist = new Vector3(0, crouchDist / 2, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Sprint();
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
            //Debug.Log("player is crouching");
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            controller.height += crouchDist;
            playerSpeed /= crouchMod;
            Camera.main.transform.localPosition += crouchCameraDist;
            //Debug.Log("player is no longer crouching");
        }
    }
}
