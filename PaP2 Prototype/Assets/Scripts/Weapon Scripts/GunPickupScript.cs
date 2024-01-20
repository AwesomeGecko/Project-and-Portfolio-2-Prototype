using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickupScript : MonoBehaviour
{
    
    [SerializeField] GunSettings gun;
    bool triggerSet;
    // Start is called before the first frame update
    void Start()
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
            gameManager.instance.playerGunControls.getGunStats(gun);
            Destroy(gameObject);
        }
    }
}
