using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteButtons : MonoBehaviour
{

    [Header("-----Mute Images-----")]
    [SerializeField] Sprite sprite1;
    [SerializeField] Sprite sprite2;
    [SerializeField] Image image;
    public bool toggled;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void muteSounds()
    {
        if (!gameManager.instance.isMuted)
        {
            image.sprite = sprite2;
            gameManager.instance.isMuted = true;
        }
        else
        {
            image.sprite = sprite1;
            gameManager.instance.isMuted = false;
        }
    }


}
