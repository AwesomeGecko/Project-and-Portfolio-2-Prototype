using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretGenerator : MonoBehaviour
{
    [SerializeField] public GameObject turretGen;
    
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
        turretGen.SetActive(false);
        



        yield return new WaitForSeconds(waitTime);

        turretGen.SetActive(true);

       





    }

}
