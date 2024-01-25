using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretGenerator : MonoBehaviour
{
    [SerializeField] public GameObject power;
    [SerializeField]  public  Collider col;
    [SerializeField] int waitTime;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("PlayerBullet"))
        {
            StartCoroutine(TurnOffTur());
        }
    }
    IEnumerator TurnOffTur()
    {
        power.SetActive(false);
        col.enabled = false;
        



        yield return new WaitForSeconds(waitTime);

        col.enabled = true;

        power.SetActive(true);





    }

}
