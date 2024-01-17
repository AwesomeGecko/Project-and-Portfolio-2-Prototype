using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class menuManager : MonoBehaviour
{
    public static menuManager instance;

    [Header("Menus")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPrevious;
    [SerializeField] GameObject menuMain;
    [SerializeField] GameObject menuBackStory;
    [SerializeField] GameObject menuControls;
    [SerializeField] GameObject menuSettings;
    [SerializeField] GameObject menuCredits;
    [SerializeField] GameObject menuExit;
    [SerializeField] GameObject menuLoadNew;

    [Header("Menus")]
    [SerializeField] TextMeshProUGUI fastForwardText;
    [SerializeField] Animator credits;

    // Start is called before the first frame update
    void Start()
    {
        menuActive = menuMain;
        fastForwardText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (menuActive == menuCredits)
        {
            StartCoroutine(spacebutton());
            spaceBarPressed();
            escPressed();
        }
        else 
        {
            fastForwardText.gameObject.SetActive(false);
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
        updateMenu();
        menuActive.SetActive(false);
        menuActive = menuCredits;
        menuActive.SetActive(true);
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

    public void escPressed()
    {
        if (Input.GetButton("Cancel"))
        {
            backBttn();
        }
    }

    IEnumerator spacebutton()
    {
        yield return new WaitForSeconds(2);
        fastForwardText.gameObject.SetActive(true);
    }
}
