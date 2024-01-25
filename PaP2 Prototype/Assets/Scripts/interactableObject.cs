using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class interactableObject : MonoBehaviour {

    [SerializeField] private string id;
    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

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
        aud = GetComponent<AudioSource>();
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
            if (gameManager.instance.DebugLogs)
            {
                Debug.Log("Added item to inventory!");
            }

            InteractSound();


            if (ItemName == "Ammo")
            {
                ammoBox(); //opens and resets ammo box after time
            }

            if (ItemName == "Health")
            {
                healthBox(); //opens and resets health box after time
            }

            if (ItemName == "Checkpoint")
            {
                giveCheckpoint(); //opens and resets checkpoint box after time
            }

            if (ItemName == "TP Key")
            {
                
                keyCollector();
                if (gameManager.instance.keysCollected == 3)
                {
                    //Collect all 3 keys sound here
                    gameManager.instance.isTPOn = true;
                }
                Destroy(gameObject);
                //used to turn on teleporter
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
        ammoCur = gameManager.instance.playerGunControls.gunList[gameManager.instance.playerGunControls.selectedGun].AmmoInMag;
        magSize = gameManager.instance.playerGunControls.gunList[gameManager.instance.playerGunControls.selectedGun].MagSize;
        maxAmmo = gameManager.instance.playerGunControls.maxAmmo;
        totalAmmo = gameManager.instance.playerGunControls.gunList[gameManager.instance.playerGunControls.selectedGun].PlayerTotalAmmo;
        ammoMax = gameManager.instance.playerGunControls.gunList[gameManager.instance.playerGunControls.selectedGun].MaxGunAmmo;
    }

    void ammoBox()
    {
       
        gameManager.instance.isAmmo = true;
        int bulletsNeeded = gameManager.instance.playerGunControls.gunList[gameManager.instance.playerGunControls.selectedGun].MagSize - gameManager.instance.playerGunControls.gunList[gameManager.instance.playerGunControls.selectedGun].PlayerTotalAmmo;

        //if current ammo is less than max
        if (gameManager.instance.playerGunControls.gunList[gameManager.instance.playerGunControls.selectedGun].PlayerTotalAmmo < gameManager.instance.playerGunControls.gunList[gameManager.instance.playerGunControls.selectedGun].MaxGunAmmo)
        {
            StartCoroutine(openBox());
            if (gameManager.instance.playerGunControls.gunList[gameManager.instance.playerGunControls.selectedGun].PlayerTotalAmmo + ammoAmount > gameManager.instance.playerGunControls.gunList[gameManager.instance.playerGunControls.selectedGun].MaxGunAmmo)
            {
                //Add the amount needed to the gun and none over ex: magSize = 10 ammoAmmount = 25
                //subtracts magSize from totalAmmo to give propper refill
                ammoAmount = bulletsNeeded;
                gameManager.instance.playerGunControls.gunList[gameManager.instance.playerGunControls.selectedGun].PlayerTotalAmmo += ammoAmount;
                gameManager.instance.maxText.text = $"Ammo Given {ammoAmount}";
                ammoAmount = ammoReset;
            }
            else 
            {
                //Add the amount of ammo directly to the specific gun magazine
                gameManager.instance.playerGunControls.gunList[gameManager.instance.playerGunControls.selectedGun].PlayerTotalAmmo += ammoAmount;
                gameManager.instance.maxText.text = $"Ammo Given {ammoAmount}";
            }
            gameManager.instance.runText();
        }
        else
        {
            gameManager.instance.maxItems();
        }
        gameManager.instance.isAmmo = false;
        gameManager.instance.playerGunControls.UpdatePlayerUI();
    }

    void healthBox()
    {
        
        gameManager.instance.isHP = true;
        if (gameManager.instance.playerScript.HP < gameManager.instance.playerScript.HPOriginal)
        {
            StartCoroutine(openBox());
            gameManager.instance.playerScript.HP += healAmount;
            gameManager.instance.maxText.text = $"Healed by {healAmount}";
            gameManager.instance.runText();
        }
        else
        {
            gameManager.instance.maxItems();
        }
        gameManager.instance.isHP = false;

    }

    void giveCheckpoint()
    {
        animator.SetTrigger("isOpen");
        interactCollider.enabled = false;
    }

    void keyCollector()
    {
        if (gameManager.instance.keysRemain > 0)
        {
            gameManager.instance.keysRemain--;
            gameManager.instance.keysCollected++;
            gameManager.instance.maxText.text = $"Keys remain {gameManager.instance.keysRemain}";
            gameManager.instance.runText();
            
        }
        else
        {
            gameManager.instance.maxText.text = "Teleporter is open";
            gameManager.instance.runText();
        }
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
            AudioSource.PlayClipAtPoint(interactSound, transform.position);
        }
    }    

    
}
