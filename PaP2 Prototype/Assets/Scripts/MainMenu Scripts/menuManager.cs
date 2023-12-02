using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuManager : MonoBehaviour
{
    public static menuManager instance;

    [Header("Menus")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuMain;
    [SerializeField] GameObject menuBackStory;

    // Start is called before the first frame update
    void Start()
    {
        menuActive = menuMain;
    }

    // Update is called once per frame
    void Update()
    {

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

}
