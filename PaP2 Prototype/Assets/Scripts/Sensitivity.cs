using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Sensitivity : MonoBehaviour
{
    [SerializeField] public int Sens;
    [SerializeField] Slider Tivity;
    [SerializeField] TextMeshProUGUI text;

    public string SceneName;

    public void Start()
    {
        text.text = "1";
        SceneName = SceneManager.GetActiveScene().name;

        if (SceneName == "Main Menu")
        {
            Sens = Convert.ToInt32(PlayerPrefs.GetFloat("Tivity", 100));
            Tivity.value = Sens;
        }
        else 
        {
            gameManager.instance.cameraScript.sensitivity = Convert.ToInt32(PlayerPrefs.GetFloat("Tivity", 100));
            Tivity.value = gameManager.instance.cameraScript.sensitivity;
        }
    }

    public void OnDisable()
    {
        PlayerPrefs.Save();
    }

    //only call on other levels
    public void SetSens()
    { 
        Sens = gameManager.instance.cameraScript.sensitivity / 100;
        gameManager.instance.cameraScript.sensitivity = Convert.ToInt32(Tivity.value);
        PlayerPrefs.SetFloat("Tivity", Tivity.value);
        text.text = Sens.ToString();
    }

    //only call on main menu
    public void SetSensMainMenu()
    {
        Sens = Convert.ToInt32(Tivity.value);
        int New_Sens = Sens / 100;
        PlayerPrefs.SetFloat("Tivity", Tivity.value);
        text.text = New_Sens.ToString();
    }

}
