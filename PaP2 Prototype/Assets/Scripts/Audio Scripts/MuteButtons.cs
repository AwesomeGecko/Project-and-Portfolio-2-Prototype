using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteButtons : MonoBehaviour
{

    [Header("-----Mute Images-----")]
    [SerializeField] Sprite muted;
    [SerializeField] Sprite unmuted;
    [SerializeField] Image muteButtn;

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
            muteButtn.sprite = muted;
            gameManager.instance.isMuted = true;
        }
        else
        {
            muteButtn.sprite = unmuted;
            gameManager.instance.isMuted = false;
        }
    }

}
