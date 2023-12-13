using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingScript : MonoBehaviour
{
    public GameObject gunModel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            gunModel.GetComponent<Animator>().Play("ADS");
        }

        if(Input.GetMouseButtonDown(1))
        {
            gunModel.GetComponent<Animator>().Play("New State");
        }
    }
}
