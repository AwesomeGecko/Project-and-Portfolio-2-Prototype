using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickupScript : MonoBehaviour
{
    
    [SerializeField] GunSettings gun;
    private PlayerGunControls gunControl;
    bool playerInRange;
    bool triggerSet;
    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Get the PlayerGunControls script from the player GameObject
            gunControl = player.GetComponent<PlayerGunControls>();

            if (gunControl != null)
            {
                gun.ammoCur = gun.magSize;
            }
        }   
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Interact") && playerInRange)
        {
            if (gunControl.gunList.Count < 2)
            {
                PickUpGun();
            }
            else
            {
                Debug.Log("SwapGuns");
                gameManager.instance.playerGunControls.SwapGuns();
                PickUpGun();
            }
        }
    }

    private void PickUpGun()
    {
        gameManager.instance.playerGunControls.getGunStats(gun);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (gunControl.gunList.Count <= 1)
    //    {
    //        if (other.CompareTag("Player") && !triggerSet)
    //        {
    //            triggerSet = true;
    //            //give the stats to the player
    //            gameManager.instance.playerGunControls.getGunStats(gun);
    //            Destroy(gameObject);
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("you can not pick me up");
    //    }
    //}
}
