using System.Collections;
using System.Collections.Generic;
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

        public float fitnessScore;
    }

    public DroneAttributes Faction1FirstParent;
    public DroneAttributes Faction1SecondParent;

    public DroneAttributes Faction2FirstParent;
    public DroneAttributes Faction2SecondParent;

    public int maxPopulation1;
    public int maxPopulation2;
    public int numPopulation1 = 0;
    public int numPopulation2 = 0;
    public int ownedTerritory1 = 2;
    public int ownedTerritory2 = 2;

    public int generation = 1;

    // Start is called before the first frame update
    void Start()
    {
        Faction1FirstParent = new DroneAttributes();
        Faction1SecondParent = new DroneAttributes();

        maxPopulation1 = ownedTerritory1 * 5;
        maxPopulation2 = ownedTerritory2 * 5;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        maxPopulation1 = ownedTerritory1 * 5;
        maxPopulation2 = ownedTerritory2 * 5;
    }

}
