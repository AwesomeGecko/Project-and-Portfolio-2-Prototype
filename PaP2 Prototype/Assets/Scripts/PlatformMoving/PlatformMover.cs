using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlatformMover : MonoBehaviour
{

   [SerializeField] private PlatformPath platformPath;
    [SerializeField] private float speed;
    private int platformIndex;

    private Transform startPath;
    private Transform endPath;

   
    private float timer;
    private float timeLeft;

    // Start is called before the first frame update
    void Start()
    {
        NextPath();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        float currPercentageOfProgress = timer / timeLeft;
        currPercentageOfProgress = Mathf.SmoothStep(0, 1, currPercentageOfProgress);
        transform.position = Vector3.Lerp(startPath.position, endPath.position, currPercentageOfProgress);
        transform.rotation = Quaternion.Lerp(startPath.rotation, endPath.rotation, currPercentageOfProgress);

        if (currPercentageOfProgress >= 1)
        {
            
            NextPath();
        }
    }

    

    private void NextPath()
    {
        startPath = platformPath.GetPlatformPath(platformIndex);
        platformIndex = platformPath.GetNextPlatformIndex(platformIndex);
        endPath = platformPath.GetPlatformPath(platformIndex);

        timer = 0;

        float distanceleft = Vector3.Distance(startPath.position, endPath.position);
        timeLeft = distanceleft / speed;
    }
}
