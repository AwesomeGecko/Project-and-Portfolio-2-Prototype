using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossEnemyAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Rigidbody rb;
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
    [SerializeField] GameObject EnemyBullet;
    [SerializeField] int bulletDamage;
    [SerializeField] int bulletDestroyTime;
    [SerializeField] int bulletSpeed;
    [SerializeField] float shootRate;
    [SerializeField] float shootSpeed;
    public Transform enemyshootPos;

    [Header("----- Audio -----")]
    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip deathSound;
    [Range(0f, 1f)][SerializeField] float hitSoundVol;
    [Range(0f, 1f)][SerializeField] float deathSoundVol;

    bool isShooting;
    bool PlayerInRange;
    bool destinationChosen;
    float angleToPlayer;
    Vector3 playerDir;
    Vector3 startingPos;
    float stoppingDistanceOrig;
    private bool isDead = false;
    private bool TookDmg;
    private bool ChasingPlayer;
    public float notifyRadius = 25f;
    private int enemyCount;
    void Start()
    {
        enemyCount++;
        agent = GetComponent<NavMeshAgent>();

        startingPos = transform.position;
        stoppingDistanceOrig = agent.stoppingDistance;

        float animationSpeed = agent.velocity.normalized.magnitude;
        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), animationSpeed, Time.deltaTime * animSpeedTrans));

        gameManager.instance.updateGameGoal(enemyCount);
    }

    void Update()
    {
        if (gameManager.instance.playerScript.isDead)
        {
            
            PlayerInRange = false;
            agent.stoppingDistance = 0;
            isShooting = false;
            TookDmg = false;
            ChasingPlayer = false;
        }
        if (agent.isActiveAndEnabled)
        {
            float animationSpeed = agent.velocity.normalized.magnitude;
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), animationSpeed, Time.deltaTime * animSpeedTrans));

            if (PlayerInRange && !canSeePlayer() && !SearchforPlayer())
            {
                StartCoroutine(roam());
            }
            if (TookDmg && ChasingPlayer && !gameManager.instance.playerScript.isDead)
            {

                StartCoroutine(PlayerLocation());

            }
            else if (!PlayerInRange)
            {
                StartCoroutine(roam());
            }
        }
    }

    // If the enemy took damage and it does not see the player it looks for the player
    bool SearchforPlayer()
    {
        if (TookDmg)
        {

            agent.SetDestination(gameManager.instance.player.transform.position);
            return true;
        }

        return false;
    }


    IEnumerator PlayerLocation()
    {
        float SearchTime = 0f;
        while (SearchTime < 8f)
        {
            SearchforPlayer();
            SearchTime += Time.deltaTime;
            yield return null;
        }
        TookDmg = false;
        ChasingPlayer = false;
    }

    IEnumerator AttackLocation()
    {
        float SearchTime = 0f;
        while (SearchTime < 8f)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);
            SearchTime += Time.deltaTime;
            yield return null;
        }
        TookDmg = false;
        ChasingPlayer = false;
    }

    void NotifyNearbyEnemies()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, notifyRadius);

        foreach (var collider in nearbyColliders)
        {
            if (collider.CompareTag("Enemy") && collider.gameObject != gameObject)
            {
                BossEnemyAI nearbyEnemy = collider.GetComponent<BossEnemyAI>();
                if (nearbyEnemy != null)
                {
                    nearbyEnemy.StartChasing();
                }
            }
        }
    }

    public void StartChasing()
    {
        StopCoroutine(roam());
        ChasingPlayer = true;
        if (!PlayerInRange || PlayerInRange)
        {
            StartCoroutine(AttackLocation());
        }
    }

    IEnumerator roam()
    {
        if (agent.remainingDistance < 0.5 && !destinationChosen)
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

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * targetFaceSpeed);
    }

    bool canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;

        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(headPos.position, playerDir);

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


                agent.stoppingDistance = stoppingDistanceOrig;

                return true;
            }
        }
        agent.stoppingDistance = 0;
        return false;
    }





    IEnumerator shoot()
    {
        isShooting = true;
        anim.SetTrigger("Shoot");
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
    public void CreateBullet()
    {
        GameObject newBullet = Instantiate(EnemyBullet, enemyshootPos.position, transform.rotation);
        EnemyBullet enemyBullet = newBullet.GetComponent<EnemyBullet>();
        enemyBullet.SetBulletProperties(bulletDamage, bulletDestroyTime, bulletSpeed);

    }

    public void takeDamage(int amount)
    {
        if (isDead)
            return;

        HP -= amount;

        StopAllCoroutines();

        if (HP <= 0)
        {
            isDead = true;
            enemyCount--;
            aud.PlayOneShot(deathSound, deathSoundVol);
            gameManager.instance.updateGameGoal(-1);
            anim.SetBool("Dead", true);
            agent.enabled = false;
            damageCol.enabled = false;
        }

        else
        {
            TookDmg = true;
            ChasingPlayer = true;
            isShooting = false;
            aud.PlayOneShot(hitSound, hitSoundVol);
            destinationChosen = false;
            agent.SetDestination(gameManager.instance.player.transform.position);
            NotifyNearbyEnemies();
        }
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
            isShooting = false;
        }
    }
}
