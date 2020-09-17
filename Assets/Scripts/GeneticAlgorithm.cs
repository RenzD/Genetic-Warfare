using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GeneticAlgorithm : MonoBehaviour
{
    public enum TerritoryState
    {
        UNCAPTURED,
        FACTION1,
        FACTION2
    }
    [Header("State")]
    public TerritoryState territoryState = TerritoryState.UNCAPTURED;

    [Header("GameObjects")]
    public Faction1 dronePrefab;
    public Faction2 dronePrefab2;
    public World world;
    public Drone faction1Mom;
    public Drone faction1Dad;

    [Header("Info")]
    public float timeLeft1 = 0f;
    public float timeLeft2 = 0f;
    public float capturePoint = 0;
    public float maxCapturePoint = 10f;
    public float minCapturePoint = -10f;

    Drone[] crossOrder;
    Drone tempGO;
    SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CapturePoints();
    }

    private void CapturePoints()
    {
        // IF CAPTURE POINT IS 25 AND TERRITORY IS NOT OWN FACTION
        if (capturePoint >= maxCapturePoint && territoryState != TerritoryState.FACTION1 && territoryState != TerritoryState.UNCAPTURED)
        {
            // CAPTURE THE TERRITORY
            sprite.color = new Color(1, 0, 0, 1);
            capturePoint = maxCapturePoint;
            territoryState = TerritoryState.FACTION1;
            
            world.ownedTerritory1++;
            world.ownedTerritory2--;
        }
        else if (capturePoint <= minCapturePoint && territoryState != TerritoryState.FACTION2 && territoryState != TerritoryState.UNCAPTURED)
        {
            sprite.color = new Color(0, 1, 0, 1);
            capturePoint = minCapturePoint;
            territoryState = TerritoryState.FACTION2;
            world.ownedTerritory2++;
            world.ownedTerritory1--;
        }

        // UNCAPTURED TERRITORY
        else if (territoryState == TerritoryState.UNCAPTURED)
        {
            //sprite.color = new Color(0, 1, 0, 1);
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
                sprite.color = new Color(1, 0, 0, 1);
                if (world.numPopulation1 < world.maxPopulation1)
                {
                    timeLeft1 -= Time.deltaTime;
                    if (timeLeft1 < 0)
                    {
                        //Fitness/Selection
                        ParentSelection("Drone1");

                        if (faction1Mom == null || faction1Dad == null)
                        {
                            InitializeRandomAttributes(dronePrefab);
                            Faction1 droneObj = Instantiate(dronePrefab, new Vector3(transform.position.x, transform.position.y - 0.6f, 0), Quaternion.identity);
                            world.numPopulation1++;
                        }
                        else if (faction1Mom != null && faction1Dad != null)
                        {
                            //Genetic Algorithm
                            Crossover(dronePrefab);
                            Mutation(dronePrefab);

                            Faction1 droneObj = Instantiate(dronePrefab, new Vector3(transform.position.x, transform.position.y - 0.6f, 0), Quaternion.identity);
                            droneObj.name = "Child";
                            world.numPopulation1++;
                        }
                        timeLeft1 = 2f;
                    }
                }
            }
            if (territoryState == TerritoryState.FACTION2)
            {
                sprite.color = new Color(0, 0, 1, 1);
                if (world.numPopulation2 < world.maxPopulation2)
                {
                    timeLeft2 -= Time.deltaTime;
                    if (timeLeft2 <= 0)
                    {
                        InitializeRandomAttributes(dronePrefab2);
                        Instantiate(dronePrefab2, new Vector3(transform.position.x, transform.position.y - 0.6f, 0), Quaternion.identity);
                        world.numPopulation2++;
                        timeLeft2 = 2f;
                    }
                }
            }
        }
    }

    /**
     * Crossover/mixes 2 parent drones attributes to create a new drone 
     */
    private void Crossover(Faction1 droneObj)
    {
        //Shuffles what parent will pass down their attribute
        Shuffle();
        droneObj.wander = crossOrder[0].wander;
        droneObj.seek = crossOrder[1].seek;
        droneObj.arrive = crossOrder[2].arrive;
        droneObj.flee = crossOrder[3].flee;
        droneObj.flock = crossOrder[4].flock;
        droneObj.health = crossOrder[5].health;
        droneObj.attack = crossOrder[6].attack;
        droneObj.speed = crossOrder[7].speed;
        droneObj.maxHealth = crossOrder[8].maxHealth;
        droneObj.capture = crossOrder[9].capture;
        droneObj.visionRange = crossOrder[10].visionRange;
    }

    /**
     * Mutates a random attribute of our new drone
     * Still needs tweaking/balancing for some attributes
     */
    private void Mutation(Faction1 dronePrefab)
    {
        int rdmAttrPos = Random.Range(0, 10);
        float rdmBehavior = Random.Range(-3f, 3f);
        float rdmStat = Random.Range(-1f, 1f);

        switch (rdmAttrPos)
        {
            case 0:
                dronePrefab.wander += rdmBehavior;
                break;
            case 1:
                dronePrefab.seek += rdmBehavior;
                break;
            case 2:
                dronePrefab.arrive += rdmBehavior;
                break;
            case 3:
                dronePrefab.flee += rdmBehavior;
                break;
            case 4:
                dronePrefab.flock += rdmBehavior;
                break;
            case 5:
                dronePrefab.health += rdmStat;
                break;
            case 6:
                dronePrefab.attack += rdmStat;
                break;
            case 7:
                dronePrefab.speed += rdmStat;
                break;
            case 8:
                dronePrefab.maxHealth += rdmStat;
                break;
            case 9:
                dronePrefab.capture += rdmStat;
                break;
            case 10:
                dronePrefab.visionRange += rdmStat;
                break;
        }
    }

    /**
     * Crossover uses this method to shuffle the order of parents to pass down their type of attributes
     */
    private void Shuffle()
    {
        crossOrder = new Drone[] { faction1Mom, faction1Mom, faction1Mom, faction1Mom, faction1Mom, faction1Mom, faction1Dad, faction1Dad, faction1Dad, faction1Dad, faction1Dad };
        for (int i = 0; i < crossOrder.Length; i++)
        {
            int rnd = Random.Range(0, crossOrder.Length);
            tempGO = crossOrder[rnd];
            crossOrder[rnd] = crossOrder[i];
            crossOrder[i] = tempGO;
        }
    }

    // Set top 2 parent drones
    private void ParentSelection(string droneTag)
    {
        List<GameObject> bestDrones = new List<GameObject>();
        GameObject[] droneObjects;

        droneObjects = GameObject.FindGameObjectsWithTag(droneTag);
        foreach (GameObject drone in droneObjects)
        {
            Drone droneTracker = drone.GetComponent<Drone>();
            bestDrones = droneObjects.OrderByDescending(x => x.GetComponent<Drone>().fitnessScore).ToList();
        }

        if (bestDrones.Count >= 2 && bestDrones.ElementAt(0).GetComponent<Drone>().fitnessScore != 0 &&
                                     bestDrones.ElementAt(1).GetComponent<Drone>().fitnessScore != 0)
        {
            faction1Mom = bestDrones.ElementAt(0).GetComponent<Drone>();
            faction1Dad = bestDrones.ElementAt(1).GetComponent<Drone>();
            //Debug.Log(bestDrones.ElementAt(0).GetComponent<Faction1>().fitnessScore.ToString());
            //Debug.Log(bestDrones.ElementAt(1).GetComponent<Faction1>().fitnessScore.ToString());
        }
        //Debug.Log("====================================");
    }

    /**
     * Initializes random attributes for drones when there are no parents drones yet
     * Wander, seek, arrive, flee, flock attributes will total a sum of 100
     */
    private void InitializeRandomAttributes(Drone droneObj)
    {
        //float[] attributes = { Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)};
        float[] att = new float[6];
        att[0] = Random.Range(0f, 1f); // Wander
        att[1] = Random.Range(0f, 1f); // Seek
        att[2] = Random.Range(0f, 1f); // Arrive
        att[3] = Random.Range(0f, 1f); // Flee
        att[4] = Random.Range(0f, 1f); // Flock
        att[5] = Random.Range(0f, 1f); // Capture

        float sum = 0;
        foreach (float val in att)
        {
            sum += val;
        }

        droneObj.wander = (att[0] / sum) * 100f;
        droneObj.seek = (att[1] / sum) * 100f;
        droneObj.arrive = (att[2] / sum) * 100f;
        droneObj.flee = (att[3] / sum) * 100f;
        droneObj.flock = (att[4] / sum) * 100f;
        droneObj.capture = (att[5] / sum) * 100f;

        droneObj.health = Random.Range(30f, 40f);    // Healthh
        droneObj.attack = Random.Range(5f, 10f);     // Attack
        droneObj.speed = Random.Range(3f, 5f);       // Speed
        droneObj.visionRange = Random.Range(4f, 6f); // Vision Range

    }
}
