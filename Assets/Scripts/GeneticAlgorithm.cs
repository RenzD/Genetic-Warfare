﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.IO;
using System.Diagnostics;

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
    public Image mask;
    public GameObject spawner;
    public Drone faction1Mom;
    public Drone faction1Dad;
    public Drone faction2Mom;
    public Drone faction2Dad;


    [Header("Info")]
    public float timeLeft1 = 0f;
    public float timeLeft2 = 0f;
    public float capturePoint = 0;
    public float maxCapturePoint = 10f;
    public float minCapturePoint = -10f;

    World.DroneAttributes[] droneAttributes; // contains best drone parents to crossover attributes
    World.DroneAttributes tempDroneAtt; // used for shuffling droneAttributes array order
    SpriteRenderer sprite; // used to change color
    
    static readonly int FACTION1NUM = 1;
    static readonly int FACTION2NUM = 2;

    static readonly int RANDOMDRONESNUM = 15;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        world = GameObject.FindWithTag("World").GetComponent<World>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CapturePoints();
        UpdateCaptureUI();
        CapturePointRegen();
    }

    private void CapturePointRegen()
    {
        if (territoryState == TerritoryState.FACTION1)
        {
            if (capturePoint > minCapturePoint)
            {
                if (capturePoint < maxCapturePoint)
                {
                    capturePoint += Time.deltaTime;
                }
            } 
        }
        else if (territoryState == TerritoryState.FACTION2)
        {
            if (capturePoint < maxCapturePoint)
            {
                if (capturePoint > minCapturePoint)
                {
                    capturePoint -= Time.deltaTime;
                }
            }
        } else if (territoryState == TerritoryState.UNCAPTURED)
        {
            if (capturePoint < 0)
            {
                capturePoint += Time.deltaTime / 3;
                if (capturePoint > 0)
                {
                    capturePoint = 0;
                }
            } else if (capturePoint > 0)
            {
                capturePoint -= Time.deltaTime / 3;
                if (capturePoint < 0)
                {
                    capturePoint = 0;
                }
            }
        }
    }

    private void UpdateCaptureUI()
    {
        if (capturePoint > 0)
        {
            mask.color = new Color(1, 0, 0, 1);
        }
        else
        {
            mask.color = new Color(0, 0, 1, 1);
        }
        mask.fillAmount = Math.Abs(capturePoint) / maxCapturePoint;
    }

    /**
     * Sets the states of territories whether its uncaptured, or owned by faction 1 or 2.
     */
    private void CapturePoints()
    {
        // If/else if - Territory is already captured and overturned by either faction
        if (capturePoint >= maxCapturePoint && territoryState != TerritoryState.FACTION1 && territoryState != TerritoryState.UNCAPTURED)
        {
            sprite.color = new Color(1, 0, 0, 1);
            capturePoint = maxCapturePoint;
            territoryState = TerritoryState.FACTION1;
            
            world.capturedTerritoryNum1++;
            world.capturedTerritoryNum2--;
        }
        else if (capturePoint <= minCapturePoint && territoryState != TerritoryState.FACTION2 && territoryState != TerritoryState.UNCAPTURED)
        {
            sprite.color = new Color(0, 1, 0, 1);
            capturePoint = minCapturePoint;
            territoryState = TerritoryState.FACTION2;
            world.capturedTerritoryNum2++;
            world.capturedTerritoryNum1--;
        }

        // When a territory is uncaptured and the capture point reaches a certain number
        // The territory is given to the faction
        else if (territoryState == TerritoryState.UNCAPTURED)
        {
            if (capturePoint >= maxCapturePoint)
            {
                capturePoint = maxCapturePoint;
                territoryState = TerritoryState.FACTION1;
                world.capturedTerritoryNum1++;
            }
            else if (capturePoint <= minCapturePoint)
            {
                capturePoint = minCapturePoint;
                territoryState = TerritoryState.FACTION2;
                world.capturedTerritoryNum2++;
            }
        }

        // When a territory is captured 
        // The owning faction spawns their drones
        // Initializes starting population, then uses the application of GA to modify drone attributes
        if (territoryState != TerritoryState.UNCAPTURED)
        {
            if (territoryState == TerritoryState.FACTION1)
            {
                sprite.color = new Color(1, 0, 0, 1);
                if (world.numPopulation1 < world.maxPopulation1)
                {
                    ParentSelection("Drone1");
                    timeLeft1 -= Time.deltaTime;
                    if (timeLeft1 < 0)
                    {
                        //Fitness/Selection
                        
                        // Initial population
                        if (world.Faction1FirstParent.fitnessScore == 0 || world.Faction1SecondParent.fitnessScore == 0 || world.numInitDrones1 < RANDOMDRONESNUM)
                        {
                            InitializeRandomAttributes(dronePrefab);
                            Faction1 droneObj = Instantiate(dronePrefab, new Vector3(spawner.transform.position.x, spawner.transform.position.y, 0), Quaternion.identity);
                            droneObj.droneNum = world.droneNum1++;
                            droneObj.name = "Faction1 Parent " + world.numInitDrones1;
                            world.generation1++;
                            world.numPopulation1++;
                            world.numInitDrones1++;
                        }
                        // Applies Genetic Algorithm to create new drones
                        else if (world.Faction1FirstParent.fitnessScore != 0 && world.Faction1SecondParent.fitnessScore != 0 && world.numInitDrones1 >= RANDOMDRONESNUM)
                        {
                            Crossover(dronePrefab, FACTION1NUM);
                            Mutation(dronePrefab);
                            Faction1 droneObj = Instantiate(dronePrefab, new Vector3(spawner.transform.position.x, spawner.transform.position.y, 0), Quaternion.identity);
                            droneObj.droneNum = world.droneNum1++;
                            droneObj.name = "Faction1 Gen " + world.generation1++;
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
                    if (timeLeft2 < 0)
                    {
                        //Fitness/Selection
                        ParentSelection("Drone2");
                        /*
                        InitializeDronesWithSetAttributes(dronePrefab2);
                        Faction2 droneObj = Instantiate(dronePrefab2, new Vector3(spawner.transform.position.x, spawner.transform.position.y, 0), Quaternion.Euler(new Vector3(0, 0, 180)));
                        droneObj.name = "Faction2 Parent " + world.numInitDrones2;
                        world.numPopulation2++;
                        world.numInitDrones2++;
                        */
                        if (faction2Mom == null || faction2Dad == null || world.numInitDrones2 < RANDOMDRONESNUM)
                        {
                            InitializeRandomAttributes(dronePrefab2);
                            Faction2 droneObj = Instantiate(dronePrefab2, new Vector3(spawner.transform.position.x, spawner.transform.position.y, 0), Quaternion.Euler(new Vector3(0, 0, 180)));
                            droneObj.name = "Faction2 Parent " + world.numInitDrones2;
                            world.numPopulation2++;
                            world.numInitDrones2++;
                        }
                        else if (faction2Mom != null && faction2Dad != null && world.numInitDrones2 >= RANDOMDRONESNUM)
                        {
                            //Genetic Algorithm
                            Crossover(dronePrefab2, FACTION2NUM);
                            Mutation(dronePrefab2);

                            Faction2 droneObj2 = Instantiate(dronePrefab2, new Vector3(spawner.transform.position.x, spawner.transform.position.y, 0), Quaternion.Euler(new Vector3 (0,0,180)));
                            droneObj2.name = "Faction2 Gen " + world.generation2++;
                            world.numPopulation2++;
                        }
                        
                        timeLeft2 = 2f;
                    }
                }
            }
        }
    }

    private void InitializeDronesWithSetAttributes(Faction2 droneObj)
    {
        droneObj.wander = 40f;
        droneObj.seek = 33f;
        droneObj.arrive = 35f;
        droneObj.flee = 32.5f;
        droneObj.flock = 36f;
        droneObj.capture = 38f;

        droneObj.maxHealth = 40f;      // Healthh
        droneObj.health = droneObj.maxHealth;
        droneObj.attack = 6.5f;            // Attack
        droneObj.speed = 3.5f;            // Speed
        droneObj.visionRange = 4f;         // Vision Range
        droneObj.hungerMeter = 0.6f;     // Hunger
        /*
         * droneObj.wander = 40f;
        droneObj.seek = 30f;
        droneObj.arrive = 35f;
        droneObj.flee = 32.5f;
        droneObj.flock = 36f;
        droneObj.capture = 38f;

        droneObj.maxHealth = 40f;      // Healthh
        droneObj.health = droneObj.maxHealth;
        droneObj.attack = 6.5f;            // Attack
        droneObj.speed = 3.5f;            // Speed
        droneObj.visionRange = 4f;         // Vision Range
        droneObj.hungerMeter = 0.6f;     // Hunger
        */
    }

    /**
     * Crossover/mixes 2 parent drones attributes to create a new drone 
     */
        private void Crossover(Drone droneObj, int factionNum)
    {
        //Shuffles what parent will pass down their attribute
        ShuffleParents(factionNum);
        droneObj.wander = droneAttributes[0].wander;
        droneObj.seek = droneAttributes[1].seek;
        droneObj.arrive = droneAttributes[2].arrive;
        droneObj.flee = droneAttributes[3].flee;
        droneObj.flock = droneAttributes[4].flock;
        droneObj.maxHealth = droneAttributes[5].maxHealth;
        droneObj.health = droneObj.maxHealth;
        droneObj.attack = droneAttributes[6].attack;
        droneObj.speed = droneAttributes[7].speed;
        droneObj.capture = droneAttributes[8].capture;
        droneObj.visionRange = droneAttributes[9].visionRange;
        droneObj.hungerMeter = droneAttributes[10].hungerMeter;
    }

    /**
     * Mutates 1 random attribute of our new drone
     */
    public int Mutation(Drone dronePrefab)
    {
        int rdmAttrPos = Random.Range(0, 10);
        float rdmBehavior = (float)Math.Round(Random.Range(-3f, 3f), 2);

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
                dronePrefab.capture += rdmBehavior;
                break;
            case 6:
                float rdmHealth = (float)Math.Round(Random.Range(0f, 10f), 2);
                dronePrefab.maxHealth += rdmHealth;
                dronePrefab.health += dronePrefab.maxHealth;
                break;
            case 7:
                float rdmAttack = (float)Math.Round(Random.Range(0f, 1f), 2);
                dronePrefab.attack += rdmAttack;
                break;
            case 8:
                float rdmSpeed = (float)Math.Round(Random.Range(0f, 0.2f), 2);
                dronePrefab.speed += rdmSpeed;
                break;
            case 9:
                float rdmVision = (float)Math.Round(Random.Range(0f, 0.2f), 2);
                dronePrefab.visionRange += rdmVision;
                break;
            case 10:
                if (dronePrefab.hungerMeter > 0.2f && dronePrefab.hungerMeter < 0.95f)
                {
                    float rdmHunger = (float)Math.Round(Random.Range(-0.05f, 0.05f), 2);
                    dronePrefab.hungerMeter += rdmHunger;
                    // Makes sure it stays within parameter
                    if (dronePrefab.hungerMeter < 0.2f)
                    {
                        dronePrefab.hungerMeter = 0.2f;
                    } 
                    else if (dronePrefab.hungerMeter > 0.95f)
                    {
                        dronePrefab.hungerMeter = 0.95f;
                    }
                }
                break;
        }
        return rdmAttrPos;
    }

    /**
     * Crossover uses this method to shuffle the order of parents to pass down their type of attributes
     */
    private void ShuffleParents(int factionNum)
    {
        //Picks between faction 1 or 2
        if (factionNum == FACTION1NUM)
        {
            // Crossover ratio - 8:3
            droneAttributes = new World.DroneAttributes[] { world.Faction1FirstParent, world.Faction1FirstParent, world.Faction1FirstParent, world.Faction1FirstParent, world.Faction1FirstParent, world.Faction1FirstParent,
                                                            world.Faction1FirstParent, world.Faction1FirstParent, world.Faction1SecondParent, world.Faction1SecondParent, world.Faction1SecondParent };
        }
        else if (factionNum == FACTION2NUM)
        {
            // Crossover ratio - 6:5
            droneAttributes = new World.DroneAttributes[] { world.Faction2FirstParent, world.Faction2FirstParent, world.Faction2FirstParent, world.Faction2FirstParent, world.Faction2FirstParent, world.Faction2FirstParent,
                                                            world.Faction2SecondParent, world.Faction2SecondParent, world.Faction2SecondParent, world.Faction2SecondParent, world.Faction2SecondParent };
        }
        //Shuffles content of crossOrder
        for (int i = 0; i < droneAttributes.Length; i++)
        {
            int rnd = Random.Range(0, droneAttributes.Length);
            tempDroneAtt = droneAttributes[rnd];
            droneAttributes[rnd] = droneAttributes[i];
            droneAttributes[i] = tempDroneAtt;
        }
    }
 
    /**
     * Selects the best drones as parents
     * Saves top 2 drones attributes to world
     */
    private void ParentSelection(string droneTag)
    {
        List<World.DroneAttributes> bestDrones = world.faction1DroneList.OrderByDescending(x => x.fitnessScore).ToList();
        List<World.DroneAttributes> bestDrones2 = world.faction2DroneList.OrderByDescending(x => x.fitnessScore).ToList();

        /* Cuts the list to 2 elements
        List<World.DroneAttributes> faction1Top2 = new List<World.DroneAttributes>();
        List<World.DroneAttributes> faction2Top2 = new List<World.DroneAttributes>();
        faction1Top2.Add(faction1Top2.ElementAt(0));
        faction1Top2.Add(faction1Top2.ElementAt(1));
        */

        if (bestDrones.Count >= 2 && bestDrones.ElementAt(0).fitnessScore != 0 &&
                                     bestDrones.ElementAt(1).fitnessScore != 0 &&
                                     droneTag == "Drone1")
        {
            world.Faction1FirstParent = bestDrones.ElementAt(0);
            world.Faction1SecondParent = bestDrones.ElementAt(1);
        }
        else if (bestDrones2.Count >= 2 && bestDrones2.ElementAt(0).fitnessScore != 0 &&
                                     bestDrones2.ElementAt(1).fitnessScore != 0 &&
                                     droneTag == "Drone2")
        {
            world.Faction2FirstParent = bestDrones2.ElementAt(0);
            world.Faction2SecondParent = bestDrones2.ElementAt(1);
        }
    }

    /**
     * Saves/Sets the attributes of the best drones
     */
    public World.DroneAttributes SetAttributes(Drone drone)
    {
        World.DroneAttributes stats;
        stats.wander = drone.wander;
        stats.seek = drone.seek;
        stats.arrive = drone.arrive;
        stats.flee = drone.flee;
        stats.flock = drone.flock;

        stats.health = drone.health;
        stats.maxHealth = drone.maxHealth;
        stats.attack = drone.attack;
        stats.speed = drone.speed;
        stats.capture = drone.capture;
        stats.visionRange = drone.visionRange;
        stats.hungerMeter = drone.hungerMeter;

        stats.fitnessScore = drone.fitnessScore;

        string droneName = drone.name;
        string[] splitArray = droneName.Split(' ');
        stats.generation = splitArray.Length >= 2 ? splitArray[1] == "Parent" ? "0" : splitArray[2] : "-";
        return stats;
    }

    /**
     * Initializes random attributes for drones when there are no parents drones yet
     */
    public void InitializeRandomAttributes(Drone droneObj)
    {
        //float[] attributes = { Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)};
        /* SUM of 100
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
        */

        droneObj.wander = (float) Math.Round(Random.Range(35f, 40f), 2);
        droneObj.seek = (float)Math.Round(Random.Range(35f, 40f), 2);
        droneObj.arrive = (float)Math.Round(Random.Range(35f, 40f), 2);
        droneObj.flee = (float)Math.Round(Random.Range(35f, 40f), 2);
        droneObj.flock = (float)Math.Round(Random.Range(35f, 40f), 2);
        droneObj.capture = (float)Math.Round(Random.Range(35f, 40f), 2);

        droneObj.maxHealth = (float)Math.Round(Random.Range(30f, 40f), 2);      // Healthh
        droneObj.health = droneObj.maxHealth;
        droneObj.attack = (float)Math.Round(Random.Range(5f, 8f), 2);            // Attack
        droneObj.speed = (float)Math.Round(Random.Range(3.5f, 4f), 2);            // Speed
        droneObj.visionRange = (float)Math.Round(Random.Range(3f, 6f), 2);         // Vision Range
        droneObj.hungerMeter = (float)Math.Round(Random.Range(0.5f, 0.8f), 2);     // Hunger
    }
}
