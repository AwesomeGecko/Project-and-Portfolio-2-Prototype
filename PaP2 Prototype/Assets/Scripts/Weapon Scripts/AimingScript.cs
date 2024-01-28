using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingScript : MonoBehaviour
{
    public GameObject gunHolder;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            PlayAnimationWithSpeed("ADS", 0.5f);
        }

        if (Input.GetMouseButtonDown(1))
        {
            gunHolder.GetComponent<Animator>().Play("New State");
        }
    }

    void PlayAnimationWithSpeed(string ads, float speed)
    {
        Animator animator = gunHolder.GetComponent<Animator>();
        if(animator!= null)
        {
            animator.speed = speed * Time.deltaTime;
            animator.Play(ads);
            animator.speed = 1.0f;
        }
    }
}
