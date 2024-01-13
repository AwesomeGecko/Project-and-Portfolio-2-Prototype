using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTrap : MonoBehaviour
{
    [SerializeField] List<GameObject> trapList = new List<GameObject>();
    
    private void OnTriggerEnter(Collider other)
    {
        TurnOffTraps();
    }

    private void TurnOffTraps()
    {
        foreach(GameObject trap in trapList)
        {
            SpikeTrap spike = trap.GetComponent<SpikeTrap>();
            if(spike != null)
            {
                spike.Deactivate();
            }
            LaserTrap laser = trap.GetComponent<LaserTrap>();
            if(laser != null)
            {
                laser.Deactivate();
            }
        }
    }
}
