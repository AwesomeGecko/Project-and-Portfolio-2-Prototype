using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

  [RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
   
    [SerializeField] private int damage; // Serialized for inspection, but can be set programmatically
    public Rigidbody Rigidbody { get; private set; }
    [field: SerializeField] public Vector3 SpawnLocation { get; private set; }

    [field: SerializeField] public Vector3 SpawnVelocity { get; private set; }
    [field: SerializeField] public int Damage { get; private set; }

    public delegate void CollisionEvent(Bullet Bullet, Collision Collision);

    public event CollisionEvent OnCollision;
    public ParticleSystem sparkParticles;

    bool isReleased = false;

    private ObjectPool<Bullet> _pool;

    private void Awake()
    {
          Rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        isReleased = false;
        StartCoroutine(DelayedDisable(1));
    }

    public void Spawn(Vector3 spawnForce, int damage)
    {
        
        transform.forward = spawnForce.normalized;
        Rigidbody.AddForce(spawnForce, ForceMode.Impulse); // Use ForceMode.Impulse for instant velocity change
        
        this.damage = damage; // Set the damage property
    }
    private IEnumerator DelayedDisable(float time)
    {
        yield return new WaitForSeconds(time);
        DisableBullet();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("PlayerBullet") || collision.collider.CompareTag("WeaponPickUp"))
        {
            return; // Ignore collisions with other player bullets
        }
        DisableBullet(); // Disable the bullet immediately upon collision

        // Instantiate the spark particle system at the collision point
        Instantiate(sparkParticles, collision.contacts[0].point, Quaternion.identity);

        IDamage dmg = collision.collider.GetComponent<IDamage>();

        if (dmg != null)
        {
            dmg.takeDamage(damage);
        }
    }

    private void DisableBullet()
    {
        if (!isReleased)
        {
            isReleased = true;
            OnCollision?.Invoke(this, null);       
            _pool.Release(this);
        }
    }

    private void OnDisable()
    {
          StopAllCoroutines();
        Rigidbody.velocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
          OnCollision = null;
    }

    public void SetPool(ObjectPool<Bullet> pool)
    {
        _pool = pool;
    }
}
