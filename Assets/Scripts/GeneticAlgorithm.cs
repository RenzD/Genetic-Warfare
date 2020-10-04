using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
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

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        GameObject w = GameObject.FindWithTag("World");
        world = w.GetComponent<World>();
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

        // When a territory is uncaptured and the capture point reaches a certain number
        // The territory is given to the faction
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
                    timeLeft1 -= Time.deltaTime;
                    if (timeLeft1 < 0)
                    {
                        //Fitness/Selection
                        ParentSelection("Drone1");
                        // Initial population
                        if (world.Faction1FirstParent.fitnessScore == 0 || world.Faction1SecondParent.fitnessScore == 0 || world.numInitDrones1 < 5)
                        {
                            InitializeRandomAttributes(dronePrefab);
                            Faction1 droneObj = Instantiate(dronePrefab, new Vector3(spawner.transform.position.x, spawner.transform.position.y, 0), Quaternion.identity);
                            droneObj.name = "Faction1 Parent " + world.numInitDrones1;
                            world.numPopulation1++;
                            world.numInitDrones1++;
                        }
                        // Applies Genetic Algorithm to create new drones
                        else if (world.Faction1FirstParent.fitnessScore != 0 && world.Faction1SecondParent.fitnessScore != 0 && world.numInitDrones1 >= 5)
                        {
                            Crossover(dronePrefab, FACTION1NUM);
                            Mutation(dronePrefab);
                            Faction1 droneObj = Instantiate(dronePrefab, new Vector3(spawner.transform.position.x, spawner.transform.position.y, 0), Quaternion.identity);
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

                        if (faction2Mom == null || faction2Dad == null || world.numInitDrones2 < 5)
                        {
                            InitializeRandomAttributes(dronePrefab2);
                            Faction2 droneObj = Instantiate(dronePrefab2, new Vector3(spawner.transform.position.x, spawner.transform.position.y, 0), Quaternion.Euler(new Vector3(0, 0, 180)));
                            droneObj.name = "Faction2 Parent " + world.numInitDrones2;
                            world.numPopulation2++;
                            world.numInitDrones2++;
                        }
                        else if (faction2Mom != null && faction2Dad != null && world.numInitDrones2 >= 5)
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
    private void Mutation(Drone dronePrefab)
    {
        int rdmAttrPos = Random.Range(0, 10);
        float rdmBehavior = Random.Range(-3f, 3f);
        
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
                float rdmHealth = Random.Range(0f, 10f);
                dronePrefab.maxHealth += rdmHealth;
                dronePrefab.health += dronePrefab.maxHealth;
                break;
            case 7:
                float rdmAttack = Random.Range(0f, 1f);
                dronePrefab.attack += rdmAttack;
                break;
            case 8:
                float rdmSpeed = Random.Range(0f, 0.2f);
                dronePrefab.speed += rdmSpeed;
                break;
            case 9:
                float rdmVision = Random.Range(0f, 0.2f);
                dronePrefab.visionRange += rdmVision;
                break;
            case 10:
                if (dronePrefab.hungerMeter > 0.2f && dronePrefab.hungerMeter < 0.95f)
                {
                    float rdmHunger = Random.Range(-0.05f, 0.05f);
                    dronePrefab.hungerMeter += rdmHunger;
                    // Make sure it stays within parameter
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
    }

    /**
     * Crossover uses this method to shuffle the order of parents to pass down their type of attributes
     */
    private void ShuffleParents(int factionNum)
    {
        //Picks between faction 1 or 2
        if (factionNum == FACTION1NUM)
        {
            droneAttributes = new World.DroneAttributes[] { world.Faction1FirstParent, world.Faction1FirstParent, world.Faction1FirstParent, world.Faction1FirstParent, world.Faction1FirstParent, world.Faction1FirstParent,
                                                            world.Faction1SecondParent, world.Faction1SecondParent, world.Faction1SecondParent, world.Faction1SecondParent, world.Faction1SecondParent };
        }
        else if (factionNum == FACTION2NUM)
        {
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
    /*
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
            if (droneTag == "Drone1")
            {
                faction1Mom = bestDrones.ElementAt(0).GetComponent<Drone>();
                faction1Dad = bestDrones.ElementAt(1).GetComponent<Drone>();
            } 
            else if (droneTag == "Drone2")
            {
                faction2Mom = bestDrones.ElementAt(0).GetComponent<Drone>();
                faction2Dad = bestDrones.ElementAt(1).GetComponent<Drone>();
            }
            
            //Debug.Log(bestDrones.ElementAt(0).GetComponent<Faction1>().fitnessScore.ToString());
            //Debug.Log(bestDrones.ElementAt(1).GetComponent<Faction1>().fitnessScore.ToString());
        }
        //Debug.Log("====================================");
    }
    */

    /**
     * Selects the best drones as parents
     * Saves top 2 drones attributes to world
     */
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
            if (droneTag == "Drone1")
            {
                World.DroneAttributes factionParent;
                // First parent
                faction1Mom = bestDrones.ElementAt(0).GetComponent<Drone>();
                if (world.Faction1FirstParent.fitnessScore == 0)
                {
                    factionParent = SetAttributes(faction1Mom);
                    world.Faction1FirstParent = factionParent;
                }
                if (world.Faction1FirstParent.fitnessScore < faction1Mom.fitnessScore)
                {
                    factionParent = SetAttributes(faction1Mom);
                    world.Faction1FirstParent = factionParent;
                }
                // Second Parent
                faction1Dad = bestDrones.ElementAt(1).GetComponent<Drone>();
                if (world.Faction1SecondParent.fitnessScore == 0)
                {
                    factionParent = SetAttributes(faction1Dad);
                    world.Faction1SecondParent = factionParent;
                }
                if (world.Faction1SecondParent.fitnessScore < faction1Dad.fitnessScore)
                {
                    factionParent = SetAttributes(faction1Dad);
                    world.Faction1SecondParent = factionParent;
                }
            }
            else if (droneTag == "Drone2")
            {
                World.DroneAttributes factionParent;
                // First parent
                faction2Mom = bestDrones.ElementAt(0).GetComponent<Drone>();
                if (world.Faction2FirstParent.fitnessScore == 0)
                {
                    factionParent = SetAttributes(faction2Mom);
                    world.Faction2FirstParent = factionParent;
                }
                if (world.Faction2FirstParent.fitnessScore < faction2Mom.fitnessScore)
                {
                    factionParent = SetAttributes(faction2Mom);
                    world.Faction2FirstParent = factionParent;
                }
                // Second Parent
                faction2Dad = bestDrones.ElementAt(1).GetComponent<Drone>();
                if (world.Faction2SecondParent.fitnessScore == 0)
                {
                    factionParent = SetAttributes(faction2Dad);
                    world.Faction2SecondParent = factionParent;
                }
                if (world.Faction2SecondParent.fitnessScore < faction2Dad.fitnessScore)
                {
                    factionParent = SetAttributes(faction2Dad);
                    world.Faction2SecondParent = factionParent;
                }
            }
        }
    }

    /**
     * Saves/Sets the attributes of the best drones
     */
    private World.DroneAttributes SetAttributes(Drone drone)
    {
        World.DroneAttributes factionParent;
        factionParent.wander = drone.wander;
        factionParent.seek = drone.seek;
        factionParent.arrive = drone.arrive;
        factionParent.flee = drone.flee;
        factionParent.flock = drone.flock;

        factionParent.health = drone.health;
        factionParent.maxHealth = drone.maxHealth;
        factionParent.attack = drone.attack;
        factionParent.speed = drone.speed;
        factionParent.capture = drone.capture;
        factionParent.visionRange = drone.visionRange;
        factionParent.hungerMeter = drone.hungerMeter;

        factionParent.fitnessScore = drone.fitnessScore;

        return factionParent;
    }

    /**
     * Initializes random attributes for drones when there are no parents drones yet
     */
    private void InitializeRandomAttributes(Drone droneObj)
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

        droneObj.wander = Random.Range(35f, 40f);
        droneObj.seek = Random.Range(35f, 40f);
        droneObj.arrive = Random.Range(35f, 40f);
        droneObj.flee = Random.Range(35f, 40f);
        droneObj.flock = Random.Range(35f, 40f);
        droneObj.capture = Random.Range(35f, 40f);

        droneObj.maxHealth = Random.Range(30f, 40f);         // Healthh
        droneObj.health = droneObj.maxHealth;
        droneObj.attack = Random.Range(5f, 8f);              // Attack
        droneObj.speed = Random.Range(3.5f, 4f);             // Speed
        droneObj.visionRange = Random.Range(3f, 6f);         // Vision Range
        droneObj.hungerMeter = Random.Range(0.5f, 0.8f);     // Hunger
    }
}
