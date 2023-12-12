using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class autoDoors : MonoBehaviour
{

    bool triggerSet;
    [SerializeField] Animator anim;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggerSet)
        {
            triggerSet = true;
            //give stats to player
            anim.SetBool("character_nearby", true);
            
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && triggerSet)
        {
            triggerSet = false;
            //give stats to player
            anim.SetBool("character_nearby", false);

           
        }
    }
}
