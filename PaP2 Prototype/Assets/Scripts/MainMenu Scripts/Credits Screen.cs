using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScreen : MonoBehaviour
{
    public static CreditsScreen instance;

    

    public void StartCredits()
    {
        StartCoroutine(CreditsMenuOpen());
    }

    private IEnumerator CreditsMenuOpen()
    {
        Debug.Log("start credits");
        menuManager.instance.MainMenu();
        yield return new WaitForSecondsRealtime(42f);
        menuManager.instance.isCreditsOpen = false;
        Debug.Log("end credits");
    }
}
