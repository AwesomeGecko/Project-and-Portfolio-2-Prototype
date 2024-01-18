using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int KeyCount;

    //values defined here is default values
    //what the game starts with when there is no data
    public GameData()
    { 
        this.KeyCount = 0;
    }
}
