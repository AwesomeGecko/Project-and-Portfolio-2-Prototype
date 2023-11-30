using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class damageIndicator : MonoBehaviour
{
    //This is only a start and will update when we go over it in the lecture
    //Below is explinations of what is going on

    private float intensity;

    PostProcessVolume volume;
    Vignette vignette;
    private bool isPaused;

    void Start()
    {
        //Finds the PostProcessVolume
        volume = GetComponent<PostProcessVolume>();
        //Finds the Vignette
        volume.profile.TryGetSettings<Vignette>(out vignette);

        if (!vignette)
        {
            //if the Vignette is not avalible or emty
            Debug.Log("error, empty vignette");
        }
        else
        {
            //turns off the Vignette by default
            vignette.enabled.Override(false);
        }
    }

    void Update()
    {
        //ONLY FOR TESTING PERPOUSES
        //Uses the left mouse button to show the effect
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(TakeDamageEffect());
        }

        isPaused = gameManager.instance.isPaused;

        if (isPaused)
        {
            vignette.enabled.Override(false);
        }
        
    }

    IEnumerator TakeDamageEffect()
    {
        intensity = 0.4f;

        //Turns on Vignette
        vignette.enabled.Override(true);
        //Sets intensity
        vignette.intensity.Override(0.4f);
        yield return new WaitForSeconds(0.4f);


        //waites for the intensity to go back to 0
        while (intensity > 0)
        {
            intensity -= 0.01f;

            //once the intensity goes below 0
            if (intensity < 0)
            {
                intensity = 0;
            }
            //Vignette intensity is updated
            vignette.intensity.Override(intensity);
            yield return new WaitForSeconds(0.01f);
        }

        //once the intinsity is at 0 it turns off the Vignette
        vignette.enabled.Override(false);
        yield break;
    }
}
