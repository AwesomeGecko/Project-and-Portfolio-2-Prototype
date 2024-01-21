using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretGenerator : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] int waitTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            StartCoroutine(TurnOffTur());
        }
    }
    IEnumerator TurnOffTur()
    {
        Turret.setSwitch(true);
        



        yield return new WaitForSeconds(waitTime);

        Turret.setSwitch(false);

       





    }

}
