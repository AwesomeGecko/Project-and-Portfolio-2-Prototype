using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTrap : MonoBehaviour
{
    [SerializeField] int dmgAmount;
    [SerializeField] int burnAmt;
    [SerializeField] int burnOverTime;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController HP = other.GetComponent<PlayerController>();
            if (HP != null)
            {
                HP.takeDamage(dmgAmount);
                StartCoroutine(BurnOverTime(HP));
            }
        }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator BurnOverTime(PlayerController HP) // Burn Damage Method
    {
        float timer = 0f;
        while (timer < burnOverTime)
        {
            HP.takeDamage(burnAmt);
            yield return new WaitForSeconds(1f);
            timer += 1f;
        }
    }
}
