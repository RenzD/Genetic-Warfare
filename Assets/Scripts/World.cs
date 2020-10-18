using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using UnityEngine.UI;

public class World : MonoBehaviour
{
    public struct DroneAttributes
    {
        public float wander;
        public float seek;
        public float arrive;
        public float flee;
        public float flock;

        public float health;
        public float maxHealth;
        public float attack;
        public float speed;
        public float capture;
        public float visionRange;
        public float hungerMeter;

        public float fitnessScore;

        public string generation;
    }

    public DroneAttributes Faction1FirstParent;
    public DroneAttributes Faction1SecondParent;

    public DroneAttributes Faction2FirstParent;
    public DroneAttributes Faction2SecondParent;

    public int maxPopulation1;
    public int maxPopulation2;
    public int numPopulation1 = 0;
    public int numPopulation2 = 0;
    public int capturedTerritoryNum1;
    public int capturedTerritoryNum2;

    public int initTerritoryNum1;
    public int initTerritoryNum2;

    public int generation1;
    public int generation2;
    public int droneNum1;
    public int droneNum2;

    public int numInitDrones1;
    public int numInitDrones2;

    public int initOwnedMultiplier = 5;
    public int capturedMultiplier = 3;

    public float simTime;

    public Dictionary<int, float> validList;
    public List<DroneAttributes> faction1DroneList;
    public List<DroneAttributes> faction2DroneList;
    public Text textSimTime;
    private void Awake()
    {
        // Generation, Fitness
        validList = new Dictionary<int, float>();
        faction1DroneList = new List<World.DroneAttributes>();
        faction2DroneList = new List<World.DroneAttributes>();

    }
    // Start is called before the first frame update
    void Start()
    {
        Faction1FirstParent = new DroneAttributes();
        Faction1SecondParent = new DroneAttributes();
        foreach (GeneticAlgorithm territory in GameObject.FindObjectsOfType<GeneticAlgorithm>())
        {
            if (territory.territoryState == GeneticAlgorithm.TerritoryState.FACTION1)
            {
                initTerritoryNum1++;
            }
            else if (territory.territoryState == GeneticAlgorithm.TerritoryState.FACTION2)
            {
                initTerritoryNum2++;
            }
        }
        UpdateMaxPopulation();
        generation1 = 1;
        generation2 = 1; 
        droneNum1 = 1;
        droneNum2 = 1;
    }

    // Update is called once per frame
    void Update()
    {
        simTime += Time.deltaTime;
        textSimTime.text = "Time: " + ((float) Math.Round(simTime, 2)).ToString();
        UpdateMaxPopulation();

        /*
        if (Input.GetKeyUp(KeyCode.G))
        {
            // Order values in descending order. LINQ
            var items = from pair in validList
                        orderby pair.Key ascending
                        select pair;

            string filepath = Application.dataPath + "/Data/" + "validation4.csv";
            foreach (KeyValuePair<int, float> pair in items)
            {
                Debug.Log("Key: " + pair.Key + " Value: " + pair.Value);
                addRecord(pair.Key, pair.Value, filepath);
            }
        }
        */
    }

    private void UpdateMaxPopulation()
    {
        maxPopulation1 = capturedTerritoryNum1 * capturedMultiplier + initTerritoryNum1 * initOwnedMultiplier;
        maxPopulation2 = capturedTerritoryNum2 * capturedMultiplier + initTerritoryNum2 * initOwnedMultiplier;
    }

    private void addRecord(int generation, float fitnessScore, string filepath)
    {
        try
        {
            using (StreamWriter file = new StreamWriter(filepath, true))
            {
                file.WriteLine(generation.ToString() + "," + fitnessScore.ToString());
            }
        }
        catch (Exception e)
        {
            throw new ApplicationException("This program did an oopsie", e);
        }
    }
}
