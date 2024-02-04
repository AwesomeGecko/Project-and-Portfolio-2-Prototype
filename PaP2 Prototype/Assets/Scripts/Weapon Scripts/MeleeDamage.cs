using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDamage : MonoBehaviour
{
    [SerializeField] int damage;
    bool hitObject;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            if (other.CompareTag("PlayerBullet"))
            {
                return; // Ignore collisions with other player bullets
            }

            if(!other.CompareTag("Enemy"))
            {
                hitObject = true;
                return;
            }

            IDamage dmg = other.GetComponent<IDamage>();

            if (dmg != null && !hitObject)
            {
                dmg.takeDamage(damage);
            }
        }
        hitObject = false;
    }
}
