using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController2 : MonoBehaviour
{
    public float RotationSpeed = 1;
    public Transform CamTarget, Player;
    float mouseX, mouseY;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        CamControl();
    }

    void CamControl()
    {
        mouseX += Input.GetAxis("Mouse X") * RotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * RotationSpeed;
        mouseY = Mathf.Clamp(mouseY, -35, 60);


        if (Input.GetKey(KeyCode.LeftShift))
        {
            CamTarget.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        }
        else
        {
            // takes the X movement of the mouse and applies to a Y rotation, takes Y mouse and rotates around the x axis
            CamTarget.rotation = Quaternion.Euler(mouseY, mouseX, 0);
            //Player.rotation = Quaternion.Euler(0, mouseX, 0);
        }

        transform.LookAt(CamTarget);
    }
}
