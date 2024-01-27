using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickupScript : MonoBehaviour
{

    [SerializeField] GunSettings gun;
    private PlayerGunControls gunControl;
    public bool playerInRange;
    private float speed;
    [SerializeField] Collider PickupCollider;
    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Get the PlayerGunControls script from the player GameObject
            gunControl = player.GetComponent<PlayerGunControls>();
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
        speed = gun.PickupRotateSpeed * Time.deltaTime * 40f;
        transform.Rotate(Vector3.up, speed);
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
}
