using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public Vector2 panLimit;
    public bool pauseToggle = false;
    public float scrollSpeed = 2f;
    Camera cam;
    public float minSize = 20f;
    public float maxSize = 30f;

    public float maxX;
    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        PauseGame();
        CameraControls();
    }

    private void CameraControls()
    {
        maxX = cam.orthographicSize - 10f;
        panLimit.x = (1 - maxX / 20f) * 35f;
        panLimit.y = (1 - maxX / 20f) * 20f;

        Vector3 pos = transform.position;
        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.y += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
        {
            pos.y -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cam.orthographicSize -= scroll * scrollSpeed * 100f * Time.deltaTime;

        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minSize, maxSize);
        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.y = Mathf.Clamp(pos.y, -panLimit.y, panLimit.y);

        transform.position = pos;
    }

    private void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!pauseToggle)
            {
                Time.timeScale = 0;
                pauseToggle = true;
            }
            else
            {
                Time.timeScale = 1;
                pauseToggle = false;
            }
        }
    }
}
