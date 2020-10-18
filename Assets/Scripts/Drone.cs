using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public abstract class Drone : MonoBehaviour
{
    public enum BehaviorState
    {
        WANDER,
        SEEK,
        ARRIVE,
        FLEE,
        FLOCK,
        CAPTURE
    };

    private const float healthRegenMultiplier = 6f;
    private const float attackScoreMultiplier = 0.5f;

    public BehaviorState behaviorState = BehaviorState.WANDER;
    //AIRigidbody rb;
    public Drone drone; // this drone

    Vision vision;

    [Header("Vision Objects")]
    //Shooting shoot;
    public Transform target;
    public Resource resourceObject;
    public GeneticAlgorithm territoryObject;
    public Faction1 faction1;
    public Faction2 faction2;

    //GameObject debugRing;

    [Header("Flocking")]
    public float cohesionWeight = 1.5f;
    public float separationWeight = 2f;
    public float velocityMatchWeight = 1f;

    [Header("Arrive")]
    public float arriveSize;
    // Keys
    protected string wanderStr = "WANDER";
    protected string seekStr = "SEEK";
    protected string fleeStr = "FLEE";
    protected string arriveStr = "ARRIVE";
    protected string flockStr = "FLOCK";
    protected string captureStr = "CAPTURE";

    [Header("Attributes")]
    public float wander;
    public float seek;
    public float arrive;
    public float flee;
    public float flock;
    public float capture;

    public float attack;
    public float speed;
    public float visionRange;
    public float health;
    public float maxHealth;
    public float hungerMeter;
    public float fitnessScore;
    public float healthNorm;

    public int droneNum;

    [Header("Other")]
    public AvoidSensor colAvoidSensor;
    public Slider slider;
    public GameObject impactEffect;
    public GameObject deathEffect;
    public LineRenderer lineRenderer;
    public Transform firePoint;
    public float attacktimer;
    public float linetimer;
    public float miningtimer;
    public bool hungryBool;

    World world; 
    protected virtual void Start()
    {
        colAvoidSensor = transform.Find("ColAvoidSensor").GetComponent<AvoidSensor>();
        vision = transform.Find("Vision").GetComponent<Vision>();
        vision.GetComponent<CircleCollider2D>().radius = visionRange;
        world = GameObject.FindWithTag("World").GetComponent<World>();
        fitnessScore = 0f;
        hungryBool = false;
        GetBehaviorPriority();
        //shoot = GetComponent<Shooting>();
    }

    protected virtual void FixedUpdate()
    {
        TimeLived();
        HealthDegen();
        GetVisionTargets();
        GetBehaviorPriority();
        DroneBehavior();
        CheckHunger();
    }

    private void CheckHunger()
    {
        // If life is lower than hunger Meter, then hungrybool is true
        if (healthNorm < hungerMeter)
        {
            hungryBool = true;
        }
        // if life is 100%, then false
        if (healthNorm > .99f)
        {
            hungryBool = false;
        }
    }

    private void TimeLived()
    {
        fitnessScore += (Time.deltaTime / 2);
    }

    private void GetVisionTargets()
    {
        if (resourceObject != null && !vision.targets.Contains(resourceObject.gameObject))
        {
            resourceObject = null;
        }
        if (faction1 != null && !vision.targets.Contains(faction1.gameObject))
        {
            faction1 = null;
        }
        if (faction2 != null && !vision.targets.Contains(faction2.gameObject))
        {
            faction2 = null;
        }
        if (territoryObject != null && !vision.targets.Contains(territoryObject.gameObject))
        {
            territoryObject = null;
        }

        foreach (GameObject target in vision.targets)
        {
            //Debug.Log("Inside foreach");
            // SET UP TYPES OF OBJECTS
            if (target.GetComponent<Resource>())
            {
                if (resourceObject == null)
                    resourceObject = target.GetComponent<Resource>();
                else if (Vector3.Distance(target.transform.position, transform.position) < Vector3.Distance(resourceObject.transform.position, transform.position))
                    resourceObject = target.GetComponent<Resource>();
            }
            else if (target.GetComponent<Faction1>())
            {
                if (faction1 == null)
                    faction1 = target.GetComponent<Faction1>();
                else if (Vector3.Distance(target.transform.position, transform.position) < Vector3.Distance(faction1.transform.position, transform.position))
                    faction1 = target.GetComponent<Faction1>();
            }
            else if (target.GetComponent<Faction2>())
            {
                if (faction2 == null)
                    faction2 = target.GetComponent<Faction2>();
                else if (Vector3.Distance(target.transform.position, transform.position) < Vector3.Distance(faction2.transform.position, transform.position))
                    faction2 = target.GetComponent<Faction2>();
            }
            else if (target.GetComponent<GeneticAlgorithm>())
            {
                if (territoryObject == null)
                    territoryObject = target.GetComponent<GeneticAlgorithm>();
                else if (Vector3.Distance(target.transform.position, transform.position) < Vector3.Distance(territoryObject.transform.position, transform.position))
                    territoryObject = target.GetComponent<GeneticAlgorithm>();
            }
        }
    }

    protected virtual void GetBehaviorPriority() { }
    protected virtual void DroneBehavior() { }

    public virtual void Clear()
    {
        if (behaviorState == BehaviorState.WANDER || behaviorState == BehaviorState.FLEE || behaviorState == BehaviorState.FLOCK)
        {
            lineRenderer.enabled = false;
            attacktimer = 0f;
        }
    }

    public virtual void DestroyWhenHealthIsZero()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public virtual int DestroyDrone(int numPopulation)
    {
        if (health < 0)
        {
            if (FindObjectOfType<AudioManager>())
            {
                FindObjectOfType<AudioManager>().Play("Explosion");
            }
            Instantiate(deathEffect, transform.position, Quaternion.Euler(new Vector3(0f, 0f, Random.Range(0f, 360f))));

            if (gameObject.GetComponent<Faction1>())
            {
                world.faction1DroneList.Add(SetAttributes(this));
                world.validList.Add(droneNum, fitnessScore);
            }
            if (gameObject.GetComponent<Faction2>())
            {
                world.faction2DroneList.Add(SetAttributes(this));
            }

            Destroy(gameObject);
            numPopulation--;
        }
        return numPopulation;
    }

    public World.DroneAttributes SetAttributes(Drone drone)
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

        string droneName = drone.name;
        string[] splitArray = droneName.Split(' ');

        if (splitArray[1] == "Parent")
        {
            factionParent.generation = "0";
        }
        else
        {
            factionParent.generation = splitArray[2];
        }
        return factionParent;
    }

    public virtual void SetHealthSlider()
    {
        healthNorm = health / maxHealth;
        slider.value = healthNorm;
    }

    public virtual void Attack(Drone enemyDrone)
    { 
        if (enemyDrone.health > 0)
        {
            if (FindObjectOfType<AudioManager>())
            {
                FindObjectOfType<AudioManager>().Play("Blaster");
            }
            enemyDrone.health -= attack;
            fitnessScore += (attack * attackScoreMultiplier);
        }
    }
    public virtual void HealthDegen()
    {
        if (health > 0)
        {
            health -= Time.deltaTime;
        }
    }
    public virtual void HealthRegen()
    {
        if (health < maxHealth)
        {
            health += (Time.deltaTime * healthRegenMultiplier);
        }
    }

}
