using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AstroidMover : MonoBehaviour
{

    private float speed;
    public float deadZone = -45;
    [SerializeField] private Vector3 rotation;
    private float randomRotatSpeed;
    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(100, 700);
        randomRotatSpeed = Random.Range(0, 30);


    }

    // Update is called once per frame
    void Update()
    {
        
        transform.position += (Vector3.left * speed) * Time.deltaTime;
        transform.Rotate(rotation * randomRotatSpeed * Time.deltaTime);
        if (transform.position.x < deadZone)
        {
            Destroy(gameObject);
        }
        
    }
}
