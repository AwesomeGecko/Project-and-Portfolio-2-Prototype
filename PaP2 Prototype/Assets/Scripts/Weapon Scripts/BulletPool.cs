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
        Debug.Log($"Created Bullet - Position: {bullet.transform.position}, Rotation: {bullet.transform.rotation.eulerAngles}");
        return bullet;
    }

    private void TakeFromPool(Bullet bullet)
    {
        Debug.Log($"Taking from pool - Current Position: {bullet.transform.position}, Current Rotation: {bullet.transform.rotation.eulerAngles}");
        bullet.transform.position = gunControls.isAiming ? gunControls.spawnScopedPos : gunControls.spawnPos;
        bullet.transform.rotation = gunControls.spawnRotation;

        bullet.Rigidbody.velocity = Vector3.zero;

        bullet.gameObject.SetActive(true);
    }


    private void ReturnToPool(Bullet bullet)
    {
        Debug.Log($"Returning to pool - Current Position: {bullet.transform.position}, Current Rotation: {bullet.transform.rotation.eulerAngles}");

        bullet.gameObject.SetActive(false);

        bulletTrail = bullet.GetComponent<TrailRenderer>();
        if (bulletTrail != null)
        {
            bulletTrail.enabled = false;
        }
        Debug.Log($"Returned to pool - New Position: {bullet.transform.position}, New Rotation: {bullet.transform.rotation.eulerAngles}");
    }

    private void DestroyInPool(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }
}
