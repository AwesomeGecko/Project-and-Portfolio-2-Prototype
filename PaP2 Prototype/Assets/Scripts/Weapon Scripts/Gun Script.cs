using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    
    [SerializeField] gunStats gun;
    bool triggerSet;
    // Start is called before the first frame update
    void Awake()
    {
        gun.ammoCur = gun.magSize;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggerSet)
        {
            triggerSet = true;
            //give the stats to the player
            gameManager.instance.playerScript.getGunStats(gun);
            Destroy(gameObject);
        }
    }
}
