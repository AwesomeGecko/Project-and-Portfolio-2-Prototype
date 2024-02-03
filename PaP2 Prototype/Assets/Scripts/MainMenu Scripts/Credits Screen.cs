using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScreen : MonoBehaviour
{
    [SerializeField] Animator credits;

    private void Update()
    {
        spaceBarPressed();
        escapePresed();
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
        if (Input.GetButtonDown("Cancel"))
        {
            SceneManager.LoadScene("Main Menu");
        }
    }

    public void StartCredits()
    {
        SceneManager.LoadScene("Main Menu");
    }

}
