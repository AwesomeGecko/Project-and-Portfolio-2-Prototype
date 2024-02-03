using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingScript : MonoBehaviour
{
    public GameObject gunHolder;
    // Start is called before the first frame update
    private PlayerGunControls gunControl;

    void Start()
    {
        gunControl = gameManager.instance.playerGunControls;
    }

    public void ShutOffCrosshair()
    {
        //Deactivate the main crosshairs
        gameManager.instance.Crosshair.gameObject.SetActive(false);
    }

    public void AimDownSights()
    {

        GunSettings currentGun = gameManager.instance.playerGunControls.gunList[gameManager.instance.playerGunControls.selectedGun];

        gunControl.gunLocation.transform.localPosition = currentGun.ADSGunPositionOffset;
        //Adjust the camera properties
        Camera.main.fieldOfView = currentGun.fieldOfView;
        if (gunControl.isAiming)
        {

            
            //If the current gun wants the scope
            if (currentGun.shouldUseScope)
            {
                //Using Invoke enables the scope image overlay ontop of the main camera through a time delay
                ActivateM4Sight();
                //Adjust the scope cameras FOV
                Camera.main.fieldOfView = currentGun.fieldOfView;
            }

            //If the current gun is the shotgun use the shotgun sight
            else if (currentGun.isShotgun)
            {
                //Using Invoke enables the shotgun sight ontop of the main camera through a time delay
                ActivateShotgunSight();
                //Adjust the shotgun camera FOV
                Camera.main.fieldOfView = currentGun.fieldOfView;
            }
            else if (currentGun.isAssaultRifle)
            {
                //Debug.Log("Entering Scar case");
                //Using Invoke enables the assault rifle sight ontop of the main camera through a time delay
                ActivateAssaultRifleSight();
                //Adjust the scope cameras FOV
                Camera.main.fieldOfView = currentGun.fieldOfView;
            }
            else if (currentGun.GunName == "AK-101")
            {
                //Using Invoke enables the assault rifle sight ontop of the main camera through a time delay      
                ActivateAKSight();
                //Adjust the scope cameras FOV
                Camera.main.fieldOfView = currentGun.fieldOfView;
            }
        }
        else
        {
            //Deactivate the Scope image
            gameManager.instance.Scope.gameObject.SetActive(false);
            
            //Cull the gun back onto screen
            gunControl.scopeIn.cullingMask = gunControl.scopeIn.cullingMask | (1 << 7);
            //Adjust the main cameras FOV
            Camera.main.fieldOfView = currentGun.fieldOfView;
        }
    }

    public void NotAimingDownSight()
    {
        gunControl.isAiming = false;
        GunSettings currentGun = gameManager.instance.playerGunControls.gunList[gameManager.instance.playerGunControls.selectedGun];

        gunControl.gunLocation.transform.localPosition = Vector3.zero;

        currentGun.model.transform.localPosition = currentGun.defaultGunPositionOffset;
        currentGun.model.transform.localRotation = currentGun.defaultRotation;

        //Enable the main crosshairs
        gameManager.instance.Crosshair.gameObject.SetActive(true);

        //Disable the scope camera
        gameManager.instance.Scope.gameObject.SetActive(false);

        //Disable the shotgun sight
        gameManager.instance.ShotgunSight.gameObject.SetActive(false);

        //Disable the Assualt rifle sight
        gameManager.instance.AssaultRifleSight.gameObject.SetActive(false);
        //Disable the AK Rifle Sight

        gameManager.instance.AKSight.gameObject.SetActive(false);

        //Cull the gun onto screen
        gunControl.scopeIn.cullingMask = gunControl.scopeIn.cullingMask | (1 << 7);

        //Re-enable the main camera and set it to the default value
        Camera.main.fieldOfView = gunControl.defaultFOV;


    }

    //This method simply calls the UI image of the scope and sets it to true
    void ActivateM4Sight()
    {
        gameManager.instance.Scope.gameObject.SetActive(true);
        //Cull the gun out of screen by setting the gun model on a layer called weapon. Then the m4 will not be shown when the scope image is overlayed on the main camera.
        gunControl.scopeIn.cullingMask = gunControl.scopeIn.cullingMask & ~(1 << 7);
    }

    //This method simply calls the UI image of the shotgun sight and sets it to true
    void ActivateShotgunSight()
    {
        gameManager.instance.ShotgunSight.gameObject.SetActive(true);
    }

    //This method is to use the assault rifle UI sight.
    void ActivateAssaultRifleSight()
    {
        // Set local position using the selected gun's offset
        gunControl.gunLocation.localPosition = gameManager.instance.playerGunControls.gunList[gameManager.instance.playerGunControls.selectedGun].ADSGunPositionOffset;

        gameManager.instance.AssaultRifleSight.gameObject.SetActive(true);
        gunControl.scopeIn.cullingMask = gunControl.scopeIn.cullingMask & ~(1 << 7);
    }

    void ActivateAKSight()
    {
        // Set local position using the selected gun's offset
        gunControl.gunLocation.transform.localPosition = gameManager.instance.playerGunControls.gunList[gameManager.instance.playerGunControls.selectedGun].ADSGunPositionOffset;

        gameManager.instance.AKSight.gameObject.SetActive(true);
        gunControl.scopeIn.cullingMask = gunControl.scopeIn.cullingMask & ~(1 << 7);
    }
}
