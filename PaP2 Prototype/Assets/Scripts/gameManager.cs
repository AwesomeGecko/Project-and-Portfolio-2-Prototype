using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("Menus")]
    [SerializeField] GameObject menuActive;
    private GameObject previousMenu;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuControls;

    [Header("Interactive")]
    [SerializeField] GameObject interactive;
    [SerializeField] TextMeshProUGUI interact_text;

    [Header("Player")]
    public GameObject player;

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

                interact_text.text = "Press E to pick up " + interactable.GetItemName();
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
    
}
