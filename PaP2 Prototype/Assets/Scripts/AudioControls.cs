using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControls : MonoBehaviour
{
    [SerializeField] AudioSource music;
    [SerializeField] AudioSource soundEffects;

    private void Start()
    {
        music = gameManager.instance.cameraObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
