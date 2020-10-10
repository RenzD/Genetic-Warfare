using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public Vector2 panLimit;
    public bool pauseToggle = false;
    public float scrollSpeed = 2f;
    Camera cam;

    float minSize = 20f;
    float maxSize = 30f;

    public float zoomDist;

    public GameObject selectCircle;
    public GameObject settings;
    public Stats stats;
    bool selectBool = false;
    Drone drone;
    float droneScore;
    public bool parentBool = false;

    private void Start()
    {
        cam = GetComponent<Camera>();

    }

    void Update()
    {
        PauseGame();
        CameraControls();
        Selector();
        if (drone == null)
        {
            selectCircle.SetActive(false);
        }

        if (drone != null && !parentBool)
        {
            droneScore = (float)Math.Round(drone.fitnessScore, 2);
            stats.UpdateDroneStats((float)Math.Round(drone.health, 2), droneScore);
        } 
        else if (!parentBool)
        {
            stats.UpdateDroneStats(0, droneScore);
        }
    }

    private void Selector()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {
                //Debug.Log("Hit " + hitInfo.transform.gameObject.name);
                if (hitInfo.transform.parent.gameObject.tag == "Drone1" || hitInfo.transform.parent.gameObject.tag == "Drone2")
                {
                    drone = hitInfo.transform.parent.gameObject.GetComponent<Drone>();
                    if (drone != null)
                    {
                        parentBool = false;
                        stats.SetStats(drone.wander, drone.seek, drone.flee, drone.flock, drone.arrive, drone.capture, drone.maxHealth,
                            (float)Math.Round(drone.health, 2), drone.attack, drone.speed, drone.visionRange, drone.hungerMeter, drone.fitnessScore);

                        string team = "";
                        string type = "";
                        string gen = "";

                        string str = drone.name;
                        string[] splitArray = str.Split(' ');
                        if (splitArray[0] == "Faction1")
                        {
                            team = "red";
                        }
                        else if (splitArray[0] == "Faction2")
                        {
                            team = "blue";
                        }
                        if (splitArray[1] == "Parent")
                        {
                            type = "Parent";
                            gen = "-";
                        } 
                        else if (splitArray[1] == "Gen")
                        {
                            type = "Child";
                            gen = splitArray[2];
                        }
                        
                        stats.SetHeader(team, type, gen);


                    }
                    selectBool = true;
                }
            }
            else
            {
                //Deselect
                //selectCircle.SetActive(false);
                //selectBool = false;
                //stats.SetStats(0,0,0,0,0,0,0,0,0,0,0,0);
            }
        }
        if (drone != null && selectBool)
        {
            selectCircle.SetActive(true);
            selectCircle.transform.position = drone.transform.position;
        }
    }

    private void CameraControls()
    {
        zoomDist = cam.orthographicSize - 10f;
        panLimit.x = (1 - zoomDist / 20f) * 35f;
        panLimit.y = (1 - zoomDist / 20f) * 20f;

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
        pos.x = Mathf.Clamp(pos.x, -(panLimit.x + 5f), panLimit.x + 5f);
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
