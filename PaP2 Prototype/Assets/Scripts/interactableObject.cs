using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactableObject : MonoBehaviour {

    public bool playerInRange;
    public string ItemName;
    private int ammoAmount = 25;
    private int healAmount = 5;


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

            Destroy(gameObject);

            if (ItemName == "Ammo")
            {
                gameManager.instance.playerScript.ammoCounter += ammoAmount;
            }
            if (ItemName == "Health")
            {
                gameManager.instance.playerScript.HP += healAmount;
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
}
