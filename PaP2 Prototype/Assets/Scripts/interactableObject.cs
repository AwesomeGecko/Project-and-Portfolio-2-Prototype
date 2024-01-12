using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactableObject : MonoBehaviour {

    public bool playerInRange;
    public string ItemName;
    private int ammoAmount = 25;
    private int healAmount = 5;
    Animator animator;
    [SerializeField] Collider interactCollider;

    public float rotationSpeed = 25f;
    public float bounceHeight = 0.1f;
    public float bounceSpeed = 1.0f;
    public float despawnDelay = 0.5f;
    public float delayBeforeEffects = 1.0f;
    private bool isPickedUp = false;
    private float initialY;

    private int ammoCur;
    private int magSize;
    private int maxAmmo;
    private int ammoMax;
    private int totalAmmo;
    private int ammoReset;

    // Audio
    [Header("Audio")]
    [SerializeField] public AudioSource aud;
    public AudioClip interactSound;
    [Range(0f, 1f)][SerializeField] float interactSoundVol;

    void Start()
    {
        initialY = transform.position.y;
        animator = GetComponent<Animator>();
        ammoReset = ammoAmount;
    }

    public string GetItemName()
    {
        return ItemName;
    }


    void Update()
    {
        interact();
        if (!isPickedUp)
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);


        if (!isPickedUp)
        {
            float newY = initialY + Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    public void interact()
    {

        if (Input.GetButtonDown("Interact") && playerInRange && gameManager.instance.onTarget)
        {
            
            Debug.Log("Item Added to Inventory");

            InteractSound();

            if (ItemName == "Ammo")
            {
                ammoBox(); //opens and resets ammo box after time
                
            }

            if (ItemName == "Health")
            {
                healthBox(); //opens and resets health box after time
            }

            if (ItemName == "TP Key")
            {
                //used to turn on teleporter
                keyCollector();
                if (gameManager.instance.keysCollected == 3)
                { 
                    gameManager.instance.isTPOn = true;
                }
            }

        }
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

    public void getVariables()  
    {
        //not used like a method just for refferance so i dont get confused...
        //DO NOT USE THESE VARIABLES, it wont work T_T use the gameManager refreance instead
        ammoCur = gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].ammoCur;
        magSize = gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].magSize;
        maxAmmo = gameManager.instance.playerScript.maxAmmo;
        totalAmmo = gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].totalAmmo;
        ammoMax = gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].ammoMax;
    }

    void ammoBox()
    {
        gameManager.instance.isAmmo = true;
        int bulletsNeeded = gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].magSize - gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].totalAmmo;

        //if current ammo is less than max
        if (gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].totalAmmo < gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].magSize)
        {
            StartCoroutine(openBox());
            if (gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].totalAmmo + ammoAmount > gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].magSize)
            {
                //Add the amount needed to the gun and none over ex: magSize = 10 ammoAmmount = 25
                //subtracts magSize from totalAmmo to give propper refill
                ammoAmount = bulletsNeeded;
                gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].totalAmmo += ammoAmount;
                ammoAmount = ammoReset;
            }
            else 
            {
                //Add the amount of ammo directly to the specific gun magazine
                gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].totalAmmo += ammoAmount;
            }
        }
        else
        {
            gameManager.instance.maxItems();
        }
        gameManager.instance.isAmmo = false;

    }

    void healthBox()
    {
        
        gameManager.instance.isHP = true;
        if (gameManager.instance.playerScript.HP < gameManager.instance.playerScript.HPOriginal)
        {
            StartCoroutine(openBox());
            gameManager.instance.playerScript.HP += healAmount;
        }
        else
        {
            gameManager.instance.maxItems();
        }
        gameManager.instance.isHP = false;

    }

    void keyCollector()
    {
        gameManager.instance.maxText.text = "Key Collected";
        gameManager.instance.runText();
        gameManager.instance.keysCollected++;
        Destroy(gameObject);
    }

    IEnumerator openBox()
    {
        animator.SetTrigger("isOpen");
        interactCollider.enabled = false;
        yield return new WaitForSeconds(20f);
        animator.SetTrigger("isClosed");
        interactCollider.enabled = true;
    }

    void InteractSound()
    {
        if(aud && interactSound != null)
        {
            float adjustedVolume = interactSoundVol * gameManager.instance.aud.volume;
            aud.PlayOneShot(interactSound, adjustedVolume);
        }
    }    

    
}
