using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    private int damage;
    private int destroyTime;
    private int speed;
    public ParticleSystem sparkParticles;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = (gameManager.instance.player.transform.position - rb.transform.position) * speed;        
        Destroy(gameObject, destroyTime);
    }
    public void SetBulletProperties(int damage, int destroyTime, int speed)
    {
        this.damage = damage;
        this.destroyTime = destroyTime;
        this.speed = speed;        
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (rb.CompareTag("EnemyBullet"))
        {
            if (collision.gameObject.CompareTag("EnemyBullet"))
            {
                return;
            }
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
