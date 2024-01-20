using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        gameManager.instance.stateUnpause();
    }

    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpause();
    }

    public void quit()
    {
        //SceneManager.LoadScene(0);
        Time.timeScale = gameManager.instance.timeScaleOrig;
    }

    public void quitwarning()
    {
        gameManager.instance.quitMenu();
    }

    public void respawnPlayer()
    {
        gameManager.instance.playerScript.playerRespawn();
        gameManager.instance.stateUnpause();
    }

    public void controls()
    {
        gameManager.instance.openControls();
    }

    public void settings()
    {
        gameManager.instance.openSettings();
    }

    public void back()
    {
        gameManager.instance.backBttn();
    }

    public void play()
    {
        gameManager.instance.stateUnpause();
    }

    public void SavedData()
    {
        gameManager.instance.openSavedScreen();
    }

    public void savePlayer()
    {
        
    }

    public void LoadPlayer()
    {
         
    }
}
