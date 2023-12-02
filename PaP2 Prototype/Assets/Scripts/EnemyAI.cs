using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Rigidbody rb;
    [SerializeField] int HP;

    [SerializeField] Renderer model;

    [SerializeField] GameObject player;
    [SerializeField] Transform playerPos;
    [SerializeField] int speed;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] float shootSpeed;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform shootPos2;
    bool isShooting;
    bool PlayerInRange;

    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.updateGameGoal(1);


    }

    

    // Update is called once per frame
    void Update()
    {

        

        if (PlayerInRange )
        {
            transform.rotation = Quaternion.LookRotation(playerPos.position - transform.position, transform.up);

            agent.SetDestination(gameManager.instance.player.transform.position);
            if (!isShooting )
            {
                StartCoroutine(shoot());
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
    IEnumerator shoot()
    {
        isShooting = true;

        var enemybullet = Instantiate(bullet, shootPos.position, transform.rotation);
        var enemybullet2 = Instantiate(bullet, shootPos2.position, transform.rotation);

        


        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void takeDamage(int amount)
    {
            HP -= amount;

            StartCoroutine(flashRed());

            if (HP <= 0)
            {
                gameManager.instance.updateGameGoal(-1);
                Destroy(gameObject);
            }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }

}
