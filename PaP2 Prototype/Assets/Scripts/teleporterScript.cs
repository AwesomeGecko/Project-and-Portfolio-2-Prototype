using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class teleporterScript : MonoBehaviour
{
    [SerializeField] ParticleSystem particles;

    [Header("Audio")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip idleTeleport;
    [SerializeField] AudioClip changeScene;

    public bool TeleportToSpawn;
    public bool playerInRange;
    public bool isTeleporterOn;
    public int keyCounter;
    private DataPersistenceManager dataPersistenceManager;
    public string sceneName;

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
        // CR
        aud.clip = changeScene;
        aud.loop = false;
        aud.Play();
        StartCoroutine(LoadSceneAfterSFX(changeScene.length));
        SceneManager.LoadScene(sceneName);
    }


    void turnOnTeleporter()
    {
        isTeleporterOn = gameManager.instance.isTPOn;
        if (isTeleporterOn)
        {
            particles.Play();
            // CR
            if(!aud.isPlaying)
            {
                aud.clip = idleTeleport;
                aud.loop = true;
                aud.Play();
            }
        }
        else if(!isTeleporterOn)
        {
            particles.Pause();
            // CR
            aud.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (TeleportToSpawn)
            {
                gameManager.instance.playerScript.teleportToSpawn();
            }
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
            if (isTeleporterOn && gameManager.instance.enemiesRemaining <= 0 && !TeleportToSpawn)
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

    private IEnumerator LoadSceneAfterSFX(float delayAmt)
    {
        yield return new WaitForSeconds(delayAmt);
        SceneManager.LoadScene(sceneName);
    }
    
    public void SetVolume(float volume)
    {
        if (aud != null)
        {
            aud.volume = volume;
        }
    }
    
}
