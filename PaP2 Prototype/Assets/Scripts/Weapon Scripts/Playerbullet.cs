using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerbullet : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    private int damage;
    private int destroyTime;
    private int speed;
    private Vector3 direction;
    public ParticleSystem sparkParticles;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        ////Make sure rb is not null
        //if(rb != null )
        //{
        //    if (rb.tag == "PlayerBullet")
        //    {
        //        rb.velocity = Camera.main.transform.forward * speed;
        //    }
        //    else
        //    {
        //        rb.velocity = (gameManager.instance.player.transform.position - rb.transform.position) * speed;
        //    }
        //}

        Destroy(gameObject, destroyTime);
    }

    public void SetBulletProperties(int damage, int destroyTime, int speed, Vector3 initialDirection)
    {
        direction = initialDirection.normalized;
        this.damage = damage;
        this.destroyTime = destroyTime;
        this.speed = speed;
        
    }
    private void Update()
    {
        rb.AddForce(direction * speed);
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        // Check if the bullet collided with a surface
        if (rb.CompareTag("PlayerBullet"))
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                return;
            }
            //else if (collision.gameObject.CompareTag("PlayerBullet"))
            //{
            //    return;
            //}
        }
        if (!collision.collider.isTrigger)
        {
            // Instantiate the spark particle system at the collision point
            Instantiate(sparkParticles, collision.contacts[0].point, Quaternion.identity);

            IDamage dmg = collision.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(damage);
            }
            
        }
        Destroy(gameObject);
    }
}
