using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using UnityEngine.UI;
using TransitionsPlus;

public class menuManager : MonoBehaviour, IDataPersistence
{
    [Header("Menus")]
    [SerializeField] public float SkyBoxSpeed;
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

    [Header("Audio")]
    [SerializeField] public AudioSource aud;
    public AudioClip mainSound;

    [Header("-----Mute Images-----")]
    [SerializeField] Sprite Mute;
    [SerializeField] Sprite UnMute;
    [SerializeField] Image image;

    public bool isMuted;
    public int Level;

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

        if (!isMuted)
        {
            image.sprite = UnMute;
            isMuted = false;
        }
        else
        {
            image.sprite = Mute;
            isMuted = true;
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
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * SkyBoxSpeed);
        
    }

    //public void muteSounds()
    //{

    //    if (!AudioControls.instance.isMuted)
    //    {
    //        image.sprite = Mute;
    //        AudioControls.instance.isMuted = true;
    //        isMuted = AudioControls.instance.isMuted;
    //        AudioControls.instance.Muted();
    //    }
    //    else
    //    {
    //        image.sprite = UnMute;
    //        AudioControls.instance.isMuted = false;
    //        isMuted = AudioControls.instance.isMuted;
    //        AudioControls.instance.Muted();
    //    }
    //}


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
        DataPersistenceManager.instance.LoadAudio();
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
        if (Input.GetButton("Jump") && !gameManager.instance.isPaused)
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


    public void LoadData(GameData data)
    {
        Level = data.level;
    }

    public void SaveData(GameData data)
    {
        data.level = Level;
    }
}
