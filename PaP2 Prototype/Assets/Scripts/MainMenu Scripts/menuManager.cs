using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using UnityEngine.UI;

public class menuManager : MonoBehaviour
{
    public static menuManager instance;

    

    [Header("Menus")]
    [SerializeField] public GameObject menuActive;
    [SerializeField] GameObject menuPrevious;
    [SerializeField] public GameObject menuMain;
    [SerializeField] GameObject menuBackStory;
    [SerializeField] GameObject menuControls;
    [SerializeField] GameObject menuSettings;
    [SerializeField] public GameObject menuCredits;
    [SerializeField] GameObject menuExit;
    [SerializeField] GameObject menuLoadNew;

    [SerializeField] private Button LoadGameButton;
    [SerializeField] TextMeshProUGUI LoadGameText;
    [SerializeField] private Button NewGameButton;
    [SerializeField] TextMeshProUGUI NewGameText;

    [Header("Credits Info")]
    [SerializeField] Animator credits;
    [SerializeField] public bool isCreditsOpen;

    // Start is called before the first frame update
    void Start()
    {
        if (!DataPersistenceManager.instance.HasGameData())
        { 
            LoadGameButton.interactable = false;
            LoadGameText.alpha = 0.5f;
        }

        if (!isCreditsOpen)
        {
            menuActive = menuMain;
            menuActive.SetActive(true);
        }
        else 
        {
            menuActive = menuCredits;
            menuActive.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (menuActive == menuCredits)
        {
            spaceBarPressed();
            escapePresed();
        }

        
    }

    public void MainMenu()
    {
        menuActive.SetActive(false);
        menuActive = menuMain;
        menuActive.SetActive(true);
    }

    public void backStory()
    {
        menuActive.SetActive(false);
        menuActive = menuBackStory;
        menuActive.SetActive(true);
    }

    public void OnPlayButton()
    {
        SceneManager.LoadScene(1);
    }


    public void OnNewGameClicked()
    {
        DisableMenuButtons();
        //Creates new game
        DataPersistenceManager.instance.NewGame();
        //SceneManager.LoadSceneAsync(1);
    }
    public void OnLoadGameClicked() 
    {
        DisableMenuButtons();
        //SceneManager.LoadSceneAsync(1);
    }

    private void DisableMenuButtons()
    { 
        LoadGameButton.interactable = false;
        NewGameButton.interactable = false;
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    public void updateMenu()
    {
        if (menuActive != null)
        {
            menuPrevious = menuActive;
        }
    }

    public void openControls()
    {
        updateMenu();
        menuActive.SetActive(false);
        menuActive = menuControls;
        menuActive.SetActive(true);
    }

    public void openSettings()
    {
        updateMenu();
        menuActive.SetActive(false);
        menuActive = menuSettings;
        menuActive.SetActive(true);
    }

    public void openCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void openExitMenu()
    {
        updateMenu();
        menuActive.SetActive(false);
        menuActive = menuExit;
        menuActive.SetActive(true);
    }

    public void openLoadNew()
    {
        updateMenu();
        menuActive.SetActive(false);
        menuActive = menuLoadNew;
        menuActive.SetActive(true);
    }

    public void backBttn()
    {
        menuActive.SetActive(false);
        menuActive = menuPrevious;
        menuActive.SetActive(true);
    }

    public void spaceBarPressed()
    {
        if (Input.GetButton("Jump"))
        {
            credits.speed = 2.5f;
        }
        else
        {
            credits.speed = 0.8f;
        }
    }

    public void escapePresed()
    {
        MainMenu();
    }

    //public void StartCredits()
    //{
    //    StartCoroutine(CreditsMenuOpen());
    //}

    //private IEnumerator CreditsMenuOpen()
    //{
    //    Debug.Log("start credits");
    //    yield return new WaitForSecondsRealtime(42f);
    //    MainMenu();
    //    isCreditsOpen = false;
    //    Debug.Log("end credits");
    //}
}
