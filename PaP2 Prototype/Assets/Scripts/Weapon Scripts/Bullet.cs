using System.Collections;
using System.Collections.Generic;
using UnityEngine;

  [RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    private int objectsPenetrated;
    [SerializeField] private int damage; // Serialized for inspection, but can be set programmatically
    public Rigidbody Rigidbody { get; private set; }
    [field: SerializeField] public Vector3 SpawnLocation { get; private set; }

    [field: SerializeField] public Vector3 SpawnVelocity { get; private set; }
    [field: SerializeField] public int Damage { get; private set; }

    public delegate void CollisionEvent(Bullet Bullet, Collision Collision, int ObjectsPenetrated);

    public event CollisionEvent OnCollision;
    public ParticleSystem sparkParticles;

    private void Awake()
    {
          Rigidbody = GetComponent<Rigidbody>();
    }

    public void Spawn(Vector3 spawnForce, int damage)
    {
        objectsPenetrated = 0;
        transform.forward = spawnForce.normalized;
        Rigidbody.AddForce(spawnForce, ForceMode.Impulse); // Use ForceMode.Impulse for instant velocity change
        StartCoroutine(DelayedDisable(2));
        this.damage = damage; // Set the damage property
    }
    private IEnumerator DelayedDisable(float time)
    {
        yield return new WaitForSeconds(time);
        DisableBullet();
    }

    private void OnCollisionEnter(Collision collision)
    {
        DisableBullet(); // Disable the bullet immediately upon collision

        if (collision.collider.CompareTag("PlayerBullet"))
        {
            return; // Ignore collisions with other player bullets
        }

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
        OnCollision?.Invoke(this, null, objectsPenetrated);
        objectsPenetrated++;
        gameObject.SetActive(false); // Instead of destroying, just deactivate the GameObject
    }

    private void OnDisable()
    {
          StopAllCoroutines();
          Rigidbody.velocity = Vector3.zero;
          Rigidbody.angularVelocity = Vector3.zero;
          OnCollision = null;
    }
  }
