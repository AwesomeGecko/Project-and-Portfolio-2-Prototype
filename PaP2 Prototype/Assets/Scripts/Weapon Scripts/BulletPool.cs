using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoBehaviour
{
    public ObjectPool<Bullet> pool;
    private PlayerGunControls gunControls;
    private TrailRenderer bulletTrail;

   
    private void Start()
    {
        gunControls = GetComponent<PlayerGunControls>();
        pool = new ObjectPool<Bullet> (CreateBullet, TakeFromPool, ReturnToPool, DestroyInPool, true, 100, 100);
             
    }

    private Bullet CreateBullet()
    {
        
        Bullet bullet = Instantiate(gunControls.BulletPrefab, gunControls.isAiming ? gunControls.spawnScopedPos : gunControls.spawnPos, gunControls.spawnRotation);

        bullet.SetPool(pool);
        return bullet;
    }

    private void TakeFromPool(Bullet bullet)
    {
       
        bullet.transform.position = gunControls.isAiming ? gunControls.spawnScopedPos : gunControls.spawnPos;
        bullet.transform.rotation = gunControls.spawnRotation;

        bullet.Rigidbody.velocity = Vector3.zero;

        bullet.gameObject.SetActive(true);
    }


    private void ReturnToPool(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);

        bulletTrail = bullet.GetComponent<TrailRenderer>();
        if (bulletTrail != null)
        {
            bulletTrail.enabled = false;
        }      
    }

    private void DestroyInPool(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }
}
