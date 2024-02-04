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
    [SerializeField] GameObject Shield;


    [Header("----- Enemy Stat -----")]
    [SerializeField] int HP;
    [SerializeField] int viewCone;   
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [SerializeField] float animSpeedTrans;
    [SerializeField] int targetFaceSpeed;

    [Header("----- Weapon -----")]
    [SerializeField] GameObject EnemyBullet;
    [SerializeField] int CanonbulletDamage;
    [SerializeField] int CanonbulletDestroyTime;
    [SerializeField] int CanonbulletSpeed;
    [SerializeField] int SmallcanonbulletDamage;
    [SerializeField] int SmallcanonbulletDestroyTime;
    [SerializeField] int SmallcanonbulletSpeed;
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform shootPos2;
    [SerializeField] Transform shootPos3;
    [SerializeField] Transform shootPos4;
    [SerializeField] Transform shootPos5;
    [SerializeField] Transform shootPos6;
    [SerializeField] Transform shootPos7;
    [SerializeField] Transform shootPos8;

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
    public BossSpawner mySpawner;
    public int startingHP;
    private bool hasReturnedToSpawn75;
    private bool hasReturnedToSpawn50;
    private bool hasReturnedToSpawn25;
    private bool isDead = false;
    private int enemyCount;


    [Header("----- Enemy Lists -----")]
    [SerializeField] GameObject[] initialEnemies;
    [SerializeField] GameObject[] enemiesAt75Percent;
    [SerializeField] GameObject[] enemiesAt50Percent;
    [SerializeField] GameObject[] enemiesAt25Percent;
    // Start is called before the first frame update
    void Start()
    {
        //gameManager.instance.updateGameGoal(1);
        enemyCount++;
        startingHP = HP;
        startingPos = transform.position;
        stoppingDistanceOrig = agent.stoppingDistance;
        gameManager.instance.updateGameGoal(enemyCount);
        ActivateEnemies(initialEnemies);
    }



    // Update is called once per frame
    void Update()
    {
        if (agent.isActiveAndEnabled)
        {
            float animspeed = agent.velocity.normalized.magnitude;
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), animspeed, Time.deltaTime * animSpeedTrans));

            if (PlayerInRange && !CanSeePLayer() && !isReturningToSpawn)
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
        // Disable damage and shooting
        anim.SetTrigger("StopShoot");
        PlayerInRange = false;
        damageCol.enabled = false;
        Shield.SetActive(true);

        isReturningToSpawn = true;




        // Move to spawn position
        agent.SetDestination(startingPos);

        // Wait for boss to reach spawn position
        yield return new WaitUntil(() => Vector3.Distance(transform.position, startingPos) < 0.5f);

        

        // Wait for 5 seconds
        yield return new WaitForSeconds(15f);

        isReturningToSpawn = false;        
        Shield.SetActive(false);
        damageCol.enabled = true;
              
        anim.SetTrigger("Shoot");
        agent.SetDestination(gameManager.instance.player.transform.position);


    }

    void HandleHealthThreshold(float threshold)
    {
        if (threshold == 0.75f && !hasReturnedToSpawn75)
        {
            
            StartCoroutine(ReturnToSpawnAndRecover());
            hasReturnedToSpawn75 = true;
            ActivateEnemies(enemiesAt75Percent);
        }
        else if (threshold == 0.5f && !hasReturnedToSpawn50)
        {
            
            StartCoroutine(ReturnToSpawnAndRecover());
            hasReturnedToSpawn50 = true;
            ActivateEnemies(enemiesAt50Percent);
        }
        else if (threshold == 0.25f && !hasReturnedToSpawn25)
        {
            
            StartCoroutine(ReturnToSpawnAndRecover());
            hasReturnedToSpawn25 = true;
            ActivateEnemies(enemiesAt25Percent);
        }    
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInRange = true;
            //Debug.Log("Player entered boss trigger zone.");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInRange = false;
            agent.stoppingDistance = 0;
            //Debug.Log("Player exited boss trigger zone.");
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
        if (!isReturningToSpawn)
        {
            playerDir = gameManager.instance.player.transform.position - headPos.position;
            angleToPlayer = Vector3.Angle(playerDir, transform.forward);
            playerDir.Normalize();

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

        }
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
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
        anim.SetTrigger("StopShoot");
    }

    public void CreateBullet()
    {
        //In the animation controller use the shoot animation of BigCanon_A
        GameObject newBullet1 = Instantiate(EnemyBullet, shootPos.position, transform.rotation);
        EnemyBullet enemyBullet1 = newBullet1.GetComponent<EnemyBullet>();
        enemyBullet1.SetBulletProperties(CanonbulletDamage, CanonbulletDestroyTime, CanonbulletSpeed);

        GameObject newBullet2 = Instantiate(EnemyBullet, shootPos2.position, transform.rotation);
        EnemyBullet enemyBullet2 = newBullet2.GetComponent<EnemyBullet>();
        enemyBullet2.SetBulletProperties(CanonbulletDamage, CanonbulletDestroyTime, CanonbulletSpeed);
    }
    public void CreateCanon_BBullet()
    {
        //In the animation controller use the shoot animation of BigCanon_B
        GameObject newBullet3 = Instantiate(EnemyBullet, shootPos3.position, transform.rotation);
        EnemyBullet enemyBullet3 = newBullet3.GetComponent<EnemyBullet>();
        enemyBullet3.SetBulletProperties(CanonbulletDamage, CanonbulletDestroyTime, CanonbulletSpeed);

        GameObject newBullet4 = Instantiate(EnemyBullet, shootPos4.position, transform.rotation);
        EnemyBullet enemyBullet4 = newBullet4.GetComponent<EnemyBullet>();
        enemyBullet4.SetBulletProperties(CanonbulletDamage, CanonbulletDestroyTime, CanonbulletSpeed);
    }
    public void CreateSmallCanon_ABullet()
    {
        //In the animation controller use the shoot animation of SmallCanon_A
        GameObject newBullet5 = Instantiate(EnemyBullet, shootPos5.position, transform.rotation);
        EnemyBullet enemyBullet5 = newBullet5.GetComponent<EnemyBullet>();
        enemyBullet5.SetBulletProperties(SmallcanonbulletDamage, SmallcanonbulletDestroyTime, SmallcanonbulletSpeed);

        GameObject newBullet6 = Instantiate(EnemyBullet, shootPos6.position, transform.rotation);
        EnemyBullet enemyBullet6 = newBullet6.GetComponent<EnemyBullet>();
        enemyBullet6.SetBulletProperties(SmallcanonbulletDamage, SmallcanonbulletDestroyTime, SmallcanonbulletSpeed);
    }
    public void CreateSmallCanon_BBullet()
    {
            //In the animation controller use the shoot animation of SmallCanon_B
            GameObject newBullet7 = Instantiate(EnemyBullet, shootPos7.position, transform.rotation);
        EnemyBullet enemyBullet7 = newBullet7.GetComponent<EnemyBullet>();
        enemyBullet7.SetBulletProperties(SmallcanonbulletDamage, SmallcanonbulletDestroyTime, SmallcanonbulletSpeed);

        GameObject newBullet8 = Instantiate(EnemyBullet, shootPos8.position, transform.rotation);
        EnemyBullet enemyBullet8 = newBullet8.GetComponent<EnemyBullet>();
        enemyBullet8.SetBulletProperties(SmallcanonbulletDamage, SmallcanonbulletDestroyTime, SmallcanonbulletSpeed);
    }

    public void takeDamage(int amount)
    {
        if (!isReturningToSpawn) // Check if the boss is not returning to spawn
        {
            if (isDead)
                return;
            HP -= amount;

            UpdateBossHP();

            //Debug.Log("Boss took damage. HP=" + HP);

            if (HP <= 0 && damageCol.enabled == true && agent.enabled == true)
            {
                isDead = true;
                enemyCount--;
                anim.SetTrigger("StopShoot");
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

            else
            {

                if (HP <= startingHP * 0.75f && HP > startingHP * 0.5f)
                {
                    normalDamage();
                    HandleHealthThreshold(0.75f);                    
                }
                else if (HP <= startingHP * 0.5f && HP > startingHP * 0.25f)
                {
                    normalDamage();
                    HandleHealthThreshold(0.5f);
                    
                }
                else if (HP <= startingHP * 0.25f && HP > 0)
                {
                    normalDamage();
                    HandleHealthThreshold(0.25f);                    
                }
                else
                {
                    normalDamage();
                }
            }
        }
    }  

    public void UpdateBossHP()
    {
        gameManager.instance.BossHPBar.fillAmount = (float)HP / startingHP;
    }

    void normalDamage()
    {
        //Debug.Log("NormalDamage triggered.");
        aud.PlayOneShot(hitSound);

        anim.SetTrigger("Damage");
        destinationChosen = false;

        StartCoroutine(flashRed());

        if (agent.isActiveAndEnabled)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);
        }

        faceTarget();
        //Debug.Log("Boss took damage");
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }

    void ActivateEnemies(GameObject[] enemyList)
    {
        foreach (GameObject enemy in enemyList)
        {
            enemy.SetActive(true);
        }
    }
}
