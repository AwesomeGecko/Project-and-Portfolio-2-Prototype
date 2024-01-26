using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] int dmgAmount;
    [SerializeField] float moveSpeed;
    [SerializeField] float maxHeight;
    [SerializeField] float minHeight;
    [SerializeField] int bleedAmt;
    [SerializeField] int bleedOverTime;

    private bool retract = true;

    private void Start()
    {
        maxHeight += transform.position.y;
        minHeight += transform.position.y;
    }
    private void Update()
    {
        TrapActive();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController HP = other.GetComponent<PlayerController>();
            if (HP != null)
            {
                HP.takeDamage(dmgAmount);
                StartCoroutine(BleedOverTime(HP));
                
            }
        }
    }

    private void TrapActive()
    {
        float newHeight = retract ? maxHeight : minHeight;
        float calculateHeight = Mathf.MoveTowards(transform.position.y, newHeight, moveSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, calculateHeight, transform.position.z);
        if(Mathf.Approximately(calculateHeight, newHeight))
        {
            retract = !retract;
        }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator BleedOverTime(PlayerController HP) // Bleed Damage Method
    {
        float timer = 0f;
        while(timer < bleedOverTime)
        {
            HP.takeDamage(bleedAmt);
            yield return new WaitForSeconds(1f);
            timer += 1f;
        }
    }
}
