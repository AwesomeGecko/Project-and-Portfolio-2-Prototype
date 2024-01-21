using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEditor;
using System.IO;

public class gameManager : MonoBehaviour, IDataPersistence
{
    public static gameManager instance;

    public bool DebugLogs;

    [Header("Menus")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject previousMenu;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuControls;
    [SerializeField] GameObject menuSettings;
    [SerializeField] GameObject quitWarning;
    [SerializeField] GameObject SavedDataDev;

    [Header("Interactive UI")]
    [SerializeField] public GameObject interactive;
    [SerializeField] public TextMeshProUGUI interact_text;
    [SerializeField] public GameObject maxPickup;
    [SerializeField] public TextMeshProUGUI maxText;

    [Header("Player")]
    [SerializeField] public GameObject Checkpoint_Alpha;
    [SerializeField] public GameObject playerSpawnPos;
    [SerializeField] public GameObject TeleportPos;
    public GameObject player;
    public GameObject cameraObject;
    public Vector3 currentEnemyPosition;
    [SerializeField] GameObject damageScreen;
    private float intensity;
    private PostProcessVolume volume;
    Vignette vignette;

    [Header("UI")]
    [SerializeField] public Image playerHPBar;
    [SerializeField] public Image playerStaminaBar;
    [SerializeField] public Image Scope;
    [SerializeField] public Image Crosshair;
    [SerializeField] public Image ShotgunSight;
    [SerializeField] public TextMeshProUGUI ammoCounter;
    [SerializeField] public TextMeshProUGUI maxAmmoCounter;
    [SerializeField] public TextMeshProUGUI gunName;
    [SerializeField] public TextMeshProUGUI enemyCounter;
    [SerializeField] public TextMeshProUGUI keysLeft;
    [SerializeField] public TextMeshProUGUI SavedDataText;

    [Header("Scripts")]
    public PlayerController playerScript;
    public CameraController cameraScript;


    [Header("Public variables")]
    public bool isPaused;
    public float timeScaleOrig;
    public int enemiesRemaining;
    public bool onTarget;
    public bool isAmmo;
    public bool isHP;
    public bool isTPOn;
    public int keysCollected;
    string sceneName;
    Scene currentScene;
    public bool isMuted;

    public bool isDev;
    public int keysRemain = 3;




    // Audio
    [Header("Audio")]
    [SerializeField] public AudioSource aud;
    public AudioClip winSound;
    [Range(0f, 1f)][SerializeField] float winSoundVol;
    public AudioClip loseSound;
    [Range(0f, 1f)][SerializeField] float loseSoundVol;
    public AudioClip pauseSound;
    [Range(0f, 1f)][SerializeField] float pauseSoundVol;
    // CR
    public AudioClip UIButtonForward;
    public AudioClip UIButtonBack;
    [Range(0f, 3f)][SerializeField] float buttonClickVolume;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        onTarget = false;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        cameraObject = GameObject.FindWithTag("MainCamera");
        playerScript = player.GetComponent<PlayerController>();
        cameraScript = cameraObject.GetComponent<CameraController>();
        playerSpawnPos = GameObject.FindWithTag("PlayerSpawnPos");
        TeleportPos = GameObject.FindWithTag("TeleportPos");
        Checkpoint_Alpha = GameObject.FindWithTag("Checkpoint_Alpha");


        damageScreen = GameObject.FindWithTag("DamageScreen");
        volume = damageScreen.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings<Vignette>(out vignette);
        if (!vignette)
        {
            //if the Vignette is not avalible or emty
            Debug.Log("error, empty vignette");
        }
        else
        {
            //turns off the Vignette by default
            vignette.enabled.Override(false);
        }

        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Cancel") && menuActive == null)
        {
            statePause();
            menuActive = menuPause;
            menuActive.SetActive(isPaused);
        }
        interact();

        if (isPaused)
        {
            vignette.enabled.Override(false);
        }
        enemyCounter.text = enemiesRemaining.ToString("0");
        keysLeft.text = keysCollected.ToString("0");


        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyUp(KeyCode.X))
            {
                DebugLogs = !DebugLogs;
            }
        }
    }


    public void statePause()
    {
        aud.PlayOneShot(pauseSound, pauseSoundVol);
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        //optional \/\/\/
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void updateGameGoal(int amount)
    {

        enemiesRemaining += amount;
        if (enemiesRemaining <= 0)
        {
            if (sceneName == "Boss Level")
            {
                youWin();
            }
            else if (sceneName == "Casey")
            {
                youWin();
            }
            else if (sceneName == "Samuel")
            {
                youWin();
            }
        }
    }

    public void youWin()
    {
        aud.PlayOneShot(winSound, winSoundVol);
        StartCoroutine(ShowMenuAfterDelay());
    }

    private IEnumerator ShowMenuAfterDelay()
    {
        yield return new WaitForSeconds(3);
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public void youLose()
    {
        
        aud.PlayOneShot(loseSound, loseSoundVol);
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void interact()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;

            interactableObject interactable = selectionTransform.GetComponent<interactableObject>();

            if (interactable && interactable.playerInRange)
            {
                onTarget = true;

                interact_text.text = "Pick up [F] " + interactable.GetItemName();
                interactive.SetActive(true);

            }
            else
            {
                onTarget = false;
                interactive.SetActive(false);
            }

        }
        else
        {
            onTarget = false;
            interactive.SetActive(false);

        }
    }


    public void openControls()
    {
        updateMenu();
        menuActive.SetActive(false);
        menuActive = menuControls;
        menuActive.SetActive(true);
    }

    public void quitMenu()
    {
        updateMenu();
        menuActive.SetActive(false);
        menuActive = quitWarning;
        menuActive.SetActive(true);
    }

    public void openSettings()
    {
        updateMenu();
        menuActive.SetActive(false);
        menuActive = menuSettings;
        menuActive.SetActive(true);
    }

    public void openSavedScreen()
    {
        updateMenu();
        menuActive.SetActive(false);
        menuActive = SavedDataDev;
        menuActive.SetActive(true);
    }

    public void updateMenu()
    {
        if (menuActive != null)
        {
            // CR
            aud.PlayOneShot(UIButtonForward, buttonClickVolume);
            previousMenu = menuActive;
        }
    }

    public void backBttn()
    {
        menuActive.SetActive(false);
        aud.PlayOneShot(UIButtonBack, buttonClickVolume);
        menuActive = previousMenu;
        menuActive.SetActive(true);
    }

    public void damageIndicator()
    {
        StartCoroutine(TakeDamageEffect());
    }

    IEnumerator TakeDamageEffect()
    {
        intensity = 0.4f;

        //Turns on Vignette
        vignette.enabled.Override(true);
        //Sets intensity
        vignette.intensity.Override(0.4f);
        yield return new WaitForSeconds(0.4f);


        //waites for the intensity to go back to 0
        while (intensity > 0)
        {
            intensity -= 0.01f;

            //once the intensity goes below 0
            if (intensity < 0)
            {
                intensity = 0;
            }
            //Vignette intensity is updated
            vignette.intensity.Override(intensity);
            yield return new WaitForSeconds(0.01f);
        }

        //once the intinsity is at 0 it turns off the Vignette
        vignette.enabled.Override(false);
        yield break;
    }


    public void maxItems()
    {
        //refrencing players current gun ammo and mag size
        if (playerScript.gunList[playerScript.selectedGun].totalAmmo >= playerScript.gunList[playerScript.selectedGun].magSize && isAmmo)
        {
            StartCoroutine(maxPickups());
            maxText.text = "Ammo Too Full";
        }
        if (playerScript.HP >= playerScript.HPOriginal && isHP)
        {
            StartCoroutine(maxPickups());
            maxText.text = "Health Too Full";
        }
    }

    public void runText()
    {
        StartCoroutine(maxPickups());
    }

    IEnumerator maxPickups()
    {
        maxPickup.SetActive(true);
        yield return new WaitForSeconds(2f);
        maxPickup.SetActive(false);
    }

    public void LoadData(GameData data)
    {

    }

    public void SaveData(GameData data)
    {

    }
}
