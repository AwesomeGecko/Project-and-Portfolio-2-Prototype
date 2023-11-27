using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] int sensitivity;
    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;

    [SerializeField] bool invtertY;

    private float xRot;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;

        switch (invtertY)
        {
            case true:
                xRot += mouseY;
                break;
            case false:
                xRot -= mouseY;
                break;
        }

        xRot = Mathf.Clamp(xRot, lockVertMin, lockVertMax);
        transform.localRotation = Quaternion.Euler(xRot, 0, 0);
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
