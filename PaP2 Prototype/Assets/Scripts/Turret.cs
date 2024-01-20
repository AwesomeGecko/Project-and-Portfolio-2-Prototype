using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Turret : MonoBehaviour
{

    [Header("Turret Models")]
    [SerializeField] GameObject turretMount;
    [SerializeField] GameObject turretHead;
    [Space]
    [SerializeField] Transform aimPoint;
    [SerializeField] int viewCone;
    Vector3 playerDir;
    float playerDirMount;

    
    public Transform turrHeadTrans;
    public Transform turrMountTrans;
    

    [SerializeField] float shootRate;
    [SerializeField] int targetFaceSpeed;
    bool isShooting;
    //[SerializeField] Animator anim;
    bool PlayerInRange;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerInRange)
        {
            canSeePlayer();
        }
    }
    void canSeePlayer()
    {
       // playerDir = Vector3(gameManager.instance.player.transform.position.x - turretMount.transform.position.x, turretMount.transform.position.y, turretMount.transform.position.z);
        playerDir = gameManager.instance.player.transform.position - aimPoint.position;


        float angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(aimPoint.position, playerDir);

        RaycastHit hit;

        if (Physics.Raycast(aimPoint.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") /*&& angleToPlayer <= viewCone*/)
            {
                faceTarget();
                //if (!isShooting)
                //{
                //    StartCoroutine(shoot());
                //}
                
            }
        }
       
       
    }

    IEnumerator shoot()
    {
        isShooting = true;
      //  anim.SetTrigger("Shoot");
        yield return new WaitForSeconds(shootRate);
        isShooting = false;

    }
    void faceTarget()
    {
       // turretHead.transform.LookAt( gameManager.instance.player.transform.position);

        Vector3 playerTemp = gameManager.instance.player.transform.position - turretHead.transform.position;
        Quaternion target = Quaternion.LookRotation(playerTemp);
        turretHead.transform.rotation = Quaternion.Slerp(turrHeadTrans.rotation, target, targetFaceSpeed * Time.deltaTime);
        
       // Quaternion midRot = Quaternion.Euler(-90f, 0f, -target.eulerAngles.z);
        //turrMountTrans.rotation = Quaternion.Slerp(turrMountTrans.rotation, midRot, targetFaceSpeed * Time.deltaTime);

      // Quaternion rot = Quaternion.LookRotation(new Vector3(gameManager.instance.player.transform.position.x - turretMount.transform.position.x, gameManager.instance.player.transform.position.y - turretMount.transform.position.y, gameManager.instance.player.transform.position.z - turretMount.transform.position.z));
       // turretHead.transform.rotation = Quaternion.Lerp(turretHead.transform.rotation, rot, targetFaceSpeed);
       // turretMount.transform.rotation = Quaternion.Euler(rot.ro);
      
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInRange = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInRange = false;
        }
    }
}
