using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class teleporterScript : MonoBehaviour
{
    [SerializeField] ParticleSystem particles;

    public bool playerInRange;
    public bool isTeleporterOn;
    public int keyCounter;

    private void Start()
    {
        particles.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        //if the teleporter is on teleport
        turnOnTeleporter();
        keyCounter = gameManager.instance.keysCollected;
    }

    void teleport()
    {
        SceneManager.LoadScene(2);
    }

    void turnOnTeleporter()
    {
        isTeleporterOn = gameManager.instance.isTPOn;
        if (isTeleporterOn)
        {
            particles.Play();
        }
        else if(!isTeleporterOn)
        {
            particles.Pause();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (keyCounter < 3)
            {
                gameManager.instance.maxText.text = "Missing Keys";
                gameManager.instance.runText();
            }
            if (gameManager.instance.enemiesRemaining > 0)
            {
                gameManager.instance.maxText.text = "Enemys remain!";
                gameManager.instance.runText();
            }
            if (isTeleporterOn && gameManager.instance.enemiesRemaining <= 0)
            { 
                playerInRange = true;
                teleport();
            }    
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
