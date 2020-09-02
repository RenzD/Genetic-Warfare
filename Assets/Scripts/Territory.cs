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
    public World world;

    public float timeLeft1 = 0f;
    float timeLeft2 = 1f;
    public float capturePoint = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (capturePoint >= 5f && territoryState != TerritoryState.FACTION1)
        {
            capturePoint = 5f;
            territoryState = TerritoryState.FACTION1;
            world.ownedTerritory1++;
        }
        else if (capturePoint <= -5f && territoryState != TerritoryState.FACTION2)
        {
            capturePoint = -5f;
            territoryState = TerritoryState.FACTION2;
            world.ownedTerritory2++;
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
                        Instantiate(dronePrefab, new Vector3(transform.position.x, transform.position.y - 0.6f, 0), Quaternion.identity);
                        world.numPopulation2++;
                        timeLeft2 = 5f;
                    }
                }
            }
        }
    }
}
