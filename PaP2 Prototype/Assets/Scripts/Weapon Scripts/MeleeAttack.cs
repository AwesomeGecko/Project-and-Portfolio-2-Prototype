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
        if(Input.GetButtonDown("Melee"))
        {
            anim.Play("MeleeAttack");
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
        anim.Play("New State");
    }
}
