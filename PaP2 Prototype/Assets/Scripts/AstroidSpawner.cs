using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroidSpawner : MonoBehaviour
{
    public GameObject astroid;
    private float timer = 0;
    private float spawnRate = 2;
    // Start is called before the first frame update
    void Start()
    {
        
}

    // Update is called once per frame
    void Update()
    {
       
        if (timer < spawnRate)
        {
            timer += Time.deltaTime;
        }
        else
        {
            spawnAstroid();
            timer = 0;
            spawnRate = Random.Range(1, 20);
        }
    }
    void spawnAstroid ()
    {
        Instantiate(astroid, transform.position, transform.rotation);
    }
}
