using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class interactableObject : MonoBehaviour {

    [SerializeField] public TextMeshProUGUI Counter_Text;
    [SerializeField] public Image Counter_Image;
    public int Counter_Countdown;
    public int Counter_RemaingTime;
    [SerializeField] private string id;
    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    public bool playerInRange;
    public string ItemName;
    private int ammoAmount = 40;
    private int healAmount = 15;
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
                //Debug.Log("Added item to inventory!");
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


        

        foreach (GunSettings gun in gameManager.instance.playerGunControls.gunList)
        {
            if (gun.PlayerTotalAmmo < gun.MaxGunAmmo)
            {
                StartCoroutine(openBox());
                CountDownTimer(20);
                gun.PlayerTotalAmmo = gun.MaxGunAmmo;


                gameManager.instance.maxText.text = $"Max Ammo";
                gameManager.instance.runText();
            }
            else if (gun.PlayerTotalAmmo == gun.MaxGunAmmo)
            {
                gameManager.instance.maxItems();
            }
            gameManager.instance.isAmmo = false;
            gameManager.instance.playerGunControls.UpdatePlayerUI();
        }
    }


    void healthBox()
    {
        gameManager.instance.isHP = true;
        PlayerController playerScript = gameManager.instance.playerScript;
        
        if (playerScript.HP < playerScript.HPOriginal)
        {
            StartCoroutine(openBox());
            CountDownTimer(20);

            int initialHealth = playerScript.HP;
            playerScript.HP += healAmount;

            if (playerScript.HP + healAmount > playerScript.HPOriginal)
            {
                playerScript.HP = playerScript.HPOriginal;
            }
                int actualHealed = Mathf.Clamp(playerScript.HP - initialHealth, 0, healAmount);

            gameManager.instance.maxText.text = $"Healed by {actualHealed}";
            gameManager.instance.runText();
        }
        else if(playerScript.HP == playerScript.HPOriginal)
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

    public void CountDownTimer(int seconds)
    {
        Counter_RemaingTime = seconds;
        StartCoroutine(CountDown());
    }

    IEnumerator CountDown() 
    {
        while (Counter_RemaingTime >= 0)
        {
            Counter_Text.text = Counter_RemaingTime.ToString();
            Counter_Image.fillAmount = Mathf.InverseLerp(0, Counter_Countdown, Counter_RemaingTime);
            Counter_RemaingTime--;
            yield return new WaitForSeconds(1f);
        }
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
