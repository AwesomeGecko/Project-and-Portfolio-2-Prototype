using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour, IDamage
{

    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject player;
    [SerializeField] Transform headPos;
    [SerializeField] Animator anim;
    [SerializeField] Renderer model;
    [SerializeField] AudioSource aud;

    [Header("----- Enemy Stat -----")]
    [SerializeField] int HP;
    [SerializeField] int viewCone;
    [SerializeField] int speed;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [SerializeField] float animSpeedTrans;
    [SerializeField] int targetFaceSpeed;

    [Header("----- Weapon -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] float shootSpeed;
    [SerializeField] Transform enemyshootPos;
    [SerializeField] Transform enemyshootPos2;

    [Header("----- Audio -----")]
    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip deathSound;

    bool isShooting;
    bool PlayerInRange;
    bool destinationChosen;
    float angleToPlayer;
    Vector3 playerDir;
    Vector3 startingPos;
    float stoppingDistanceOrig;


    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.updateGameGoal(1);
        startingPos = transform.position;
        stoppingDistanceOrig = agent.stoppingDistance;

    }

    

    // Update is called once per frame
    void Update()
    {

        

        if (agent.isActiveAndEnabled)
        {

            float animspeed = agent.velocity.normalized.magnitude;
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), animspeed, Time.deltaTime * animSpeedTrans));


            if (PlayerInRange && !CanSeePLayer())
            {
                StartCoroutine(roam());
            }
            else if (!PlayerInRange)
            {
                StartCoroutine(roam());
            }
               
        }

    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerInRange = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerInRange = false;
        }
        
    }

    IEnumerator roam()
    {
        if (agent.remainingDistance < .05f && !destinationChosen)
        {
            destinationChosen = true;
            agent.stoppingDistance = 0;
            yield return new WaitForSeconds(roamPauseTime);

            Vector3 randomPos = Random.insideUnitSphere * roamDist;
            randomPos += startingPos;

            NavMeshHit hit; 
            NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
            agent.SetDestination(hit.position);

            destinationChosen = false;
        }
    }   

    bool CanSeePLayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewCone)
            {
                agent.SetDestination(gameManager.instance.player.transform.position);


                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }
                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    faceTarget();
                }       
            }
        }

        agent.stoppingDistance = stoppingDistanceOrig;

        return true;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * targetFaceSpeed);
    }

    IEnumerator shoot()
    {
        isShooting = true;
        anim.SetTrigger("Shoot");
        CreateBullet();

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void CreateBullet()
    {
        Instantiate(bullet, enemyshootPos.position, transform.rotation);

        if (gameObject.CompareTag("Big Robot"))
        {
            Instantiate(bullet, enemyshootPos2.position, transform.rotation);
        }
    }

    public void takeDamage(int amount)
    {
            HP -= amount;         

        if (HP <= 0)
        {
            aud.PlayOneShot(deathSound);
            gameManager.instance.updateGameGoal(-1);
            anim.SetBool("Dead", true);
            agent.enabled = false;
            
            
        }
        else
        {
            aud.PlayOneShot(hitSound);
            StopAllCoroutines();

            anim.SetTrigger("Damage");
            destinationChosen = false;
            isShooting = false;

            StartCoroutine (flashRed());
            agent.SetDestination(gameManager.instance.player.transform.position);

            faceTarget();
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }

}
