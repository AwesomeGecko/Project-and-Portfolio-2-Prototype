using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject closedDoor; // Closed Door Object
    public GameObject openDoor; // Open Door Object
    public GameObject interact; // Interaction Icon Object

    public float timeToOpen; // Checks how many seconds it takes to open door

    void OnTriggerStay(Collider other) // When the player's camera trigger is staying with the door
    {
        if (other.CompareTag("MainCamera")) // If the tag colliding with the door equals the MainCamera Collider
        {
            interact.SetActive(true); // Interact Icon gets set to True
            if(Input.GetKeyDown(KeyCode.E)) // If we press the 'E' key
            {
                interact.SetActive(false); // Interaction Icon turns OFF
                closedDoor.SetActive(false); // Closed Door Object will turn OFF
                openDoor.SetActive(true); // Open Door Object will turn ON
                StartCoroutine(CloseDoors()); // Coroutine starts CloseDoors() method
            }
        }
    }

    IEnumerator CloseDoors() // Method to close door
    {
        yield return new WaitForSeconds(timeToOpen); // Total amount of seconds it will wait for door to open/close
        openDoor.SetActive(false); // Open Door Object gets turned OFF
        closedDoor.SetActive(true); // Closed Door Object gets turned ON
    }

    void OnTriggerExit(Collider other) // When the player looks away from the door
    {
        if(other.CompareTag("MainCamera"))
        {
            interact.SetActive(false); // The Interaction Icon Object will turn OFF
        }
    }
}
