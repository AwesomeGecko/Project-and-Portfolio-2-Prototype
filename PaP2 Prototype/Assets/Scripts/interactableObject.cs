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

    void Start()
    {
        initialY = transform.position.y;
        animator = GetComponent<Animator>();
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

    void ammoBox()
    {
        gameManager.instance.isAmmo = true;
        if (gameManager.instance.playerScript.ammoCounter < gameManager.instance.playerScript.maxAmmo) //if current ammo is less than max
        {
            StartCoroutine(openBox());
            gameManager.instance.playerScript.ammoCounter += ammoAmount; //adds ammo

            //Add the amount of ammo directly to the specific gun
            gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].totalAmmo += ammoAmount;

            if (gameManager.instance.playerScript.ammoCounter >= gameManager.instance.playerScript.maxAmmo) //if current ammo is greater than max
            {
                gameManager.instance.playerScript.ammoCounter = gameManager.instance.playerScript.maxAmmo; //sets back to max
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
}
