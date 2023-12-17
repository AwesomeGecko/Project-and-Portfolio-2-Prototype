using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossScript : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject player;
    [SerializeField] Transform headPos;
    [SerializeField] Animator anim;
    [SerializeField] Renderer model;

    [SerializeField] AudioSource aud;
    [SerializeField] Collider damageCol;


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

    [Header("----- Boss State -----")]
    
    bool isReturningToSpawn;
    

    bool isShooting;
    bool PlayerInRange;
    bool destinationChosen;
    float angleToPlayer;
    Vector3 playerDir;
    Vector3 startingPos;
    float stoppingDistanceOrig;
    public enemySpawn mySpawner;
    public int startingHP;


    // Start is called before the first frame update
    void Start()
    {
        //gameManager.instance.updateGameGoal(1);
        startingHP = HP;
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
            else if (!PlayerInRange && !isReturningToSpawn) // Add condition to check if not returning to spawn
            {
                StartCoroutine(roam());
            }
                        
        }
    }

    IEnumerator ReturnToSpawnAndRecover()
    {
        Debug.Log("Boss is returning to spawn");
        isReturningToSpawn = true;

        // Disable damage and shooting
        PlayerInRange = false;
        damageCol.enabled = false;

        Debug.Log("Initial Position: " + startingPos);

        // Move to spawn position
        agent.SetDestination(startingPos);

        // Wait for boss to reach spawn position
        yield return new WaitUntil(() => Vector3.Distance(transform.position, startingPos) < 0.5f);

        

        // Wait for 5 seconds
        yield return new WaitForSeconds(5f);

        // Re-engage the player
        isReturningToSpawn = false;
        PlayerInRange = true;
        agent.SetDestination(gameManager.instance.player.transform.position);

        // Enable damage and shooting
        damageCol.enabled = true;       
    }

    void HandleHealthThreshold(float threshold)
    {
        StartCoroutine(ReturnToSpawnAndRecover());
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
            agent.stoppingDistance = 0;
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
            agent.stoppingDistance = stoppingDistanceOrig;

            return true;
        }

        agent.stoppingDistance = 0;

        return false;
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
        if (!isReturningToSpawn) // Check if the boss is not returning to spawn
        {
            HP -= amount;

            Debug.Log("Boss took damage. HP=" + HP);

            if (HP <= 0 && damageCol.enabled == true && agent.enabled == true)
            {
                mySpawner.heyIDied();

                if (isShooting)
                {
                    StopCoroutine(shoot());
                }

                aud.PlayOneShot(deathSound);

                gameManager.instance.updateGameGoal(-1);
                anim.SetBool("Dead", true);

                agent.enabled = false;
                damageCol.enabled = false;
            }

            if (HP <= startingHP * 0.75f && HP > startingHP * 0.5f)
            {
                HandleHealthThreshold(0.75f);
            }
            else if (HP <= startingHP * 0.5f && HP > startingHP * 0.25f)
            {
                HandleHealthThreshold(0.5f);
            }
            else if (HP <= startingHP * 0.25f && HP > 0)
            {
                HandleHealthThreshold(0.25f);
            }
            

            else
            {
                aud.PlayOneShot(hitSound);

                anim.SetTrigger("Damage");
                destinationChosen = false;

                StartCoroutine(flashRed());

                if (agent.isActiveAndEnabled)
                {
                    agent.SetDestination(gameManager.instance.player.transform.position);
                }

                faceTarget();
                Debug.Log("Boss took damage");
            }
        }
    }



    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }

    
}
