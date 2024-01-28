using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameData
{
    [Header("player stats")]
    public Vector3 playerPosition;
    public int Health;
    public float Stamina;
    public float playerSpeed;
    public float jumpHeight;
    public float gravityValue;
    public float sprintSpeed;
    public float crouchSpeed;
    public float crouchDist;
    public float slideSpeed;
    public float leanDist;
    public float leanSpeed;
    public int ammo;
    public int maxAmmo;

    
    public int level;

    public Dictionary<string, bool> KeysCollected;

    //values defined here is default values
    //what the game starts with when there is no data
    public GameData()
    {
        playerPosition = Vector3.zero;
        level = 0;
        Health = 30;
        Stamina = 10;
        playerSpeed = 10;
        jumpHeight = 7.5f;
        gravityValue = -20f;
        sprintSpeed = 15f;
        crouchSpeed = 7.5f;
        crouchDist = 0.75f;
        slideSpeed = 12f;
        leanDist = 10f;
        leanSpeed = 0.5f;

        
    }
}
