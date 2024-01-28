using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lights : MonoBehaviour
{
    [SerializeField] Renderer ren;
    [SerializeField] Material shutoffmat;
    public float Oncount;
    public float Offcount;
    new Light light;

   [SerializeField] bool IfObjectiveLight;
    bool completelyOff;

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
        if (!completelyOff)
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

    private void OnTriggerEnter(Collider other)
    {
        if (IfObjectiveLight)
        {
            if(other.CompareTag("Player"))
            {
                Material[] m = ren.materials;
                m[2] = shutoffmat;
                ren.materials = m;
                completelyOff = true;
                light.enabled = false;
            }
        }
    }
}
