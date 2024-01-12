using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] int damageAmount;
    [SerializeField] float moveSpeed = 1.0f;
    [SerializeField] float maxHeight = 2.0f;
    [SerializeField] float minHeight = 0.5f;

    private bool retract = true;

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
                HP.takeDamage(damageAmount);

                // Disables the Trap
                // gameObject.SetActive(false);
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
}
