using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempDamager : MonoBehaviour
{
    [SerializeField] int damageAmount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null)
        {
            dmg.takeDamage(damageAmount);
        }
    }
}
