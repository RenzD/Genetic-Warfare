using UnityEngine;

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
    public int numInitDrones1;
    public int numInitDrones2;

    public int initOwnedMultiplier = 5;
    public int capturedMultiplier = 3;

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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateMaxPopulation();
    }

    private void UpdateMaxPopulation()
    {
        maxPopulation1 = capturedTerritoryNum1 * capturedMultiplier + initTerritoryNum1 * initOwnedMultiplier;
        maxPopulation2 = capturedTerritoryNum2 * capturedMultiplier + initTerritoryNum2 * initOwnedMultiplier;
    }
}
