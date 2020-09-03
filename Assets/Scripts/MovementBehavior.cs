using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Steering))]
public abstract class MovementBehavior : Drone
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
    public Territory territoryObject;
    public Faction1 faction1;
    public Faction2 faction2;
    


    //GameObject debugRing;

    [Header("Flocking")]
    public float cohesionWeight = 1.5f;
    public float separationWeight = 2f;
    public float velocityMatchWeight = 1f;

    [Header("Arrive")]
    public float arriveSize;


    //----------------------------------------
    // Keys

    protected string wanderStr = "WANDER";
    protected string seekStr = "SEEK";
    protected string fleeStr = "FLEE";
    protected string arriveStr = "ARRIVE";
    protected string flockStr = "FLOCK";
    protected string captureStr = "CAPTURE";

    protected virtual void Start()
    {
        vision = transform.Find("Vision").GetComponent<Vision>();

        GenerateRandomAttributes();
        GetBehaviorPriority();
        //shoot = GetComponent<Shooting>();
    }

    protected virtual void Update()
    {
        HealthDegen();
        GetVisionTargets();

        GetBehaviorPriority();
        DroneBehavior();

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
            else if (target.GetComponent<Territory>())
            {
                if (territoryObject == null)
                    territoryObject = target.GetComponent<Territory>();
                else if (Vector3.Distance(target.transform.position, transform.position) < Vector3.Distance(territoryObject.transform.position, transform.position))
                    territoryObject = target.GetComponent<Territory>();
            }
        }
    }

    protected virtual void GetBehaviorPriority() { }
    protected virtual void DroneBehavior() { }

    private void GenerateRandomAttributes()
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

        wander = (att[0] / sum) * 100f;
        seek = (att[1] / sum) * 100f;
        arrive = (att[2] / sum) * 100f;
        flee = (att[3] / sum) * 100f;
        flock = (att[4] / sum) * 100f;
        capture = (att[5] / sum) * 100f;
    }
}
