using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [SerializeField] GameObject meleeHitbox;

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if(!gameManager.instance.playerGunControls.isAiming)
        {
            if (Input.GetButtonDown("Melee"))
            {
                gameManager.instance.isMelee = true;
                anim.speed = 1.5f;
                anim.Play("MeleeAttack");
            }
        }

        
    }

    public void ColliderOn()
    {
        meleeHitbox.SetActive(true);
    }

    public void ColliderOff()
    {
        meleeHitbox.SetActive(false);
    }

    public void SetToIdle()
    {
        anim.Play("Idle");
        gameManager.instance.isMelee = false;
    }
}
