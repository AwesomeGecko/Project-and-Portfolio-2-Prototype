using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactableObject : PlayerController
{
    public bool playerInRange;
    public string ItemName;
    public int ammoAmount = 25;

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
                gameManager.instance.ammoCounter.text = ammoAmount.ToString();
                //PlayerController.instance.ammoCounter += ammoAmount;
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
