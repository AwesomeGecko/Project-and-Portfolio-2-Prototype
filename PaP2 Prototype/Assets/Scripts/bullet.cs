using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    [SerializeField] int damage;
    [SerializeField] int destroyTime;
    [SerializeField] int speed;


    // Start is called before the first frame update
    void Start()
    {
        //
        if(rb.tag == "PlayerBullet")
        {
            rb.velocity = Camera.main.transform.forward * speed;
        }
        else
        {
            rb.velocity = (gameManager.instance.player.transform.position - rb.transform.position) * speed;
        }
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;
        
        if(rb.CompareTag("PlayerBullet"))
        {
            if(other.CompareTag("Player"))
            {
                return;
            }
        }

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null)
        {
            dmg.takeDamage(damage);
        }
        Destroy(gameObject);
    }
}
