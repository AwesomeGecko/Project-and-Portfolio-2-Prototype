using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponBase : MonoBehaviour
{
    public Transform pistolShootPos;
    [SerializeField] GameObject bullet;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public virtual void Shoot()
    {
        Debug.Log("pulling shoot from pistol");
        Instantiate(bullet, pistolShootPos.position, transform.rotation);
    }

}
