using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Territory : MonoBehaviour
{
    public enum TerritoryState
    {
        UNCAPTURED,
        FACTION1,
        FACTION2
    }
    public TerritoryState territoryState = TerritoryState.UNCAPTURED;

    public GameObject dronePrefab;
    public GameObject dronePrefab2;
    public World world;

    public float timeLeft1 = 0f;
    public float timeLeft2 = 0f;
    public float capturePoint = 0;
    public float maxCapturePoint = 10f;
    public float minCapturePoint = -10f;

    // Update is called once per frame
    void FixedUpdate()
    {
        // IF CAPTURE POINT IS 25 AND TERRITORY IS NOT OWN FACTION
        if (capturePoint >= maxCapturePoint && territoryState != TerritoryState.FACTION1 && territoryState != TerritoryState.UNCAPTURED)
        {
            // CAPTURE THE TERRITORY
            capturePoint = maxCapturePoint;
            territoryState = TerritoryState.FACTION1;
            world.ownedTerritory1++;
            world.ownedTerritory2--;

        }
        else if (capturePoint <= minCapturePoint && territoryState != TerritoryState.FACTION2 && territoryState != TerritoryState.UNCAPTURED)
        {
            capturePoint = minCapturePoint;
            territoryState = TerritoryState.FACTION2;
            world.ownedTerritory2++;
            world.ownedTerritory1--;
        }

        // UNCAPTURED TERRITORY
        else if (territoryState == TerritoryState.UNCAPTURED)
        {
            if (capturePoint >= maxCapturePoint)
            {
                capturePoint = maxCapturePoint;
                territoryState = TerritoryState.FACTION1;
                world.ownedTerritory1++;
            }
            else if (capturePoint <= minCapturePoint)
            {
                capturePoint = minCapturePoint;
                territoryState = TerritoryState.FACTION2;
                world.ownedTerritory2++;
            }
        }


        if (territoryState != TerritoryState.UNCAPTURED)
        {
            //Debug.Log("Inside captured");
            if (territoryState == TerritoryState.FACTION1)
            {
                if (world.numPopulation1 < world.maxPopulation1)
                {
                    timeLeft1 -= Time.deltaTime;
                    if (timeLeft1 < 0)
                    {
                        Instantiate(dronePrefab, new Vector3(transform.position.x, transform.position.y - 0.6f, 0), Quaternion.identity);
                        world.numPopulation1++;
                        timeLeft1 = 2f;
                    }
                }
            }
            if (territoryState == TerritoryState.FACTION2)
            {
                if (world.numPopulation2 < world.maxPopulation2)
                {
                    timeLeft2 -= Time.deltaTime;
                    if (timeLeft2 <= 0)
                    {
                        Instantiate(dronePrefab2, new Vector3(transform.position.x, transform.position.y - 0.6f, 0), Quaternion.identity);
                        world.numPopulation2++;
                        timeLeft2 = 2f;
                    }
                }
            }
        }
    }
}
