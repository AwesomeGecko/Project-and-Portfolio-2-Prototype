using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPath : MonoBehaviour
{
   public Transform GetPlatformPath(int CurrPlaformIndex)
    {
        return transform.GetChild(CurrPlaformIndex);
    }
    public int GetNextPlatformIndex(int CurrPlaformIndex)
    {
        int newPlatformIndex = CurrPlaformIndex + 1;

        if (newPlatformIndex == transform.childCount) {
            newPlatformIndex = 0;
        }
        return newPlatformIndex;
    }
}
