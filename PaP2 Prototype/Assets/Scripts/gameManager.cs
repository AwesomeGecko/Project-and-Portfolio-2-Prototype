using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("Menus")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject previousMenu;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuControls;

    [Header("Interactive UI")]
    [SerializeField] GameObject interactive;
    [SerializeField] TextMeshProUGUI interact_text;
    [SerializeField] GameObject maxPickup;
    [SerializeField] TextMeshProUGUI maxText;

    [Header("Player")]
    [SerializeField] GameObject playerSpawnPos;
    public GameObject player;
    public PlayerController playerScript;
    [SerializeField] GameObject damageScreen;
    private float intensity;
    private PostProcessVolume volume;
    Vignette vignette;

    [Header("UI")]
    [SerializeField] public Image playerHPBar;
    [SerializeField] public Image playerStaminaBar;
    [SerializeField] public TextMeshProUGUI ammoCounter;
    [SerializeField] public TextMeshProUGUI gunName;
    [SerializeField] public TextMeshProUGUI enemyCounter;

    [Header("Public bools")]
    public bool isPaused;
    float timeScaleOrig;
    int enemiesRemaining;
    public bool onTarget;

    // Start is called before the first frame update
    void Awake()
    {

        instance = this;
        onTarget = false;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        playerSpawnPos = GameObject.FindWithTag("PlayerSpawnPos");

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
    }

    public void statePause()
    {
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
            youWin();
        }
    }

    public void youWin()
    {
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public void youLose()
    {
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

                interact_text.text = "Pick up [E] " + interactable.GetItemName();
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
    public void updateMenu()
    {
        if (menuActive != null)
        {
            previousMenu = menuActive;
        }
    }

    public void backBttn()
    {
        menuActive.SetActive(false);
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

    //void spawnPlayer()
    //{
    //    Instantiate(player, playerSpawnPos.transform.position, transform.rotation);
    //}

    public void maxItems()
    {
        if (playerScript.ammoCounter >= playerScript.maxAmmo)
        {
            StartCoroutine(maxPickups());
            maxText.text = "Ammo Too Full";
            playerScript.ammoCounter = playerScript.maxAmmo;
        }
    }

    IEnumerator maxPickups()
    {
        menuActive = maxPickup;
        menuActive.SetActive(true);
        yield return new WaitForSeconds(1f);
        menuActive.SetActive(false);
    }
}
