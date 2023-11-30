using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactableObject : MonoBehaviour
{
    public bool playerInRange;
    public string ItemName;

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


        }
    }


    private void OnTriggerEnter(Collider other)
    {
        
        playerInRange = true;
        
    }

    private void OnTriggerExit(Collider other)
    {
        
        playerInRange = false;
        
    }
}
