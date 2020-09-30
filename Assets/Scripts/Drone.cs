using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

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

    public BehaviorState behaviorState = BehaviorState.WANDER;
    //AIRigidbody rb;
    public Drone drone; // this drone

    Vision vision;

    [Header("Vision Objects")]
    //Shooting shoot;
    public Transform target;
    public Resource resourceObject;
    public HealthResource healthObject;
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

    public float health;
    public float attack;
    public float speed;
    public float maxHealth;
    public float capture;
    public float visionRange;

    public float fitnessScore;

    public Slider slider;

    protected virtual void Start()
    {
        vision = transform.Find("Vision").GetComponent<Vision>();
        fitnessScore = 0f;
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
        if (healthObject != null && !vision.targets.Contains(healthObject.gameObject))
        {
            healthObject = null;
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
            else if (target.GetComponent<HealthResource>())
            {
                if (healthObject == null)
                    healthObject = target.GetComponent<HealthResource>();
                else if (Vector3.Distance(target.transform.position, transform.position) < Vector3.Distance(healthObject.transform.position, transform.position))
                    healthObject = target.GetComponent<HealthResource>();
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

    

    public virtual void DestroyWhenHealthIsZero()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public virtual void SetHealth()
    {
        float norm = (health - 0f) / (maxHealth - 0f);
        slider.value = norm;
    }

    public virtual void IsAttacked()
    {
        if (health > 0)
        {
            health -= Time.deltaTime;
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
            health += (Time.deltaTime * 3);
        }
    }
}
