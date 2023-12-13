using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lights : MonoBehaviour
{
    public float Oncount;
    public float Offcount;
    Light light;

    float count;

    
    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
       
        count = Offcount;
      
    }

    // Update is called once per frame
    void Update()
    {
        if (count <= 0 && !light.enabled)
        {
            light.enabled = true;
            count = Offcount;
            
        }
        else if (count <= 0 && light.enabled)
        {
            light.enabled = false;
            count = Oncount;
           
        }
        count -= Time.deltaTime;

    }
}
