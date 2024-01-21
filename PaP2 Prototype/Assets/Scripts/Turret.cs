using System.Collections;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class Turret : MonoBehaviour
{

    [Header("Turret Models")]
    [SerializeField] Turret tr;
    [SerializeField] GameObject turretMount;
    [SerializeField] GameObject turretHead;
    [SerializeField] Transform turretLeftTop;
    [SerializeField] Transform turretRightTop;
    [SerializeField] Transform turretLeftBottom;
    [SerializeField] Transform turretRightBottom;
    [Space]
    [SerializeField] Transform aimPoint;
    [SerializeField] int colliderRadius;
    [SerializeField] int bulletDamage;
    [SerializeField] int bulletDestroyTime;
    [SerializeField] int bulletSpeed;
    [SerializeField] int viewCone;
    Vector3 playerDir;
    float playerDirMount;
   


    [SerializeField] GameObject Bullet;
    [SerializeField] float shootRate;
    [SerializeField] int targetFaceSpeed;
    bool isShooting;
    [SerializeField] Animator anim;
    public bool PlayerInRange;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        
            canSeePlayer();
        
       
    }
    void canSeePlayer()
    {
       // playerDir = Vector3(gameManager.instance.player.transform.position.x - turretMount.transform.position.x, turretMount.transform.position.y, turretMount.transform.position.z);
        playerDir = gameManager.instance.player.transform.position - aimPoint.position;

        
        if (Math.Abs(playerDir.x) >= colliderRadius || Math.Abs(playerDir.y) >= colliderRadius || Math.Abs(playerDir.z) >= colliderRadius)
        {
            playerDir = new Vector3(0,0,0);
           
        }

        float angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(aimPoint.position, playerDir);

        RaycastHit hit;

        if (Physics.Raycast(aimPoint.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player")/*&& angleToPlayer <= viewCone*/)
            {
                faceTarget();
                
                    anim.SetBool("PlayerInRange", true);

            }
            else
            {
                anim.SetBool("PlayerInRange", false);
            }
        }
        else
        {
            anim.SetBool("PlayerInRange", false);
        }
       
       
    }

    //IEnumerator shoot()
    //{
      
    //        isShooting = true;
            
            

    //        yield return new WaitForSeconds(shootRate);


            
        

    //}
    void faceTarget()
    {

       // turretHead.transform.LookAt( gameManager.instance.player.transform.position);

        Vector3 playerTemp = gameManager.instance.player.transform.position - turretHead.transform.position;
        Quaternion target = Quaternion.LookRotation(playerTemp);
        turretHead.transform.rotation = Quaternion.Slerp(turretHead.transform.rotation, target, targetFaceSpeed * Time.deltaTime);
        
       // Quaternion midRot = Quaternion.Euler(-90f, 0f, -target.eulerAngles.z);
        //turrMountTrans.rotation = Quaternion.Slerp(turrMountTrans.rotation, midRot, targetFaceSpeed * Time.deltaTime);

      // Quaternion rot = Quaternion.LookRotation(new Vector3(gameManager.instance.player.transform.position.x - turretMount.transform.position.x, gameManager.instance.player.transform.position.y - turretMount.transform.position.y, gameManager.instance.player.transform.position.z - turretMount.transform.position.z));
       // turretHead.transform.rotation = Quaternion.Lerp(turretHead.transform.rotation, rot, targetFaceSpeed);
       // turretMount.transform.rotation = Quaternion.Euler(rot.ro);
      
    }
    public void CreateLeftBullets()
    {
        GameObject newTopBullet = Instantiate(Bullet, turretLeftTop.position, transform.rotation);
        GameObject newBottomBullet = Instantiate(Bullet, turretLeftBottom.position, transform.rotation);
        EnemyBullet enemyTopBullet = newTopBullet.GetComponent<EnemyBullet>();
        EnemyBullet enemyBottomBullet = newBottomBullet.GetComponent<EnemyBullet>();
        enemyTopBullet.SetBulletProperties(bulletDamage, bulletDestroyTime, bulletSpeed);
        enemyBottomBullet.SetBulletProperties(bulletDamage, bulletDestroyTime, bulletSpeed);

    }
    public void CreateRightBullets()
    {
        GameObject newTopBullet = Instantiate(Bullet, turretRightTop.position, transform.rotation);
        GameObject newBottomBullet = Instantiate(Bullet, turretRightBottom.position, transform.rotation);
        EnemyBullet enemyTopBullet = newTopBullet.GetComponent<EnemyBullet>();
        EnemyBullet enemyBottomBullet = newBottomBullet.GetComponent<EnemyBullet>();
        enemyTopBullet.SetBulletProperties(bulletDamage, bulletDestroyTime, bulletSpeed);
       enemyBottomBullet.SetBulletProperties(bulletDamage, bulletDestroyTime, bulletSpeed);

    }
    
    
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInRange = true;
            anim.SetBool("PlayerInRange", true);
        }
        
        
    }
    

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInRange = false;
            anim.SetBool("PlayerInRange", false);
        }
    }

    
}
