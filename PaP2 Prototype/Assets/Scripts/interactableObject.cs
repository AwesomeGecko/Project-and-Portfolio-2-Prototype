using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactableObject : MonoBehaviour {

    public bool playerInRange;
    public string ItemName;
    private int ammoAmount = 25;
    private int healAmount = 5;
    [SerializeField] Animator animator;
    [SerializeField] Collider interactCollider;

    public string GetItemName()
    {
        return ItemName;
    }


    void Update()
    {
        gather();
    }

    public void gather()
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

    IEnumerator openBox()
    {
        animator.SetTrigger("isOpen");
        interactCollider.enabled = false;
        yield return new WaitForSeconds(20f);
        animator.SetTrigger("isClosed");
        interactCollider.enabled = true;
    }
}
