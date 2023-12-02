using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
            rb.velocity = transform.forward * speed;
        }
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        if(other.tag != "Player" && rb.tag != "PlayerBullet") { }
        else
        {
            IDamage dmg = other.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(damage);
            }
            Destroy(gameObject);
        }

        

    }
}
