using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP;

    [SerializeField] Renderer model;

    [SerializeField] GameObject player;
    [SerializeField] int speed;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;

    bool isShooting;

    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {

        agent.SetDestination(gameManager.instance.player.transform.position);

        if (!isShooting )
        {
            StartCoroutine(shoot());    
        }

    }

    IEnumerator shoot()
    {
        isShooting = true;

        Instantiate(bullet, shootPos.position, transform.rotation);

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
