using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : MonoBehaviour, IDamage
{
    [Header("----- Enemy Stat -----")]
    [SerializeField] int HP;
    [SerializeField] int viewCone;
    [SerializeField] int speed;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [SerializeField] float animSpeedTrans;
    [SerializeField] int targetFaceSpeed;
    [SerializeField] float attackRate;

    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform headPos;
    [SerializeField] Animator anim;
    [SerializeField] Renderer model;
    [SerializeField] AudioSource aud;
    [SerializeField] Collider damageCol;
    [SerializeField] Collider Claw;

    [Header("----- Audio -----")]
    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip deathSound;
    [Range(0f, 1f)][SerializeField] float hitSoundVol;
    [Range(0f, 1f)][SerializeField] float deathSoundVol;


    bool isAttacking;
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
            //agent.stoppingDistance = 0;
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

                if (!isAttacking)
                {
                    StartCoroutine(Attack());
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

    public IEnumerator Attack()
    {
        isAttacking = true;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(attackRate);
        isAttacking = false;
    }

    public void takeDamage(int amount)
    {
        if (isDead)
            return;

        HP -= amount;

        //StopAllCoroutines();

        if (HP <= 0)
        {
            isDead = true;
            mySpawner.heyIDied();
            aud.PlayOneShot(deathSound, deathSoundVol);
            gameManager.instance.updateGameGoal(-1);
            anim.SetBool("Dead", true);
            agent.enabled = false;
            damageCol.enabled = false;
            Claw.enabled = false;
        }

        else
        {        

            aud.PlayOneShot(hitSound, hitSoundVol);            
            destinationChosen = false;
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
            isAttacking = false;
        }
    }
}
