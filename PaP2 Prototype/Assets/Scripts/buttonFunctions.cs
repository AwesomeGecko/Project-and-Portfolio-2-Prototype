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
        Application.Quit();
    }

    public void controls()
    {
        gameManager.instance.openControls();
    }

    public void back()
    {
        gameManager.instance.backBttn();
    }
}
