using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCameraControls : MonoBehaviour
{
    private Transform cameraTransform;

    private float cameraMoveSpeed = 50.0f;

    private Vector3 moveDirection = Vector3.zero;

    private float cameraZoomDelta = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        this.cameraTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        this.moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            this.moveDirection.y += 1.0f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.moveDirection.x += 1.0f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.moveDirection.y -= 1.0f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.moveDirection.x -= 1.0f;
        }        

        this.moveDirection.Normalize();
        this.cameraTransform.Translate(this.moveDirection * this.cameraMoveSpeed * Time.deltaTime);

        if (Input.mouseScrollDelta.y > 0)
        {
            Camera.main.orthographicSize -= this.cameraZoomDelta;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            Camera.main.orthographicSize += this.cameraZoomDelta;
        }
    }    
}
