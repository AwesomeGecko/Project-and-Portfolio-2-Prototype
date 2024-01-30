using System.Collections;
using System.Collections.Generic;
using System.Threading;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class EnemyAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Rigidbody rb;
    //[SerializeField] GameObject player;
    [SerializeField] Transform headPos;
    [SerializeField] Animator anim;
    [SerializeField] Renderer model;
    [SerializeField] AudioSource aud;
    [SerializeField] Collider damageCol;

    //[Header("----- Behavior -----")]
    //public LayerMask HidableLayers;
    //public EnemyLOS LOSChecker;
    //[Range(-1, 1)]
    //[Tooltip("Lower is a better Hiding spot")]
    //public float HideSensitivity = 0;

    //private Coroutine MovementCoroutine;
    //private Collider[] Colliders = new Collider[10];




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
    //public Transform enemyshootPos2;

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
    public enemySpawn mySpawner;
    private bool isDead = false;

    void Start()
    {
       agent = GetComponent<NavMeshAgent>();         

           startingPos = transform.position;
           stoppingDistanceOrig = agent.stoppingDistance;

        float animationSpeed = agent.velocity.normalized.magnitude;
        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), animationSpeed, Time.deltaTime * animSpeedTrans));


    }

    void Update()
    {
        if (agent.isActiveAndEnabled)
        {
            float animationSpeed = agent.velocity.normalized.magnitude;
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), animationSpeed, Time.deltaTime * animSpeedTrans));

            if (PlayerInRange && !canSeePlayer())
            {
                StartCoroutine(roam());
            }
            else if (!PlayerInRange)
            {
                StartCoroutine(roam());
            }



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
                //agent.SetDestination(gameManager.instance.player.transform.position);


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
            mySpawner.heyIDied();
            aud.PlayOneShot(deathSound, deathSoundVol);
            gameManager.instance.updateGameGoal(-1);
            anim.SetBool("Dead", true);
            agent.enabled = false;
            damageCol.enabled = false;
        }

        else
        {
            
            isShooting = false;
            aud.PlayOneShot(hitSound, hitSoundVol);
            anim.SetTrigger("Damage");
            destinationChosen = false;
            agent.SetDestination(gameManager.instance.player.transform.position);
            agent.stoppingDistance = stoppingDistanceOrig;
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
            //LOSChecker.GetComponentInChildren<Collider>().enabled = false;
        }
    }








    //    void Start()
    //    {
    //        agent = GetComponent<NavMeshAgent>();

    //        LOSChecker.OnGainSight += HandleGainSight;
    //        LOSChecker.OnLoseSight += HandleLoseSight;

    //        startingPos = transform.position;
    //        stoppingDistanceOrig = agent.stoppingDistance;
    //    }

    //    // Update is called once per frame
    //    void Update()
    //    {
    //        if (agent.isActiveAndEnabled)
    //        {
    //            float animationSpeed = agent.velocity.normalized.magnitude;
    //            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), animationSpeed, Time.deltaTime * animSpeedTrans));

    //            if (PlayerInRange && !canSeePlayer())
    //            {
    //                StartCoroutine(roam());
    //            }
    //            else if (!PlayerInRange)
    //            {
    //                StartCoroutine(roam());
    //            }
    //        }
    //    }

    //    IEnumerator roam()
    //    {
    //        if (agent.remainingDistance < 0.5 && !destinationChosen)
    //        {
    //            destinationChosen = true;
    //            agent.stoppingDistance = 0;
    //            yield return new WaitForSeconds(roamPauseTime);

    //            Vector3 randomPos = Random.insideUnitSphere * roamDist;
    //            randomPos += startingPos;

    //            NavMeshHit hit;
    //            NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
    //            agent.SetDestination(hit.position);

    //            destinationChosen = false;
    //        }
    //    }

    //bool canSeePlayer()
    //{
    //    playerDir = gameManager.instance.player.transform.position - headPos.position;

    //    angleToPlayer = Vector3.Angle(playerDir, transform.forward);

    //    Debug.DrawRay(headPos.position, playerDir);

    //    RaycastHit hit;

    //    if (Physics.Raycast(headPos.position, playerDir, out hit))
    //    {
    //        if (hit.collider.CompareTag("Player") && angleToPlayer <= viewCone)
    //        {
    //            //agent.SetDestination(gameManager.instance.player.transform.position);

    //            if (!isShooting)
    //            {
    //                StartCoroutine(shoot());
    //            }

    //            if (agent.remainingDistance < agent.stoppingDistance)
    //            {
    //                faceTarget();
    //            }

    //            agent.stoppingDistance = stoppingDistanceOrig;

    //            return true;
    //        }
    //    }
    //    agent.stoppingDistance = 0;
    //    return false;
    //}

    //    void faceTarget()
    //    {
    //        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
    //        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * targetFaceSpeed);
    //    }

    //    private void HandleGainSight(Transform Target)
    //    {
    //        if(MovementCoroutine != null)
    //        {
    //            StopCoroutine(roam());
    //        }

    //        MovementCoroutine = StartCoroutine(Hide(Target));
    //    }

    //    private void HandleLoseSight(Transform Target)
    //    {
    //        if (MovementCoroutine != null)
    //        {
    //            StopCoroutine(roam());
    //        }
    //    }

    //private IEnumerator Hide(Transform Target)
    //{
    //    if(CompareTag("Cover"))
    //    {
    //        while (true)
    //        {

    //            for (int i = 0; i < Colliders.Length; i++)
    //            {
    //                Colliders[i] = null;
    //            }


    //            int hits = Physics.OverlapSphereNonAlloc(agent.transform.position, LOSChecker.Collider.radius, Colliders, HidableLayers);



    //            System.Array.Sort(Colliders, ColliderArraySortComparer);

    //            for (int i = 0; i < hits; i++)
    //            {
    //                if (NavMesh.SamplePosition(Colliders[i].transform.position, out NavMeshHit hit, 4f, agent.areaMask))
    //                {
    //                    if (!NavMesh.FindClosestEdge(hit.position, out hit, agent.areaMask))
    //                    {
    //                        Debug.LogError($"Unable to find edge close to {hit.position}");
    //                    }

    //                    if (Vector3.Dot(hit.normal, (Target.position - hit.position).normalized) < HideSensitivity)
    //                    {
    //                        agent.SetDestination(hit.position);

    //                        faceTarget();
    //                        canSeePlayer();
    //                        break;
    //                    }

    //                    else
    //                    {   //Second check for hidable position
    //                        if (NavMesh.SamplePosition(Colliders[i].transform.position - (Target.position - hit.position) * 10, out NavMeshHit hit2, 2f, agent.areaMask))
    //                        {
    //                            if (!NavMesh.FindClosestEdge(hit.position, out hit2, agent.areaMask))
    //                            {
    //                                Debug.LogError($"Unable to find edge close to {hit2.position} (second attempt)");
    //                            }

    //                            if (Vector3.Dot(hit2.normal, (Target.position - hit2.position).normalized) < HideSensitivity)
    //                            {
    //                                agent.SetDestination(hit2.position);
    //                                break;
    //                            }
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    Debug.LogError($"Unable to find NavMesh near object {Colliders[i].name} at {Colliders[i].transform.position}");
    //                }
    //            }

    //            yield return null;
    //        }
    //    }

    //}


    //private int ColliderArraySortComparer(Collider A, Collider B)
    //{
    //    if (A == null && B != null)
    //    {
    //        return 1;
    //    }
    //    else if (A != null && B == null)
    //    {
    //        return -1;
    //    }
    //    else if (A == null && B == null)
    //    {
    //        return 1;
    //    }
    //    else
    //    {
    //        return Vector3.Distance(agent.transform.position, A.transform.position).CompareTo(Vector3.Distance(agent.transform.position, B.transform.position));
    //    }

    //}



    //    public void OnTriggerEnter(Collider other)
    //    {
    //        if (other.CompareTag("Player"))
    //        {
    //            PlayerInRange = true;
    //        }
    //    }

    //    public void OnTriggerExit(Collider other)
    //    {
    //        if (other.CompareTag("Player"))
    //        {
    //            PlayerInRange = false;
    //            agent.stoppingDistance = 0;
    //        }
    //    }

    //    IEnumerator shoot()
    //    {
    //        isShooting = true;
    //        anim.SetTrigger("Shoot");
    //        yield return new WaitForSeconds(shootRate);        
    //        isShooting = false;

    //    }
    //    public void CreateBullet()
    //    {
    //        GameObject newBullet = Instantiate(EnemyBullet, enemyshootPos.position, transform.rotation);
    //        EnemyBullet enemyBullet = newBullet.GetComponent<EnemyBullet>();
    //        enemyBullet.SetBulletProperties(bulletDamage, bulletDestroyTime, bulletSpeed);

    //    }


    //    public void takeDamage(int amount)
    //    {
    //        if (isDead)
    //            return;

    //        HP -= amount;

    //        StopAllCoroutines();

    //        if (HP <= 0)
    //        {
    //            isDead = true;
    //            mySpawner.heyIDied();
    //            aud.PlayOneShot(deathSound, deathSoundVol);
    //            gameManager.instance.updateGameGoal(-1);
    //            anim.SetBool("Dead", true);
    //            agent.enabled = false;
    //            damageCol.enabled = false;
    //        }

    //        else
    //        {
    //            isShooting = false;
    //            aud.PlayOneShot(hitSound, hitSoundVol);
    //            anim.SetTrigger("Damage");
    //            destinationChosen = false;
    //            StartCoroutine(flashRed());
    //            agent.SetDestination(gameManager.instance.player.transform.position);

    //        }
    //    }

    //    IEnumerator flashRed()
    //    {
    //        model.material.color = Color.red;
    //        yield return new WaitForSeconds(0.1f);
    //        model.material.color = Color.white;
    //    }
}
